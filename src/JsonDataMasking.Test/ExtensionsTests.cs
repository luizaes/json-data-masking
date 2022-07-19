using JsonDataMasking.Test.MockData;
using System;
using System.Collections.Generic;
using Xunit;

namespace JsonDataMasking.Test
{
    public class ExtensionsTests
    {
        [Fact]
        public void ConvertToGenericType_ConvertsIEnumerableType_WhenGenericTypeIsValid()
        {
            // Arrange
            var customer = new CustomerMock
            {
                FullName = "Jane Doe"
            };

            var customers = new List<object> { customer };

            // Act
            var convertedIEnumerable = customers.ConvertToGenericType<CustomerMock>();

            // Assert
            Assert.IsAssignableFrom<IEnumerable<CustomerMock>>(convertedIEnumerable);
        }

        [Fact]
        public void ConvertToGenericType_ThrowsInvalidCastException_WhenGenericTypeIsNotValid()
        {
            // Arrange
            var login = new LoginMock
            {
                Email = "email@email.com"
            };

            var customers = new List<object> { login }; 

            // Act and assert
            Assert.ThrowsAny<InvalidCastException>(() => customers.ConvertToGenericType<CustomerMock>());
        }
    }
}
