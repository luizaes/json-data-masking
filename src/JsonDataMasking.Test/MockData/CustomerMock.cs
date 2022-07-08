using JsonDataMasking.Attributes;
using System.Text.Json.Serialization;

namespace JsonDataMasking.Test.MockData
{
    public class CustomerMock
    {
        [SensitiveData]
        [JsonPropertyName("fullName")]
        public string? FullName { get; set; }

        [SensitiveData(SubstituteText = "REDACTED")]
        [JsonPropertyName("creditCardNumber")]
        public string? CreditCardNumber { get; set; }

        [SensitiveData(PreserveLength = true, ShowFirst = 3, ShowLast = 3)]
        [JsonPropertyName("document")]
        public string? Document { get; set; }

        [JsonPropertyName("customerType")]
        public string? CustomerType { get; set; }

        [JsonPropertyName("address")]
        public CustomerAddressMock Address { get; set; }
    }
}
