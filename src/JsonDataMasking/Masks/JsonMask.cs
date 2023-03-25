using Force.DeepCloner;
using JsonDataMasking.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace JsonDataMasking.Masks
{
    public static class JsonMask
    {
        /// <summary>
        /// Default size of the mask, used if <c>PreserveLength</c> is set to <c>false</c>.
        /// </summary>
        public static readonly int DefaultMaskSize = 5;

        #region Masking
        /// <summary>
        /// Mask values of <c>string</c> and some <c>string collections</c> type class properties that have the <c>[SensitiveData]</c> attribute.
        /// Properties with <c>null</c> values or that don't have the attribute remain unchanged.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns><c>T</c> maskedData</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <c>data</c> parameter is <c>null</c></exception>
        /// <exception cref="NotSupportedException">Thrown if the type of the data passed to the method is not a class or anonymous, or the <c>[SensitiveData]</c> attribute was added to a not supported type</exception>
        public static T MaskSensitiveData<T>(T data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            var dataDeepClone = data.DeepClone();

            if(!IsParameterTypeValid(dataDeepClone))
                throw new NotSupportedException($"{nameof(data)} should either be a class or have an anonymous type");

            return MaskPropertiesWithSensitiveDataAttribute(dataDeepClone);
        }

        private static T MaskPropertiesWithSensitiveDataAttribute<T>(T data)
        {
            var typeProperties = data!.GetType().GetProperties();

            foreach (PropertyInfo property in typeProperties)
            {
                var propertyValue = property.GetValue(data);
                if (propertyValue is null)
                    continue;

                var propertyAttribute = property.GetCustomAttribute<SensitiveDataAttribute>();

                if (IsClassReferenceType(property.PropertyType))
                {
                    MaskClassProperty(data, property);
                }
                else if (IsTypeAssignableToGenericType(property.PropertyType, typeof(List<>))
                    || IsTypeAssignableToGenericType(property.PropertyType, typeof(IEnumerable<>)))
                {
                    MaskIEnumerableProperty(data, property);
                }
                else if (propertyAttribute != null && IsTypeAssignableToGenericType(property.PropertyType, typeof(Dictionary<,>)))
                {
                    MaskDictionaryProperty(data, property);
                }
                else if (propertyAttribute != null && IsSupportedBaseType(property.PropertyType))
                {
                    var maskedPropertyValue = GetMaskedPropertyValue(propertyValue?.ToString(), propertyAttribute);
                    property.SetValue(data, maskedPropertyValue);
                }
            }
            return data;
        }

        private static string? GetMaskedPropertyValue(string? currentPropertyValue, SensitiveDataAttribute attribute)
        {
            if (string.IsNullOrWhiteSpace(currentPropertyValue)) return currentPropertyValue;

            var maskedPropertyValueBuilder = new StringBuilder(attribute.SubstituteText);

            if (maskedPropertyValueBuilder.Length == 0)
            {
                var propertySize = currentPropertyValue?.Length ?? 0;
                var maskSize = attribute.PreserveLength ? propertySize - (attribute.ShowFirst + attribute.ShowLast)
                    : DefaultMaskSize;

                if (!AreFirstAndLastParametersInValidRange(propertySize, attribute))
                    return new string(attribute.Mask.First(), DefaultMaskSize);

                maskedPropertyValueBuilder.Append(currentPropertyValue?[..attribute.ShowFirst]);
                maskedPropertyValueBuilder.Append(currentPropertyValue?.Substring(propertySize - attribute.ShowLast, attribute.ShowLast));
                maskedPropertyValueBuilder.Insert(attribute.ShowFirst, new string(attribute.Mask.First(), maskSize));
            }

            return maskedPropertyValueBuilder.ToString();
        }

        private static void MaskClassProperty<T>(T data, PropertyInfo property)
        {
            var maskedNestedPropertyValue = MaskPropertiesWithSensitiveDataAttribute(property.GetValue(data));
            if (!IsPropertyTypeEqualsToAnonymousType(property))
                property.SetValue(data, maskedNestedPropertyValue);
        }

        private static void MaskIEnumerableProperty<T>(T data, PropertyInfo property)
        {
            var collection = (property.GetValue(data) as IEnumerable)!;
            var maskedCollection = new List<object>();
            Type? collectionType = null;
            var propertyAttribute = property.GetCustomAttribute<SensitiveDataAttribute>();

            foreach (var value in collection)
            {
                if (collectionType is null) collectionType = value.GetType();

                object? maskedCollectionValue = null;
                if (IsClassReferenceType(collectionType))
                    maskedCollectionValue = MaskPropertiesWithSensitiveDataAttribute(value);
                else if (propertyAttribute != null && IsSupportedBaseType(collectionType))
                    maskedCollectionValue = GetMaskedPropertyValue(value?.ToString(), propertyAttribute);

                if (maskedCollectionValue != null)
                    maskedCollection.Add(maskedCollectionValue);
            }
            if (collectionType != null && maskedCollection.Any())
            {
                var typedCollection = ConvertCollectionType(maskedCollection, collectionType);
                property.SetValue(data, typedCollection);
            }
        }

        private static void MaskDictionaryProperty<T>(T data, PropertyInfo property)
        {
            var propertyAttribute = property.GetCustomAttribute<SensitiveDataAttribute>();
            var collection = property.GetValue(data) as IDictionary<string, string>;
            if (collection is null)
                throw new NotSupportedException("Masking of non-string base types in Dictionaries is not supported");

            var maskedCollection = new Dictionary<string, string?>();

            foreach (var pair in collection)
            {
                var maskedCollectionValue = GetMaskedPropertyValue(pair.Value, propertyAttribute);
                maskedCollection.Add(pair.Key, maskedCollectionValue);
            }
            property.SetValue(data, maskedCollection);
        }
        #endregion Masking

        #region Convertion
        private static IEnumerable ConvertCollectionType(IEnumerable collection, Type type)
        {
            var conversionMethod = typeof(Extensions).GetMethod(nameof(Extensions.ConvertToGenericType),
                new[] { typeof(IEnumerable) });
            var genericMethod = conversionMethod.MakeGenericMethod(type);

            return (IEnumerable)genericMethod.Invoke(null, new[] { collection });
        }
        #endregion Convertion

        #region Validations
        private static bool IsParameterTypeValid<T>(T data)
        {
            var parameterType = data!.GetType();
            return parameterType switch
            {
                Type _ when IsClassReferenceType(parameterType) => true,
                Type _ when IsTypeAnonymous(parameterType) => true,
                _ => false
            };
        }

        private static bool IsPropertyTypeEqualsToAnonymousType(PropertyInfo property) =>
            property.ReflectedType.AssemblyQualifiedName.Contains("AnonymousType");

        private static bool IsTypeAnonymous(Type type) =>
            type.Name.Contains("AnonymousType");

        private static bool IsSupportedBaseType(Type type) => type switch
        {
            Type _ when type == typeof(string) => true,
            _ => throw new NotSupportedException("Masking of non-string base types is not supported")
        };

        private static bool AreFirstAndLastParametersInValidRange(int propertySize, SensitiveDataAttribute attribute) =>
            attribute.ShowFirst <= propertySize && attribute.ShowLast <= propertySize && (attribute.ShowFirst + attribute.ShowLast) <= propertySize;

        private static bool IsClassReferenceType(Type type)
        {
            if (type == null || type == typeof(string) || type == typeof(object) || type.BaseType == typeof(Array))
                return false;
            return type.IsClass && !type.IsGenericType;
        }

        private static bool IsTypeAssignableToGenericType(Type originalType, Type targetType)
        {
            return originalType.IsGenericType &&
                originalType.GetGenericTypeDefinition().IsAssignableFrom(targetType);
        }
        #endregion Validations
    }
}
