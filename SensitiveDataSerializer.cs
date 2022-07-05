using System.Reflection;
using System.Text.Json;

namespace JsonDataMasking
{
    public static class SensitiveData
    {
        public static string Serialize<T>(T data)
        {
            var properties = GetPropertiesWithSensitiveDataAttribute(typeof(T));

            foreach(PropertyInfo property in properties)
            {
                var attribute = property.GetCustomAttribute<SensitiveDataAttribute>()!;
                var propertyValue = property.GetValue(data);
                var propertySize = attribute.PreserveLength ? propertyValue!.ToString()!.Length : 5;

                property.SetValue(data, new string(attribute.Mask.First(), propertySize));
            }

            return JsonSerializer.Serialize<T>(data);
        }

        public static IEnumerable<PropertyInfo> GetPropertiesWithSensitiveDataAttribute(Type type)
        {
            if(type is null)
                throw new ArgumentNullException(nameof(type));

            foreach(PropertyInfo property in type.GetProperties())
            {
                if(property.GetCustomAttribute<SensitiveDataAttribute>() is not null)
                    yield return property;

                if(property.PropertyType.IsClass)
                {
                    foreach(PropertyInfo childProperty in GetPropertiesWithSensitiveDataAttribute(property.PropertyType))
                    {
                        yield return childProperty;
                    }
                }
            }
        }
    }
}
