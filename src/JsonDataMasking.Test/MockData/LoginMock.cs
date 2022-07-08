using JsonDataMasking.Attributes;
using System.Text.Json.Serialization;

namespace JsonDataMasking.Test.MockData
{
    public class LoginMock
    {
        [SensitiveData(ShowFirst = 10, ShowLast = 6)]
        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }
}
