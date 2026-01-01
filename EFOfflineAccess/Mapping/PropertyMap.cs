using System;
using System.Collections.Generic;
using System.Text;

namespace EFOfflineModels.Mapping
{
    public sealed class PropertyMap
    {
        public string PropertyName { get; private set; }
        public string ColumnName { get; private set; }
        public Type PropertyType { get; private set; }

        public Func<object, object> Getter { get; private set; }
        public Action<object, object> Setter { get; private set; }

        public bool IsKey { get; private set; }

        public PropertyMap(
            string propertyName,
            string columnName,
            Type propertyType,
            Func<object, object> getter,
            Action<object, object> setter,
            bool isKey)
        {
            PropertyName = propertyName;
            ColumnName = columnName;
            PropertyType = propertyType;
            Getter = getter;
            Setter = setter;
            IsKey = isKey;
        }
    }
}
