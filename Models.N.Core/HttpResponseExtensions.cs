using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Services.N.Core.HttpClient
{
    public static class HttpResponseExtensions
    {
        public static T ContentAsType<T>(this HttpResponseMessage response)
        {
            var data = response.Content.ReadAsStringAsync().Result;
            return string.IsNullOrEmpty(data) ? 
                            default(T) : 
                            JsonConvert.DeserializeObject<T>(data);
        }

        public static T ContentAsTypeFromJsonSoap<T>(this HttpResponseMessage response)
        {
            var operation = typeof(T).Name;
            var data = response.Content.ReadAsStringAsync().Result.Replace("$","Value");

            if (string.IsNullOrEmpty(data)) return default(T);
            
            var xNode = JsonConvert.DeserializeXNode(data).Descendants(operation).FirstOrDefault();
            var xNodeAsJson = JsonConvert.SerializeXNode(xNode);

            var dataToQuery = JObject.Parse(xNodeAsJson).SelectToken(operation);
            
            return JsonConvert.DeserializeObject<T>(dataToQuery.ToString());
        }

        public static string ContentAsJson(this HttpResponseMessage response)
        {
            var data = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.SerializeObject(data);
        }

        public static string ContentAsString(this HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
