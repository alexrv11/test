using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Services.N.Core.HttpClient
{
    public class XmlSoapProxyReader : XmlTextReader
    {
        Dictionary<string, string> propNames;
        public XmlSoapProxyReader(Stream input)
            : base(input)
        {
            propNames = new Dictionary<string, string>();
        }

        private Type proxyType;
        public Type ProxyType
        {
            get { return proxyType; }
            set
            {
                proxyType = value;
                //SaveNamespace(proxyType);


                //XmlTypeAttribute xmlAttribute = (XmlTypeAttribute)Attribute.GetCustomAttribute(
                //                   proxyType,
                //                   typeof(XmlTypeAttribute)
                //                 );
                //propNames.Add(proxyType.Name, xmlAttribute.Namespace);

                PropertyInfo[] properties = proxyType.GetProperties();
                foreach (PropertyInfo p in properties)
                {
                    var xmlAttribute = (XmlTypeAttribute)Attribute.GetCustomAttribute(
                                   p.PropertyType,
                                   typeof(XmlTypeAttribute)
                                 );

                    propNames.Add(p.Name, xmlAttribute?.Namespace ?? string.Empty);
                }
            }
    }

        private void SaveNamespace(Type value)
        {
            PropertyInfo[] properties = value.GetProperties();

            foreach (PropertyInfo p in properties)
            {
                var xmlAttribute = (XmlTypeAttribute)Attribute.GetCustomAttribute(
                               p.PropertyType,
                               typeof(XmlTypeAttribute)
                             );

                propNames.Add(p.Name, xmlAttribute?.Namespace ?? string.Empty);

                if (!p.PropertyType.IsGenericType)
                    SaveNamespace(p.PropertyType);
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
