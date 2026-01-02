using System;
using System.Collections.Generic;
using System.Text;


namespace EFOfflineModels.Attributes
{
    /// <summary>
    /// Specifies the database table name associated with a class.
    /// </summary>
    /// <remarks>Apply this attribute to a class to indicate the name of the database table it maps to. This
    /// is commonly used in object-relational mapping (ORM) scenarios to explicitly define the table name when it
    /// differs from the class name.</remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the table represented by this instance.
        /// </summary>
        public string TableName { get; }
        /// <summary>
        /// Initializes a new instance of the TableNameAttribute class with the specified table name.
        /// </summary>
        /// <param name="tableName"></param>
        public TableNameAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
