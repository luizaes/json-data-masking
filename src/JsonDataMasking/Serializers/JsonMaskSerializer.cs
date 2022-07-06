using JsonDataMasking.Masks;
using System.Text.Json;

namespace JsonDataMasking.Serializers
{
    public static class JsonMaskSerializer
    {
        public static string Serialize<T>(T data)
        {
            data = JsonMask.MaskSensitiveData(data);
            return JsonSerializer.Serialize(data);
        }
    }
}
