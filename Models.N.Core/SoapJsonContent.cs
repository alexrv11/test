using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Services.N.Core.HttpClient
{
    public class SoapJsonContent : StringContent
    {
        public SoapJsonContent(object value, string operation)
            : base(SoapXmlToJson(value, operation), Encoding.UTF8, "application/json")
        {
        }

        public SoapJsonContent(object value, string operation, string mediaType)
            : base(SoapXmlToJson(value, operation), Encoding.UTF8, mediaType)
        {
        }

        public static string SoapXmlToJson(object value, string operation)
        {
            return JsonConvert.SerializeXmlNode(SerializeAndRemoveNamespaces(value, operation)).Replace("#text", "$");
        }

        public static XmlDocument SerializeAndRemoveNamespaces(object value, string operation)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(RemoveAllNamespaces(value, operation));

            return xmlDoc;
        }

        public static string RemoveAllNamespaces(object value, string operation)
        {
            XmlSerializer ser = new XmlSerializer(value.GetType());
            XElement xmlDocumentWithoutNs = null;

            using (MemoryStream memStm = new MemoryStream())
            {
                ser.Serialize(memStm, value);
                memStm.Position = 0;
                xmlDocumentWithoutNs = RemoveAllNamespaces(XElement.Load(memStm));
            }

            var envelope = new XElement("Envelope",
                                        new XElement("Header"),
                                        new XElement("Body",
                                        string.IsNullOrEmpty(operation)? new XElement(xmlDocumentWithoutNs) :
                                        new XElement(operation, xmlDocumentWithoutNs)));

            return envelope.ToString();
        }


        public static XElement RemoveAllNamespaces(XElement e)
        {
            return new XElement(e.Name.LocalName,
               (from n in e.Nodes()
                select ((n is XElement) ? RemoveAllNamespaces(n as XElement) : n)),
               (e.HasAttributes) ? (from a in e.Attributes()
                                    where (!a.IsNamespaceDeclaration)
                                    select new XAttribute(a.Name.LocalName, a.Value)) : null);
        }


        public static string RemoveAllNamespaces(string xmlDocument)
        {
            XElement xmlDocumentWithoutNs = RemoveAllNamespaces(XElement.Parse(xmlDocument));

            return xmlDocumentWithoutNs.ToString();
        }

        //private static XElement RemoveAllNamespaces(XElement xmlDocument)
        //{
        //    if (!xmlDocument.HasElements)
        //    {
        //        XElement xElement = new XElement(xmlDocument.Name.LocalName);
        //        xElement.Value = xmlDocument.Value;

        //        foreach (XAttribute attribute in xmlDocument.Attributes())
        //            xElement.Add(attribute);

        //        return xElement;
        //    }
        //    return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespaces(el)));
        //}
    }
}
