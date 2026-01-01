using EFOfflineModels.Attributes;
using EFOfflineModels.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace EFOfflineModels
{
    public static class ModelDataConverter
    {

        public static List<T> FromDataTable<T>(DataTable table)  where T : class, IDataTableModel, new()
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

        public static DataTable ToDataTable<T>(IEnumerable<T> items)where T : class, IDataTableModel
        {
            var map = MappingCache.Get<T>();
            var table = new DataTable(map.ModelType.Name);

            foreach (var prop in map.Properties)
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                table.Columns.Add(prop.ColumnName, type);
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
        public static List<T> Clone<T>(IEnumerable<T> source) where T : class, IDataTableModel, new()
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

        public static T Clone<T>(T source) where T : class, IDataTableModel, new()
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

        public static IReadOnlyList<PropertyChange> Compare<T>(T original, T current)
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
