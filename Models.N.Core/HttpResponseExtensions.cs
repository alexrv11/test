using Newtonsoft.Json;
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

        public static T ContentAsTypeFromSoap<T>(this HttpResponseMessage response, string operation, string ns)
        {
            var data = response.Content.ReadAsStringAsync().Result.Replace("$","Value");
            if (string.IsNullOrEmpty(data)) return default(T);

            var xNode = JsonConvert.DeserializeXNode(data).Descendants(operation).FirstOrDefault();


            var root = new XmlRootAttribute();
            root.ElementName = operation;
            root.IsNullable = true;
            var serializer = new XmlSerializer(typeof(T),root);


            using (var ms = new MemoryStream())
            {
                using (var xmlW = XmlWriter.Create(ms))
                {
                    xNode.WriteTo(xmlW);
                }
                ms.Position = 0;
                var reader = new XmlSoapProxyReader(ms);
                reader.ProxyNamespace = ns;
                reader.ProxyType = typeof(T);
                
                return (T)serializer.Deserialize(reader);
            }
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
