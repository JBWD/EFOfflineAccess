using System;
using System.Collections.Generic;
using System.Text;

namespace EFOfflineModels.Mapping
{
    /// <summary>
    /// Represents a mapping between a class property and a database column, including accessors and metadata.
    /// </summary>
    /// <remarks>Use this class to define how an object property is associated with a database column for data
    /// access or object-relational mapping scenarios. Each instance encapsulates the property name, column name, type
    /// information, and delegates for getting and setting the property value. The mapping also indicates whether the
    /// property serves as a key in the data model.</remarks>
    public sealed class PropertyMap
    {
        /// <summary>
        /// Gets the name of the property associated with this instance.
        /// </summary>
        public string PropertyName { get; private set; }
        /// <summary>
        /// Gets the name of the column associated with this instance.
        /// </summary>
        public string ColumnName { get; private set; }
        /// <summary>
        /// Gets the type of the property represented by this instance.
        /// </summary>
        public Type PropertyType { get; private set; }
        /// <summary>
        /// Gets the delegate used to retrieve the value of an object property.
        /// </summary>
        /// <remarks>The delegate takes an object instance as input and returns the property's value. This
        /// property is typically set during object initialization and is read-only after construction.</remarks>
        public Func<object, object> Getter { get; private set; }
        /// <summary>
        /// Gets the delegate used to set the value of a property or field on a target object.
        /// </summary>
        /// <remarks>The setter delegate takes two parameters: the target object and the value to assign.
        /// This property is typically used in scenarios that require dynamic property or field assignment, such as
        /// serialization or object mapping. The delegate must be compatible with the target object's type and the value
        /// being assigned; otherwise, a runtime exception may occur.</remarks>
        public Action<object, object> Setter { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this property represents a key field.
        /// </summary>
        public bool IsKey { get; private set; }

        /// <summary>
        /// Initializes a new instance of the PropertyMap class, defining the mapping between an object property and a
        /// database column, including accessors and key designation.
        /// </summary>
        /// <remarks>Use this constructor to configure how an object's property is mapped to a database
        /// column, including specifying custom accessors and whether the property is a key. This is typically used in
        /// object-relational mapping scenarios.</remarks>
        /// <param name="propertyName">The name of the property in the object to be mapped. Cannot be null or empty.</param>
        /// <param name="columnName">The name of the corresponding column in the database. Cannot be null or empty.</param>
        /// <param name="propertyType">The type of the property being mapped. Must be a valid Type instance.</param>
        /// <param name="getter">A function that retrieves the value of the property from an object instance. Cannot be null.</param>
        /// <param name="setter">An action that sets the value of the property on an object instance. Cannot be null.</param>
        /// <param name="isKey">Indicates whether the property is part of the primary key for the mapping. Set to <see langword="true"/> if
        /// the property is a key; otherwise, <see langword="false"/>.</param>
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
