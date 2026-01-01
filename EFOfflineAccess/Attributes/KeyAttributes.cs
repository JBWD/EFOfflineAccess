using System;
using System.Collections.Generic;
using System.Text;

namespace EFOfflineModels.Attributes
{
    /// <summary>
    /// Specifies that a property is a primary key in a data model.
    /// </summary>
    /// <remarks>Apply this attribute to a property to indicate that it represents the unique identifier for
    /// an entity. This is commonly used in object-relational mapping (ORM) frameworks to identify the key property for
    /// database operations. Only one property in an entity class should be marked with the <see
    /// cref="KeyAttribute"/>.</remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class KeyAttribute : Attribute
    {
    }
}
