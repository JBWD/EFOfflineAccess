using EFOfflineModels.Attributes;
using EFOfflineModels.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace EFOfflineModels
{
    /// <summary>
    /// Provides static methods for converting between data table representations and strongly-typed model objects, as
    /// well as cloning and comparing model instances.
    /// </summary>
    /// <remarks>The ModelDataConverter class is intended for use with model types that implement the
    /// IDataTableModel interface. It supports mapping model properties to data table columns, enabling efficient
    /// conversion and comparison operations. All methods are thread-safe and do not maintain internal state.</remarks>
    public static class ModelDataConverter
    {

        /// <summary>
        /// Creates a list of strongly-typed model instances from the rows of the specified <see cref="DataTable"/>.
        /// </summary>
        /// <remarks>Only columns present in both the <see cref="DataTable"/> and the model mapping are
        /// set. Columns with <see cref="DBNull.Value"/> are ignored. This method requires that a mapping for
        /// <typeparamref name="T"/> is available in the mapping cache.</remarks>
        /// <typeparam name="T">The type of model to create for each row. Must implement <see cref="IDataTableModel"/> and have a
        /// parameterless constructor.</typeparam>
        /// <param name="table">The <see cref="DataTable"/> containing the data to convert. Each row is mapped to an instance of
        /// <typeparamref name="T"/>.</param>
        /// <returns>A list of <typeparamref name="T"/> instances, each representing a row in the specified <see
        /// cref="DataTable"/>. The list will be empty if the table contains no rows.</returns>
        public static List<T> ToModelList<T>(this DataTable table)  where T : class, IDataTableModel, new()
        {
            var map = MappingCache.Get<T>();
            var list = new List<T>();

            foreach (DataRow row in table.Rows)
            {
                var item = new T();

                foreach (var prop in map.Properties)
                {
                    if (!table.Columns.Contains(prop.ColumnName))
                        continue;

                    var value = row[prop.ColumnName];
                    if (value == DBNull.Value)
                        continue;

                    prop.Setter(item, value);
                }

                list.Add(item);
            }

            return list;
        }
        /// <summary>
        /// Creates a <see cref="DataTable"/> from a collection of model objects, mapping each property to a
        /// corresponding column.
        /// </summary>
        /// <remarks>The resulting <see cref="DataTable"/> will have its columns and key property
        /// determined by the mapping configuration for <typeparamref name="T"/>. Properties marked as key are stored in
        /// the table's extended properties under the "Key" key.</remarks>
        /// <typeparam name="T">The type of model objects to convert. Must implement <see cref="IDataTableModel"/>.</typeparam>
        /// <param name="items">The collection of model objects to be converted into rows of the <see cref="DataTable"/>. Cannot be null.</param>
        /// <returns>A <see cref="DataTable"/> containing one row for each item in <paramref name="items"/>, with columns
        /// corresponding to the mapped properties of <typeparamref name="T"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown if more than one property in <typeparamref name="T"/> is marked as a key property.</exception>
        public static DataTable ToDataTable<T>(this IEnumerable<T> items)where T : class, IDataTableModel
        {
            var map = MappingCache.Get<T>();
            var table = new DataTable(map.ModelType.Name);
            

            foreach (var prop in map.Properties)
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                table.Columns.Add(prop.ColumnName, type);
                if(prop.IsKey)
                {
                    if(table.ExtendedProperties.Contains("Key"))
                    {
                        throw new InvalidOperationException($"Multiple key properties defined on type {map.ModelType.FullName}. Only one property can be marked with the [Key] attribute.");
                    }
                    table.ExtendedProperties["Key"] = prop.ColumnName;
                }
            }

            foreach (var item in items)
            {
                var row = table.NewRow();

                foreach (var prop in map.Properties)
                {
                    row[prop.ColumnName] = prop.Getter(item) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;
        }
        /// <summary>
        /// Creates a deep copy of each item in the specified collection and returns a new list containing the cloned
        /// items.
        /// </summary>
        /// <remarks>Each item is cloned by copying the values of its mapped properties to a new instance.
        /// Changes to the cloned items do not affect the original items, and vice versa.</remarks>
        /// <typeparam name="T">The type of objects to clone. Must be a reference type that implements IDataTableModel and has a
        /// parameterless constructor.</typeparam>
        /// <param name="source">The collection of items to clone. Cannot be null.</param>
        /// <returns>A list containing deep copies of each item in the source collection. The list will be empty if the source
        /// contains no items.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source parameter is null.</exception>
        public static List<T> CloneModels<T>(this IEnumerable<T> source) where T : class, IDataTableModel, new()
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var map = MappingCache.Get<T>();
            var list = new List<T>();

            foreach (var item in source)
            {
                var clone = new T();

                foreach (var prop in map.Properties)
                {
                    prop.Setter(clone, prop.Getter(item));
                }

                list.Add(clone);
            }

            return list;
        }
        /// <summary>
        /// Creates a new instance of the specified data table model type and copies all property values from the source
        /// object.
        /// </summary>
        /// <remarks>The cloned object is a shallow copy; reference-type properties are copied by
        /// reference, not duplicated. The method requires that type T has a parameterless constructor and implements
        /// IDataTableModel.</remarks>
        /// <typeparam name="T">The type of the data table model to clone. Must be a reference type that implements IDataTableModel and has
        /// a parameterless constructor.</typeparam>
        /// <param name="source">The source object from which property values are copied. Cannot be null.</param>
        /// <returns>A new instance of type T with property values copied from the source object.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null.</exception>
        public static T CloneModel<T>(this T source) where T : class, IDataTableModel, new()
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var map = MappingCache.Get<T>();
            var clone = new T();

            foreach (var prop in map.Properties)
            {
                var value = prop.Getter(source);
                prop.Setter(clone, value);
            }

            return clone;
        }
        /// <summary>
        /// Compares two instances of a data table model and returns a list of property changes between them.
        /// </summary>
        /// <typeparam name="T">The type of the data table model to compare. Must implement <see cref="IDataTableModel"/>.</typeparam>
        /// <param name="original">The original instance of the data table model to compare. Cannot be <see langword="null"/>.</param>
        /// <param name="current">The current instance of the data table model to compare against the original. Cannot be <see
        /// langword="null"/>.</param>
        /// <returns>A read-only list of <see cref="PropertyChange"/> objects representing properties whose values differ between
        /// the two instances. The list will be empty if no differences are found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="original"/> or <paramref name="current"/> is <see langword="null"/>.</exception>
        public static IReadOnlyList<PropertyChange> GetDifferences<T>(this T original, T current)
    where T : class, IDataTableModel
        {
            if (original == null) throw new ArgumentNullException(nameof(original));
            if (current == null) throw new ArgumentNullException(nameof(current));

            var map = MappingCache.Get<T>();
            var changes = new List<PropertyChange>();

            foreach (var prop in map.Properties)
            {
                var oldValue = prop.Getter(original);
                var newValue = prop.Getter(current);

                if (Equals(oldValue, newValue))
                    continue;

                changes.Add(new PropertyChange(
                    prop.PropertyName,
                    prop.ColumnName,
                    oldValue,
                    newValue));
            }

            return changes;
        }
    }
}
