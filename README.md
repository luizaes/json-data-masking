# JSON Data Masking

JSON Data Masking is a library for .NET Core applications that simplifies the masking process of PII/sensitive information.

## Usage

After installing the Nuget package in your project, you need to take the following steps:

1. Add the `[SensitiveData]` attribute in your class properties to indicate what should be masked, and use its available fields to configure the masking:

    - **PreserveLength**: Set to `true` to keep the property length when masking its value. By default this setting is set to `false`.
    - **ShowFirst**: If set, shows the first `N` characters of the property, not masking them. The default value is 0.
    - **ShowLast**: If set, shows the last `N` characters of the property, not masking them. The default value is 0.
    - **SubstituteText**: If set, the entire property value will be override with this text. Note that using this setting will ignore all other settings.
    - **Mask**: Set to a character to use it when masking the property's value. By default, the character `*` is used.

2. Call the `JsonMask.MaskSensitiveData()` function or, call directly the `JsonMaskSerializer.Serialize()` to mask your data and serialize your object with `System.Text.Json`'s serializer.

## Examples

### Attributes
```csharp
public class PropertiesExamples
{
    /// 123456789 results in "*****"
    [SensitiveData]
    public string DefaultMask { get; set; }

    /// 123456789 results in "REDACTED"
    [SensitiveData(SubstituteText = "REDACTED")]
    public string SubstituteMask { get; set; }

    /// 123456789 results in "123*****789"
    [SensitiveData(ShowFirst = 3, ShowLast = 3)]
    public string ShowCharactersMask { get; set; }

    /// 123456789 results in "#########"
    [SensitiveData(PreserveLength = true, Mask = "#")]
    public string PreserveCustomMask { get; set; }
}
```

### Functions
```csharp
var maskedData = JsonMask.MaskSensitiveData(data);
var maskedDataSerialized = JsonMaskSerializer.Serialize(data);
```
