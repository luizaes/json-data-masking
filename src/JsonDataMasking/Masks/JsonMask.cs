using JsonDataMasking.Attributes;
using System.Reflection;
using System.Text;

namespace JsonDataMasking.Masks
{
    public static class JsonMask
    {
        private static readonly int DefaultMaskSize = 5;

        public static T MaskSensitiveData<T>(T data)
        {
            var properties = GetPropertiesWithSensitiveDataAttribute(typeof(T));

            foreach (PropertyInfo property in properties)
            {
                var attribute = property.GetCustomAttribute<SensitiveDataAttribute>()!;
                var propertyValue = property.GetValue(data)?.ToString();

                var maskedPropertyValue = GetMaskedPropertyValue(propertyValue, attribute);

                property.SetValue(data, maskedPropertyValue);
            }
            return data;
        }

        private static IEnumerable<PropertyInfo> GetPropertiesWithSensitiveDataAttribute(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            foreach (PropertyInfo property in type.GetProperties())
            {
                if (property.GetCustomAttribute<SensitiveDataAttribute>() is not null
                    && IsTypeValidToMask(property.PropertyType))
                    yield return property;

                if (property.PropertyType.IsClass)
                {
                    foreach (PropertyInfo childProperty in GetPropertiesWithSensitiveDataAttribute(property.PropertyType))
                    {
                        yield return childProperty;
                    }
                }
            }
        }

        private static bool IsTypeValidToMask(Type type) => type switch
        {
            Type _ when type == typeof(string) => true,
            _ => throw new NotSupportedException("Masking of non-string types is not supported")
        };

        private static bool AreFirstAndLastParametersInValidRange(int propertySize, SensitiveDataAttribute attribute) =>
            attribute.ShowFirst <= propertySize && attribute.ShowLast <= propertySize && (attribute.ShowFirst + attribute.ShowLast) <= propertySize;

        private static string? GetMaskedPropertyValue(string? currentPropertyValue, SensitiveDataAttribute attribute)
        {
            if (string.IsNullOrWhiteSpace(currentPropertyValue)) return currentPropertyValue;

            StringBuilder maskedPropertyValueBuilder = new(attribute.SubstituteText);

            if (maskedPropertyValueBuilder.Length == 0)
            {
                var propertySize = currentPropertyValue?.Length ?? 0;
                var maskSize = attribute.PreserveLength ? propertySize - (attribute.ShowFirst + attribute.ShowLast)
                    : DefaultMaskSize;

                if (!AreFirstAndLastParametersInValidRange(propertySize, attribute))
                    throw new ArgumentException($"{nameof(attribute.ShowFirst)} and {nameof(attribute.ShowLast)} values " +
                        $"cannot be greater than the original string length");

                maskedPropertyValueBuilder.Append(currentPropertyValue?[..attribute.ShowFirst]);
                maskedPropertyValueBuilder.Append(currentPropertyValue?.Substring(propertySize - attribute.ShowLast, attribute.ShowLast));
                maskedPropertyValueBuilder.Insert(attribute.ShowFirst, new string(attribute.Mask.First(), maskSize));
            }

            return maskedPropertyValueBuilder.ToString();
        }
    }
}
