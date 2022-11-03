using JsonDataMasking.Attributes;
using System.Text.Json.Serialization;

namespace JsonDataMasking.Test.MockData
{
    public class AuthCredentialsMock
    {
        [SensitiveData]
        [JsonPropertyName("apiKey")]
        public object? ApiKey { get; set; }

        [SensitiveData]
        [JsonPropertyName("apiToken")]
        public string? ApiToken { get; set; }
    }
}
