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
        private readonly string DefaultMask = new('*', JsonMask.DefaultMaskSize);

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

            // Act
            var maskedCustomerData = JsonMask.MaskSensitiveData(customer);

            // Assert
            Assert.Equal(DefaultMask, maskedCustomerData.FullName);
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
        public void MaskSensitiveData_MasksUsingDefaultSize_WhenHasAttributeWithInvalidShowFirstAndLastRange()
        {
            // Arrange
            var login = new LoginMock { Email = "email@email.com" };

            // Act
            var maskedLogin = JsonMask.MaskSensitiveData(login);

            // Assert 
            Assert.Equal(DefaultMask, maskedLogin.Email);
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

            // Act
            var maskedCustomerData = JsonMask.MaskSensitiveData(customer);

            // Assert
            Assert.Equal(DefaultMask, maskedCustomerData.Address?.ZipCode);
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

            // Act
            var maskedCustomerData = JsonMask.MaskSensitiveData(customer);

            // Assert
            Assert.True(maskedCustomerData.Documents?.All(value => DefaultMask.Equals(value)));
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

            // Act
            var maskedCustomerData = JsonMask.MaskSensitiveData(customer);

            // Assert
            Assert.True(maskedCustomerData.CustomFields?.All(pair => DefaultMask.Equals(pair.Value)));
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

            // Act
            var maskedCustomerData = JsonMask.MaskSensitiveData(customer);

            // Assert
            Assert.True(maskedCustomerData.CustomerIds?.All(id => DefaultMask.Equals(id)));
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

            // Act
            var maskedCustomerData = JsonMask.MaskSensitiveData(customer);

            // Assert
            Assert.True(maskedCustomerData.Addresses?.All(address => DefaultMask.Equals(address?.ZipCode)));
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

        [Fact]
        public void MaskSensitiveData_DoesNotModifyOriginalObject_WhenMaskingTheObjectsProperties()
        {
            // Arrange
            var customer = new CustomerMock
            {
                FullName = "John Doe"
            };

            // Act
            var maskedCustomer = JsonMask.MaskSensitiveData(customer);

            // Assert
            Assert.NotEqual(customer.FullName, maskedCustomer.FullName);
        }

        [Fact]
        public void MaskSensitiveData_ThrowsArgumentNullException_WhenObjectIsNull()
        {
            // Arrange
            CustomerMock? customer = null;

            // Act and assert
            Assert.Throws<ArgumentNullException>(() => JsonMask.MaskSensitiveData(customer));
        }

        [Fact]
        public void MaskSensitiveData_ThrowsNotSupportedException_WhenPropertyHasBaseObjectType()
        {
            // Arrange
            var credentials = new AuthCredentialsMock
            {
                ApiKey = new CreditCardMock { SecurityCode = 123},
                ApiToken = "123"
            };

            // Act and assert
            Assert.Throws<NotSupportedException>(() => JsonMask.MaskSensitiveData(credentials));
        }

        [Fact]
        public void MaskSensitiveData_MasksProperty_WhenDataHasAnonymousType()
        {
            // Arrange
            var customer = new CustomerMock
            {
                FullName = "John Doe"
            };
            var anonymousObj = new { customer };
            var expectedObj = new CustomerMock { FullName = "*****" };

            // Act
            var maskedAnonymousObj = JsonMask.MaskSensitiveData(anonymousObj);

            // Assert
            Assert.Equal(expectedObj.FullName, maskedAnonymousObj.customer.FullName);
        }

        [Fact]
        public void MaskSensitiveData_DoesNotEnterInfiniteLoop_WhenDataHasACycle()
        {
            // Arrange
            var loopObj = new LoopMock
            {
                Child = new LoopMock
                {
                    Child = new LoopMock()
                }
            };

            // Act
            var maskedLoopObj = JsonMask.MaskSensitiveData(loopObj);

            // Assert
            Assert.NotNull(maskedLoopObj);
        }

        [Fact]
        public void MaskSensitiveData_ThrowsNotSupportedException_WhenPropertyIsArray()
        {
            // Arrange
            var passcodes = new PasscodesMock
            {
                Passcodes = new[]
                {
                    "test1234",
                    "abcd"
                }
            };

            // Act and assert
            Assert.Throws<NotSupportedException>(() => JsonMask.MaskSensitiveData(passcodes));
        }

        [Fact]
        public void MaskSensitiveData_MasksOtherBaseTypes_WhenTheyHaveAttribute()
        {
            // Arrange
            var baseTypes = new BasicTypesSensitiveMock();

            // Act
            var maskedBaseTypes = JsonMask.MaskSensitiveData(baseTypes);

            // Assert
            Assert.False(maskedBaseTypes.Bool);
            Assert.Equal(0, maskedBaseTypes.Byte);
            Assert.Equal(0, maskedBaseTypes.Sbyte);
            Assert.Equal(0, maskedBaseTypes.Short);
            Assert.Equal(0, maskedBaseTypes.Ushort);
            Assert.Equal(0, maskedBaseTypes.Int);
            Assert.Equal(0u, maskedBaseTypes.Uint);
            Assert.Equal(0, maskedBaseTypes.Long);
            Assert.Equal(0u, maskedBaseTypes.Ulong);
            Assert.Equal(0, maskedBaseTypes.Float);
            Assert.Equal(0, maskedBaseTypes.Double);
            Assert.Equal(0, maskedBaseTypes.Decimal);
            Assert.Equal('\0', maskedBaseTypes.Char);
            Assert.Equal(new DateTime(), maskedBaseTypes.DateTime);
            Assert.Equal(new DateTimeOffset(), maskedBaseTypes.DateTimeOffset);
            Assert.Equal(Guid.Empty, maskedBaseTypes.Guid);
        }

        [Fact]
        public void MaskSensitiveData_DoesNotMaskOtherBaseTypes_WhenWithoutSensitiveAttribute()
        {
            // Arrange
            var nonSensitiveBaseTypes = new BasicTypesMock();

            // Act
            var maskedBaseTypes = JsonMask.MaskSensitiveData(nonSensitiveBaseTypes);

            // Assert
            Assert.True(maskedBaseTypes.Bool);
            Assert.Equal(1, maskedBaseTypes.Byte);
            Assert.Equal(1, maskedBaseTypes.Sbyte);
            Assert.Equal(1, maskedBaseTypes.Short);
            Assert.Equal(1, maskedBaseTypes.Ushort);
            Assert.Equal(1, maskedBaseTypes.Int);
            Assert.Equal(1u, maskedBaseTypes.Uint);
            Assert.Equal(1, maskedBaseTypes.Long);
            Assert.Equal(1u, maskedBaseTypes.Ulong);
            Assert.Equal(1, maskedBaseTypes.Float);
            Assert.Equal(1, maskedBaseTypes.Double);
            Assert.Equal(1, maskedBaseTypes.Decimal);
            Assert.Equal('1', maskedBaseTypes.Char);
            Assert.Equal(new DateTime().AddYears(1), maskedBaseTypes.DateTime);
            Assert.Equal(new DateTimeOffset().AddYears(1), maskedBaseTypes.DateTimeOffset);
            Assert.Equal(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), maskedBaseTypes.Guid);
        }
    }
}