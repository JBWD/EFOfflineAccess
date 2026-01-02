using EFOfflineModels.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace EFOfflineModels.Mapping
{
    /// <summary>
    /// Provides metadata describing the mapping between a model type and its properties, including the key property
    /// used for identification.
    /// </summary>
    /// <remarks>MappingInfo is typically used in scenarios where object-relational or object-data mapping is
    /// required, such as in data access layers or serialization frameworks. It encapsulates information about the model
    /// type, its mapped properties, and the property designated as the key. Instances of this class are immutable after
    /// construction.</remarks>
    public sealed class MappingInfo
    {
       /// <summary>
       /// Initializes a new instance of the MappingInfo class with the specified model type, property mappings, and key
       /// property.
       /// </summary>
       /// <param name="modelType">The type representing the model for which property mappings are defined. Cannot be null.</param>
       /// <param name="properties">A read-only list of PropertyMap objects that describe how each property of the model is mapped. Cannot be
       /// null or contain null elements.</param>
       /// <param name="keyProperty">The PropertyMap that identifies the key property for the model. Cannot be null.</param>
        public MappingInfo(
            Type modelType,
            IReadOnlyList<PropertyMap> properties,
            PropertyMap keyProperty, string tableName)
        {
            ModelType = modelType;
            Properties = properties;
            KeyProperty = keyProperty;
            TableName = tableName;
        }

        /// <summary>
        /// Gets the type of the model associated with this instance.
        /// </summary>
        public Type ModelType { get; private set; }
        /// <summary>
        /// Gets or sets the name of the database table associated with the model.
        /// </summary>
        /// <remarks>This property typically corresponds to the table name in a relational database where instances of the
        public string TableName { get; set; }
        /// <summary>
        /// Gets the collection of property mappings associated with the current object.
        /// </summary>
        /// <remarks>The returned list provides read-only access to the mappings between source and
        /// destination properties. Changes to the collection itself are not permitted.</remarks>
        public IReadOnlyList<PropertyMap> Properties { get; private set; }
        /// <summary>
        /// Gets the property mapping that represents the key for the current entity.
        /// </summary>
        /// <remarks>The key property uniquely identifies each instance of the entity. This property is
        /// typically used in scenarios such as entity tracking, updates, or lookups where the key is
        /// required.</remarks>
        public PropertyMap KeyProperty { get; private set; }

    }
}
