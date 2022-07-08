using JsonDataMasking.Masks;
using JsonDataMasking.Test.MockData;
using System;
using Xunit;

namespace JsonDataMasking.Test
{
    public class JsonMaskTests
    {
        private const string CustomerDocument = "99999999999";
        private const int ShowElementsNumber = 3;

        [Fact]
        public void MaskSensitiveData_DoesNotMaskField_WhenDoesNotHaveAttribute()
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
        public void MaskSensitiveData_MasksEntireField_WhenHasAttributeAndNoArgumentsAreGiven()
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
        public void MaskSensitiveData_SubstitutesEntireField_WhenHasAttributeWithSubstituteText()
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
        public void MaskSensitiveData_PreservesFieldLength_WhenHasAttributeWithPreserveLength()
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
        public void MaskSensitiveData_ThrowsNotSupportedException_WhenFieldWithNotSupportedTypeHasAttribute()
        {
            // Arrange
            var creditCard = new CreditCardMock { SecurityCode = 999 };

            // Act and Assert
            Assert.Throws<NotSupportedException>(() => JsonMask.MaskSensitiveData(creditCard));
        }

        [Fact]
        public void MaskSensitiveData_ThrowsArgumentException_WhenHasAttributeWithInvalidShowFirstAndLastRange()
        {
            // Arrange
            var login = new LoginMock { Email = "email@email.com" };

            // Act and Assert
            Assert.Throws<ArgumentException>(() => JsonMask.MaskSensitiveData(login));
        }

        [Fact]
        public void MaskSensitiveData_DoesNotMaskField_WhenFieldHasAttributeWithNullValue()
        {
            // Arrange
            var customer = new CustomerMock { };

            // Act
            var maskedCustomerData = JsonMask.MaskSensitiveData(customer);

            // Assert
            Assert.Null(maskedCustomerData.Document);
        }

        [Fact]
        public void MaskSensitiveData_MasksField_WhenNestedFieldHasAttribute()
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
            Assert.Equal(expectedMask, maskedCustomerData.Address.ZipCode);
        }
    }
}