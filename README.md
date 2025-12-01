[![NuGet Version](https://img.shields.io/nuget/v/JsonDataMasking)](https://www.nuget.org/packages/JsonDataMasking/)
![CI](https://github.com/luizaes/json-data-masking/actions/workflows/ci.yml/badge.svg)

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

   > PS.: These customization fields only work for fields with a `string` base type. 

2. Call the `JsonMask.MaskSensitiveData()` function, passing in your object instance as a parameter.

## Support

### Base Types
This library supports masking of the following base types:
- `string` fields, which are masked following the rules detailed in the [Usage](#usage) section.
- Other types such as `bool`, `(s)byte`, `(u)short`, `(u)int`, `(u)long`, `float`, `double`, `decimal`, `char`, `Datetime`, `DatetimeOffset`, and `Guid`, which are set to their respective default values when having the `[SensitiveData]` attribute.
- Any other base types are currently NOT supported.

### Collections
The library also supports the masking of some collections, such as:
- `List<T>`, where T is a class or `string`.
- `IEnumerable<T>`, where T is a class or `string`.
- `Dictionary<string, string>`.
- Any other collection type, such as `Array`, `ArrayList<T>`, etc., is NOT supported.

### Nested class fields
Nested class properties are also masked, independently of depth. 

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
```

## Dependencies
- [DeepCloner](https://github.com/force-net/DeepCloner)