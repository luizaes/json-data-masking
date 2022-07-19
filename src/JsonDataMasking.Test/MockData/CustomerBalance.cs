using JsonDataMasking.Attributes;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JsonDataMasking.Test.MockData
{
    public class CustomerBalance
    {
        [SensitiveData]
        [JsonPropertyName("accountsBalance")]
        public Dictionary<string, int>? AccountsBalance { get; set; }
    }
}
