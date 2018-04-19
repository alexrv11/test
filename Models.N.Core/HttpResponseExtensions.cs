using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Net.Http;
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

        public static T SoapContentAsJsonType<T>(this HttpResponseMessage message)
        {
            var operation = typeof(T);
            var data = message.Content.ReadAsStringAsync().Result.Replace("$", "Value");
            if (string.IsNullOrEmpty(data)) return default(T);

            var response = JObject.Parse(data).SelectToken($"..{operation.Name}").ToString();

            var objectResult = JsonConvert.DeserializeObject<T>(response);

            return objectResult;

        }

        public static T SoapContentAsXmlType<T>(this HttpResponseMessage message)
        {
            var operation = typeof(T);
            var xmlSer = new XmlSerializer(operation);
            var data = message.Content.ReadAsStringAsync().Result.Replace("$", "#text");

            var nodes = JsonConvert.DeserializeXNode(data).Descendants(operation.Name).FirstOrDefault().Nodes();
            var xDoc = new XDocument(
                new XElement(operation.Name,
                    new object[]{ new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                    new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"),
                    nodes}));

            using (var ms = new MemoryStream())
            {
                using (var xmlW = XmlWriter.Create(ms))
                {
                    xDoc.WriteTo(xmlW);
                }
                ms.Position = 0;
                var ns = new XmlSoapProxyReader<T>(ms);

                var xdoc2 = ns.ReadInnerXml();
                var xdoc3 = XElement.Load(ns);
                var xdoc4 = RemoveAllBlankNamespaces(xdoc3);

                var result = (T)xmlSer.Deserialize(ns);

                return result;
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


        public static XElement RemoveAllBlankNamespaces(XElement e)
        {
            return new XElement(e.Name.LocalName,
               (from n in e.Nodes()
                select ((n is XElement) ? RemoveAllBlankNamespaces(n as XElement) : n)),
               (e.HasAttributes) ? (from a in e.Attributes()
                                    where (!a.IsNamespaceDeclaration || (a.IsNamespaceDeclaration && a.Value != ""))
                                    select new XAttribute(a.Name.LocalName, a.Value)) : null);
        }
    }
}
