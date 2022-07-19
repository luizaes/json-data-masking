using JsonDataMasking.Masks;
using JsonDataMasking.Test.MockData;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace JsonDataMasking.Test
{
    public class JsonMaskTests
    {
        private const string CustomerDocument = "99999999999";
        private const int ShowElementsNumber = 3;

        [Fact]
        public void MaskSensitiveData_DoesNotMaskProperty_WhenDoesNotHaveAttribute()
        {
            // Arrange
            var customerType = "Corporate";
            var customer = new CustomerMock { CustomerType = customerType };

            // Act
            var maskedCustomerData = JsonMask.MaskSensitiveData(customer);

            // Assert
            Assert.Equal(customerType, maskedCustomerData.CustomerType);
        }

        [Fact]
        public void MaskSensitiveData_MasksEntireProperty_WhenHasAttributeAndNoArgumentsAreGiven()
        {
            // Arrange
            var customer = new CustomerMock { FullName = "John Doe" };
            var expectedMask = new string('*', JsonMask.DefaultMaskSize);

            // Act
            var maskedCustomerData = JsonMask.MaskSensitiveData(customer);

            // Assert
            Assert.Equal(expectedMask, maskedCustomerData.FullName);
        }

        [Fact]
        public void MaskSensitiveData_SubstitutesEntireProperty_WhenHasAttributeWithSubstituteText()
        {
            // Arrange
            var customer = new CustomerMock { CreditCardNumber = "9999 9999 9999 9999" };
            var expectedMask = "REDACTED";

            // Act
            var maskedCustomerData = JsonMask.MaskSensitiveData(customer);

            // Assert
            Assert.Equal(expectedMask, maskedCustomerData.CreditCardNumber);
        }

        [Fact]
        public void MaskSensitiveData_PreservesPropertyLength_WhenHasAttributeWithPreserveLength()
        {
            // Arrange
            var customer = new CustomerMock { Document = CustomerDocument };

            // Act
            var maskedCustomerData = JsonMask.MaskSensitiveData(customer);

            // Assert
            Assert.True(CustomerDocument.Length == maskedCustomerData?.Document?.Length);
        }

        [Fact]
        public void MaskSensitiveData_DoesNotMaskFirstFieldElements_WhenHasAttributeWithShowFirst()
        {
            // Arrange
            var customer = new CustomerMock { Document = CustomerDocument };

            // Act
            var maskedCustomerData = JsonMask.MaskSensitiveData(customer);

            // Assert
            Assert.True(CustomerDocument[..ShowElementsNumber] == maskedCustomerData?.Document?[..ShowElementsNumber]);
        }

        [Fact]
        public void MaskSensitiveData_DoesNotMaskLastFieldElements_WhenHasAttributeWithShowLast()
        {
            // Arrange
            var customer = new CustomerMock { Document = CustomerDocument };

            // Act
            var maskedCustomerData = JsonMask.MaskSensitiveData(customer);

            // Assert
            Assert.True(CustomerDocument.Substring(CustomerDocument.Length - ShowElementsNumber, ShowElementsNumber) 
                == maskedCustomerData?.Document?.Substring(CustomerDocument.Length - ShowElementsNumber, ShowElementsNumber));
        }

        [Fact]
        public void MaskSensitiveData_UsesCustomMask_WhenHasAttributeWithCustomMaskCharacter()
        {
            // Arrange
            var customer = new CustomerMock { CreditCardSecurityCode = "123" };
            var expectedMask = new string('#', JsonMask.DefaultMaskSize);

            // Act
            var maskedCustomerData = JsonMask.MaskSensitiveData(customer);

            // Assert
            Assert.Equal(expectedMask, maskedCustomerData.CreditCardSecurityCode);
        }

        [Fact]
        public void MaskSensitiveData_ThrowsNotSupportedException_WhenPropertyWithNotSupportedTypeHasAttribute()
        {
            // Arrange
            var creditCard = new CreditCardMock { SecurityCode = 999 };

            // Act and Assert
            Assert.Throws<NotSupportedException>(() => JsonMask.MaskSensitiveData(creditCard));
        }

        [Fact]
        public void MaskSensitiveData_MasksUsingDefaultSize_WhenHasAttributeWithInvalidShowFirstAndLastRange()
        {
            // Arrange
            var login = new LoginMock { Email = "email@email.com" };
            var expectedMask = new string('*', JsonMask.DefaultMaskSize);

            // Act
            var maskedLogin = JsonMask.MaskSensitiveData(login);

            // Assert 
            Assert.Equal(expectedMask, maskedLogin.Email);
        }

        [Fact]
        public void MaskSensitiveData_DoesNotMaskProperty_WhenPropertyHasAttributeWithNullValue()
        {
            // Arrange
            var customer = new CustomerMock { };

            // Act
            var maskedCustomerData = JsonMask.MaskSensitiveData(customer);

            // Assert
            Assert.Null(maskedCustomerData.Document);
        }

        [Fact]
        public void MaskSensitiveData_MasksProperty_WhenNestedPropertyHasAttribute()
        {
            // Arrange
            var customer = new CustomerMock { 
                Address = new CustomerAddressMock {
                    ZipCode = "99999999"
                } 
            };
            var expectedMask = new string('*', JsonMask.DefaultMaskSize);

            // Act
            var maskedCustomerData = JsonMask.MaskSensitiveData(customer);

            // Assert
            Assert.Equal(expectedMask, maskedCustomerData.Address?.ZipCode);
        }

        [Fact]
        public void MaskSensitiveData_MasksProperty_WhenListHasAttribute()
        {
            // Arrange
            var customer = new CustomerMock
            {
                Documents = new List<string>
                {
                    "12345",
                    "54321"
                }
            };
            var expectedMask = new string('*', JsonMask.DefaultMaskSize);

            // Act
            var maskedCustomerData = JsonMask.MaskSensitiveData(customer);

            // Assert
            Assert.True(maskedCustomerData.Documents?.All(value => expectedMask.Equals(value)));
        }

        [Fact]
        public void MaskSensitiveData_MasksProperty_WhenDictionaryHasAttribute()
        {
            // Arrange
            var customer = new CustomerMock
            {
                CustomFields = new Dictionary<string, string>
                {
                    { "document", "12345" },
                    { "name", "Jane Doe" }
                }
            };
            var expectedMask = new string('*', JsonMask.DefaultMaskSize);

            // Act
            var maskedCustomerData = JsonMask.MaskSensitiveData(customer);

            // Assert
            Assert.True(maskedCustomerData.CustomFields?.All(pair => expectedMask.Equals(pair.Value)));
        }

        [Fact]
        public void MaskSensitiveData_MasksProperty_WhenIEnumerableHasAttribute()
        {
            // Arrange
            var customer = new CustomerMock
            {
                CustomerIds = new List<string>
                {
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString()
                }.AsEnumerable()
            };
            var expectedMask = new string('*', JsonMask.DefaultMaskSize);

            // Act
            var maskedCustomerData = JsonMask.MaskSensitiveData(customer);

            // Assert
            Assert.True(maskedCustomerData.CustomerIds?.All(id => expectedMask.Equals(id)));
        }

        [Fact]
        public void MaskSensitiveData_MasksProperty_WhenNestedCollectionHasAttribute()
        {
            // Arrange
            var customer = new CustomerMock
            {
                Addresses = new List<CustomerAddressMock>
                {
                   new CustomerAddressMock
                   {
                       ZipCode = "9999999"
                   }
                }.AsEnumerable()
            };
            var expectedMask = new string('*', JsonMask.DefaultMaskSize);

            // Act
            var maskedCustomerData = JsonMask.MaskSensitiveData(customer);

            // Assert
            Assert.True(maskedCustomerData.Addresses?.All(address => expectedMask.Equals(address?.ZipCode)));
        }

        [Fact]
        public void MaskSensitiveData_DoesNotMaskProperty_WhenCollectionDoesNotHaveAttribute()
        {
            // Arrange
            var customerAddress = new CustomerAddressMock
            {
                GeoCoordinates = new List<double>
                {
                    1.11111111,
                    1.11111111
                }
            };

            // Act
            var maskedCustomerAddress = JsonMask.MaskSensitiveData(customerAddress);

            // Assert
            Assert.True(maskedCustomerAddress.GeoCoordinates?.All(gc => customerAddress.GeoCoordinates.Contains(gc)));
        }

        [Fact]
        public void MaskSensitiveData_ThrowsNotSupportedException_WhenDictionaryHasInvalidValueType()
        {
            // Arrange
            var customerBalance = new CustomerBalance
            {
                AccountsBalance = new Dictionary<string, int>
                {
                    { "accountBank1", 10000 },
                    { "accountBank2", 50 },
                }
            };

            // Act and assert
            Assert.Throws<NotSupportedException>(() => JsonMask.MaskSensitiveData(customerBalance));
        }
    }
}