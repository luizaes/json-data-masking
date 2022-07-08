using JsonDataMasking.Attributes;
using System.Text.Json.Serialization;

namespace JsonDataMasking.Test.MockData
{
    public class CreditCardMock
    {
        [SensitiveData]
        [JsonPropertyName("securityCode")]
        public int SecurityCode { get; set; }
    }
}
