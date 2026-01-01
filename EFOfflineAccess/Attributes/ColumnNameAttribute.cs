using System;
using System.Collections.Generic;
using System.Text;

namespace EFOfflineModels.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ColumnNameAttribute : Attribute
    {
        public string ColumnName { get; }

        public ColumnNameAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
