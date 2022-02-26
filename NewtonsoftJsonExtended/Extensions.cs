using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NewtonsoftJsonExtended
{
    public static class Extensions
    {
        public static T FromJson<T>(this string jsonText, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeObject<T>(jsonText, settings);
        }
        public static string ToJson<T>(this T obj, JsonSerializerSettings settings = null)
        {
            return JsonConvert.SerializeObject(obj, settings);
        }

        public static dynamic ToDynamic(this string jsonText, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeObject<dynamic>(jsonText, settings);
        }
        public static object ToObject(this string jsonText, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeObject<object>(jsonText, settings);
        }

        public static T Deserialize<T>(this byte[] data, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeObject<T>(data.ToText(), settings);
        }

        public static T Deserialize<T>(this Stream stream, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeObject<T>(stream.ToText(), settings);
        }

        public static string ToIndentedJson<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            });
        }
        public static string ToFormattedJson(this string jsonText)
        {
            jsonText = jsonText.Trim().Trim(',');
            string jsonFormatted = JToken.Parse(jsonText).ToString(Formatting.Indented);
            return jsonFormatted;
        }

        public static bool? IsOpenApiDocument(string swaggerJsonText)
        {
            try
            {
                var parsedJson = JToken.Parse(swaggerJsonText);

                var openapi = parsedJson?["openapi"];
                var swagger = parsedJson?["swagger"];
                var info = parsedJson?["info"];
                var title = parsedJson?["info"]?["title"];
                var version = parsedJson?["info"]?["version"];
                var paths = parsedJson?["paths"];

                var status = (openapi != null || swagger != null)
                             && info != null
                             && version != null
                             && paths != null
                             && title != null;

                return status;
            }
            catch
            {
                return null;
            }
        }

        public static IEnumerable<string> GetKeys(this JToken jToken)
        {
            var keys = new List<string>();
            var jTokenKey = $"{(string.IsNullOrEmpty(jToken.Path) ? "$" : "$." + jToken.Path)}-{jToken.Type}";
            keys.Add(jTokenKey);
            foreach (var child in jToken.Children())
            {
                var key = child.GetKeys();
                keys.AddRange(key);
            }
            return keys;
        }

        public static IEnumerable<string> GetPaths(this JToken jToken)
        {
            var result = jToken.GetKeys()
                .Select(x => x.Substring(0, x.LastIndexOfAny(new[] { '-' })))
                .Distinct();
            return result;
        }

        private static string ToText(this byte[] bytes)
        {
            return Encoding.UTF32.GetString(bytes);
        }

        private static string ToText(this Stream @this)
        {
            using var sr = new StreamReader(@this, Encoding.UTF8);
            return sr.ReadToEnd();
        }
    }
}
