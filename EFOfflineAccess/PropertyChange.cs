using System;
using System.Collections.Generic;
using System.Text;

namespace EFOfflineModels
{
    /// <summary>
    /// Represents a change to a property, including its name, associated column, and the original and current values.
    /// </summary>
    /// <remarks>This class is typically used to track modifications to object properties, such as when
    /// auditing changes or synchronizing state between data models and storage. Each instance captures the details of a
    /// single property change, allowing consumers to inspect what was changed and the values before and after the
    /// modification.</remarks>
    public sealed class PropertyChange
    {
        /// <summary>
        /// Gets the name of the property represented by this instance.
        /// </summary>
        public string PropertyName { get; }
        /// <summary>
        /// Gets the name of the column represented by this instance.
        /// </summary>
        public string ColumnName { get; }
        /// <summary>
        /// Gets the original value of the object before any modifications were made.
        /// </summary>
        public object OriginalValue { get; }
        /// <summary>
        /// Gets the current value held by the instance.
        /// </summary>
        public object CurrentValue { get; }
        /// <summary>
        /// Initializes a new instance of the PropertyChange class to represent a change in a property and its
        /// corresponding data column, including the original and current values.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed. Cannot be null.</param>
        /// <param name="columnName">The name of the data column associated with the property. Cannot be null.</param>
        /// <param name="originalValue">The original value of the property before the change. May be null if the property was previously unset.</param>
        /// <param name="currentValue">The current value of the property after the change. May be null if the property is now unset.</param>
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
