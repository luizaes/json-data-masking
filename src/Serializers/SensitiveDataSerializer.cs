using JsonDataMasking.Attributes;
using System.Reflection;
using System.Text.Json;

namespace JsonDataMasking.Serializers
{
    public static class JsonMaskSerializer
    {
        public static string Serialize<T>(T data)
        {
            data = MaskSensitiveData(data);
            return JsonSerializer.Serialize(data);
        }

        private static T MaskSensitiveData<T>(T data)
        {
            var properties = GetPropertiesWithSensitiveDataAttribute(typeof(T));

            foreach (PropertyInfo property in properties)
            {
                var attribute = property.GetCustomAttribute<SensitiveDataAttribute>()!;
                var propertyValue = property.GetValue(data);
                var propertySize = attribute.PreserveLength ? propertyValue!.ToString()!.Length : 5;

                property.SetValue(data, new string(attribute.Mask.First(), propertySize));
            }
            return data;
        }

        private static IEnumerable<PropertyInfo> GetPropertiesWithSensitiveDataAttribute(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            foreach (PropertyInfo property in type.GetProperties())
            {
                if (property.GetCustomAttribute<SensitiveDataAttribute>() is not null)
                {
                    if (property.PropertyType != typeof(string))
                        throw new NotSupportedException("Masking of non-string types is not supported");
                    yield return property;
                }

                if (property.PropertyType.IsClass)
                {
                    foreach (PropertyInfo childProperty in GetPropertiesWithSensitiveDataAttribute(property.PropertyType))
                    {
                        yield return childProperty;
                    }
                }
            }
        }
    }
}
