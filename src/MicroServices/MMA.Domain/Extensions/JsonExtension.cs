using Newtonsoft.Json;

namespace MMA.Domain
{
    public static class JsonExtensions
    {
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public static T FromJson<T>(this string json) where T : new()
        {
            return JsonConvert.DeserializeObject<T>(json) ?? new();
        }
    }
}