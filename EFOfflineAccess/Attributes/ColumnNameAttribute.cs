using System;
using System.Collections.Generic;
using System.Text;

namespace EFOfflineModels.Attributes
{
    /// <summary>
    /// Specifies the database column name to associate with a property for mapping purposes.
    /// </summary>
    /// <remarks>Apply this attribute to a property to indicate the corresponding column name in the database
    /// when performing object-relational mapping. This is commonly used in frameworks that map class properties to
    /// database columns, such as ORMs. The attribute is intended for use on properties only.</remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ColumnNameAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the column represented by this instance.
        /// </summary>
        public string ColumnName { get; }
        /// <summary>
        /// Initializes a new instance of the ColumnNameAttribute class with the specified column name.
        /// </summary>
        /// <param name="columnName">The name of the database column to associate with the target member. Cannot be null or empty.</param>
        public ColumnNameAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
