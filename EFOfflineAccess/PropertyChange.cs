using System;
using System.Collections.Generic;
using System.Text;

namespace EFOfflineModels
{
    public sealed class PropertyChange
    {
        public string PropertyName { get; }
        public string ColumnName { get; }
        public object OriginalValue { get; }
        public object CurrentValue { get; }

        public PropertyChange(
            string propertyName,
            string columnName,
            object originalValue,
            object currentValue)
        {
            PropertyName = propertyName;
            ColumnName = columnName;
            OriginalValue = originalValue;
            CurrentValue = currentValue;
        }
    }
}
