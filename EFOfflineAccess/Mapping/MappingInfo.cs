using EFOfflineModels.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace EFOfflineModels.Mapping
{
    public sealed class MappingInfo
    {
       
        public MappingInfo(
            Type modelType,
            IReadOnlyList<PropertyMap> properties,
            PropertyMap keyProperty)
        {
            ModelType = modelType;
            Properties = properties;
            KeyProperty = keyProperty;
        }


        public Type ModelType { get; private set; }
        public IReadOnlyList<PropertyMap> Properties { get; private set; }
        public PropertyMap KeyProperty { get; private set; }

    }
}
