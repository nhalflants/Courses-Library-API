using System;
using System.Collections.Generic;
namespace CourseLibrary.API.Services
{
    public class PropertyMapping<TSource, TDestination> : IPropertyMapping
    {
        public Dictionary<string, PropertyMappingValue> _mappingDictionnary { get; private set; }
        
        public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionnary)
        {
            _mappingDictionnary = mappingDictionnary ?? throw new ArgumentNullException(nameof(mappingDictionnary));
        }
    }
}