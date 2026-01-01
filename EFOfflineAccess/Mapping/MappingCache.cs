using EFOfflineModels.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace EFOfflineModels.Mapping
{
    /// <summary>
    /// Provides a thread-safe cache for mapping information between data table model types and their corresponding
    /// property-to-column mappings.
    /// </summary>
    /// <remarks>The cache stores mapping metadata for types implementing <see cref="IDataTableModel"/>,
    /// allowing efficient retrieval of property-to-column associations. This improves performance by avoiding repeated
    /// reflection and mapping construction for the same type. All public members are static and the cache is shared
    /// across the application domain. This class is intended for internal use by data access components that require
    /// mapping between model properties and database columns.</remarks>
    public static class MappingCache
    {
        private static readonly Dictionary<Type, MappingInfo> _cache = new Dictionary<Type, MappingInfo>();
        private static readonly object _lock = new object();

        public static MappingInfo Get<T>() where T : IDataTableModel
            => Get(typeof(T));

        private static MappingInfo Get(Type type)
        {
            lock (_lock)
            {
                if (_cache.TryGetValue(type, out var mapping))
                    return mapping;

                mapping = BuildMapping(type);
                _cache[type] = mapping;
                return mapping;
            }
        }
        /// <summary>
        /// Creates a mapping definition for the specified type by identifying properties decorated with column mapping
        /// attributes.
        /// </summary>
        /// <remarks>Only properties marked with the ColumnNameAttribute are included in the mapping. If a
        /// property is also marked with KeyAttribute, it is designated as the key in the resulting
        /// MappingInfo.</remarks>
        /// <param name="type">The type to analyze for property-to-column mappings. Must not be null.</param>
        /// <returns>A MappingInfo instance containing property mappings and key information for the specified type.</returns>
        private static MappingInfo BuildMapping(Type type)
        {
            var props = type.GetProperties()
                .Where(p => p.GetCustomAttribute<ColumnNameAttribute>() != null)
                .ToArray();

            var maps = new List<PropertyMap>();
            PropertyMap keyMap = null;

            foreach (var prop in props)
            {
                var columnAttr = prop.GetCustomAttribute<ColumnNameAttribute>();
                var isKey = prop.GetCustomAttribute<KeyAttribute>() != null;

                var map = new PropertyMap
                (
                     prop.Name,
                    columnAttr.ColumnName,
                    prop.PropertyType,
                    CompileGetter(type, prop),
                    CompileSetter(type, prop),
                    isKey
                );

                if (isKey)
                    keyMap = map;

                maps.Add(map);
            }

            return new MappingInfo(type,maps,keyMap);
        }

        /// <summary>
        /// Creates a delegate that retrieves the value of the specified property from an object instance.
        /// </summary>
        /// <remarks>The returned delegate casts the input object to the specified type before accessing
        /// the property. If the input object is not of the expected type, an exception will be thrown at runtime. This
        /// method is useful for dynamic property access scenarios, such as serialization or data binding.</remarks>
        /// <param name="type">The type that declares the property to access. Must match the type of the object passed to the returned
        /// delegate.</param>
        /// <param name="prop">The property information representing the property to retrieve. Must be a readable property of the specified
        /// type.</param>
        /// <returns>A delegate that takes an object instance and returns the value of the specified property as an object.</returns>
        private static Func<object, object> CompileGetter(Type type, PropertyInfo prop)
        {
            var instance = Expression.Parameter(typeof(object), "obj");
            var cast = Expression.Convert(instance, type);
            var access = Expression.Property(cast, prop);
            var box = Expression.Convert(access, typeof(object));

            return Expression.Lambda<Func<object, object>>(box, instance)
                             .Compile();
        }

        /// <summary>
        /// Creates a delegate that sets the value of a specified property on an object of a given type using dynamic
        /// invocation.
        /// </summary>
        /// <remarks>The returned delegate performs type conversions as necessary, including handling
        /// nullable property types. If the property is not writable or the types are incompatible, an exception may be
        /// thrown at runtime.</remarks>
        /// <param name="type">The type of the object containing the property to be set. Must not be null.</param>
        /// <param name="prop">The property metadata representing the property to set. Must not be null and must be writable.</param>
        /// <returns>An Action delegate that takes an object instance and a value, and sets the specified property on the
        /// instance to the provided value.</returns>
        private static Action<object, object> CompileSetter(Type type, PropertyInfo prop)
        {
            var instance = Expression.Parameter(typeof(object), "obj");
            var value = Expression.Parameter(typeof(object), "value");

            var castInstance = Expression.Convert(instance, type);
            var castValue = Expression.Convert(
                value,
                Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            var assign = Expression.Assign(
                Expression.Property(castInstance, prop),
                castValue);

            return Expression.Lambda<Action<object, object>>(assign, instance, value)
                             .Compile();
        }
    }
}
