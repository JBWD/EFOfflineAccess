using EFOfflineModels.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace EFOfflineModels.Mapping
{
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

        private static Func<object, object> CompileGetter(Type type, PropertyInfo prop)
        {
            var instance = Expression.Parameter(typeof(object), "obj");
            var cast = Expression.Convert(instance, type);
            var access = Expression.Property(cast, prop);
            var box = Expression.Convert(access, typeof(object));

            return Expression.Lambda<Func<object, object>>(box, instance)
                             .Compile();
        }


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
