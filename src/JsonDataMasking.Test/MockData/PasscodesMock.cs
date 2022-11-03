using JsonDataMasking.Attributes;

namespace JsonDataMasking.Test.MockData
{
    public class PasscodesMock
    {
        [SensitiveData]
        public string[]? Passcodes { get; set; }
    }
}
