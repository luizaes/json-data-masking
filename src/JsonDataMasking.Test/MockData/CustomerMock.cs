using JsonDataMasking.Attributes;
using System.Collections.Generic;
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

        [SensitiveData(Mask = "#")]
        [JsonPropertyName("creditCardSecurityCode")]
        public string? CreditCardSecurityCode { get; set; }

        [SensitiveData(PreserveLength = true, ShowFirst = 3, ShowLast = 3)]
        [JsonPropertyName("document")]
        public string? Document { get; set; }

        [JsonPropertyName("customerType")]
        public string? CustomerType { get; set; }

        [JsonPropertyName("address")]
        public CustomerAddressMock? Address { get; set; }

        [SensitiveData]
        [JsonPropertyName("documents")]
        public List<string>? Documents { get; set; }

        [SensitiveData]
        [JsonPropertyName("customFields")]
        public Dictionary<string, string>? CustomFields { get; set; }

        [JsonPropertyName("addresses")]
        public IEnumerable<CustomerAddressMock>? Addresses { get; set; }

        [SensitiveData]
        [JsonPropertyName("customerIds")]
        public IEnumerable<string>? CustomerIds { get; set; }
    }
}
