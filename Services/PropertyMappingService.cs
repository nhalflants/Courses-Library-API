using System.Linq;
using System;
using System.Collections.Generic;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;

namespace CourseLibrary.API.Services
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
    }

    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _authorPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase) {
                { "Id", new PropertyMappingValue(new List<string>() { "Id" }) },
                { "MainCategory", new PropertyMappingValue(new List<string>() { "MainCategory" }) },
                { "Age", new PropertyMappingValue(new List<string>() { "DateOfBirth" }, true) },
                { "Name", new PropertyMappingValue(new List<string>() { "FirstName",  "LastName"}) },
            };

        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<AuthorDto, Author>(_authorPropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            // Get matching mapping
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();
            if (matchingMapping.Count() == 1)
                return matchingMapping.First()._mappingDictionnary;

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)}{typeof(TDestination)}>");
        }
    }
}