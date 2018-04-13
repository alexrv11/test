using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Services.N.Core.HttpClient
{
    public class XmlSoapProxyReader<T> : XmlTextReader
    {
        public Dictionary<string, string> propNames;
        public XmlSoapProxyReader(Stream input)
            : base(input)
        {
            propNames = new Dictionary<string, string>();
            SaveNamespace(typeof(T));
        }

        private void SaveNamespace(Type type)
        {
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo p in properties)
            {
                 var xmlAttribute = (XmlTypeAttribute)Attribute.GetCustomAttribute(
                               p.PropertyType,
                               typeof(XmlTypeAttribute)
                             );

                if (xmlAttribute!= null && !String.IsNullOrEmpty(xmlAttribute.Namespace))
                    propNames.Add(p.Name, xmlAttribute.Namespace);
            }
        }
        
        public override string NamespaceURI
        {
            get
            {
                string localname = LocalName;

                return propNames.GetValueOrDefault<string, string>(localname.ToString()) ?? string.Empty;
            }
        }
    }
}
