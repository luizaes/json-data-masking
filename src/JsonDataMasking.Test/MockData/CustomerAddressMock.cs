using JsonDataMasking.Attributes;
using System.Text.Json.Serialization;

namespace JsonDataMasking.Test.MockData
{
    public class CustomerAddressMock
    {
        [SensitiveData]
        [JsonPropertyName("zipCode")]
        public string? ZipCode { get; set; }
    }
}
