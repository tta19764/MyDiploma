using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourcesApp.Classes
{
    public static class JSONLoggerHelper
    {
        public static object TrimForLog(object entity)
        {
            if (entity == null) return null;

            // If it's already a Dictionary (from a previous TrimForLog call), just return it
            if (entity is Dictionary<string, object>)
                return entity;

            // Check if entity is a proxy and get the real entity type
            var entityType = entity.GetType();
            var baseType = entityType;

            // If it's a proxy, get the base entity type
            if (entityType.Namespace == "Castle.Proxies" || entityType.Name.Contains("Proxy"))
            {
                baseType = entityType.BaseType;
            }

            // Create a simple anonymous object with only the primitive properties
            var propertyValues = new Dictionary<string, object>();

            foreach (var prop in baseType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                // Skip properties that can't be read or write
                if (!prop.CanRead) continue;

                // Skip navigation properties and collections that would cause circular references
                if (IsNavigationProperty(prop)) continue;

                // Skip EF Core specific properties
                if (IsEfCoreProperty(prop)) continue;

                try
                {
                    var value = prop.GetValue(entity);

                    // For basic value types, add directly
                    if (value == null || IsSimpleType(prop.PropertyType))
                    {
                        propertyValues[prop.Name] = value;
                    }
                    else
                    {
                        // For complex objects, just store the ID or a string representation
                        propertyValues[prop.Name] = GetSafeValueForLogging(prop.Name, value);
                    }
                }
                catch
                {
                    // If we can't get the property value (due to lazy loading issues), skip it
                    propertyValues[prop.Name] = "[Access Error]";
                }
            }

            return propertyValues;
        }

        public static bool IsNavigationProperty(PropertyInfo prop)
        {
            // Check if it's a navigation property (entity or collection of entities)
            if (prop.PropertyType.Namespace?.StartsWith("HumanResourcesManagement.Models") == true &&
                prop.PropertyType != typeof(string))
                return true;

            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType) &&
                prop.PropertyType != typeof(string))
                return true;

            return false;
        }

        public static bool IsEfCoreProperty(PropertyInfo prop)
        {
            // Check if it's an EF Core specific property
            return prop.Name == "LazyLoader" ||
                   prop.Name.EndsWith("Proxy") ||
                   prop.Name.StartsWith("EF");
        }

        public static bool IsSimpleType(Type type)
        {
            // Check if it's a simple type that can be safely serialized
            return type.IsPrimitive ||
                   type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(Guid) ||
                   type.IsEnum ||
                   Nullable.GetUnderlyingType(type) != null && IsSimpleType(Nullable.GetUnderlyingType(type));
        }

        public static object GetSafeValueForLogging(string propName, object value)
        {
            // Try to get an ID value for entities
            if (propName.EndsWith("Id"))
                return value;

            // For collections, just return the count
            if (value is System.Collections.ICollection collection)
                return $"[Collection: {collection.Count} items]";

            // Try to find an ID property
            var idProp = value.GetType().GetProperty("Id") ??
                         value.GetType().GetProperty($"{value.GetType().Name}Id");

            if (idProp != null)
            {
                try
                {
                    var id = idProp.GetValue(value);
                    return $"[Reference: {value.GetType().Name} #{id}]";
                }
                catch
                {
                    // If we can't get the ID, fall back to ToString
                }
            }

            // Last resort: just use ToString
            try
            {
                return value.ToString();
            }
            catch
            {
                return "[Complex Object]";
            }
        }
    }
}
