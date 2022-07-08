using JsonDataMasking.Attributes;
using System.Reflection;
using System.Text;

namespace JsonDataMasking.Masks
{
    public static class JsonMask
    {
        public static readonly int DefaultMaskSize = 5;

        public static T MaskSensitiveData<T>(T data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            return MaskPropertiesWithSensitiveDataAttribute(data);
        }

        private static T MaskPropertiesWithSensitiveDataAttribute<T>(T data)
        {
            var typeProperties = data!.GetType().GetProperties();

            foreach(PropertyInfo property in typeProperties)
            {
                var propertyValue = property.GetValue(data);
                var propertyAttribute = property.GetCustomAttribute<SensitiveDataAttribute>();
                if (propertyValue is null)
                    continue;

                if (propertyAttribute is not null && IsTypeValidToMask(property.PropertyType))
                {
                    var maskedPropertyValue = GetMaskedPropertyValue(propertyValue?.ToString(), propertyAttribute);
                    property.SetValue(data, maskedPropertyValue);
                } else if (IsNonStringReferenceType(property.PropertyType)) {
                    var maskedNestedPropertyValue = MaskPropertiesWithSensitiveDataAttribute(propertyValue);
                    property.SetValue(data, maskedNestedPropertyValue);  
                }
            }
            return data;
        }

        private static bool IsTypeValidToMask(Type type) => type switch
        {
            Type _ when type == typeof(string) => true,
            _ => throw new NotSupportedException("Masking of non-string types is not supported")
        };

        private static bool AreFirstAndLastParametersInValidRange(int propertySize, SensitiveDataAttribute attribute) =>
            attribute.ShowFirst <= propertySize && attribute.ShowLast <= propertySize && (attribute.ShowFirst + attribute.ShowLast) <= propertySize;

        private static bool IsNonStringReferenceType(Type type)
        {
            if (type == null || type == typeof(string))
                return false;
            return type.IsClass;
        }

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
