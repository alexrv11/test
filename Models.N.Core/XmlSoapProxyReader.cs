using System;
using System.Collections;
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
            SaveNamespaceWithChilds(typeof(T));
        }

        public XmlSoapProxyReader(Stream input, object obj)
            : base(input)
        {
            propNames = new Dictionary<string, string>();
            SaveNamespaceWithChilds(obj);
        }


        private void SaveNamespace(Type type)
        {
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo p in properties)
            {
                var xmlAttribute = (XmlTypeAttribute)Attribute.GetCustomAttribute(
                              (typeof(IEnumerable).IsAssignableFrom(p.PropertyType) ? p.PropertyType.GetElementType() : p.PropertyType),
                              typeof(XmlTypeAttribute)
                            );

                if (xmlAttribute != null && !String.IsNullOrEmpty(xmlAttribute.Namespace))
                    propNames.Add(p.Name, xmlAttribute.Namespace);
            }
        }

        private void SaveNamespaceWithChilds(Type type)
        {
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo p in properties)
            {
                var t = p.PropertyType.IsArray ? p.PropertyType.GetElementType() : p.PropertyType;
                bool isPrimitiveType = t.IsPrimitive || t.IsValueType || (t == typeof(string));

                if (t == typeof(object))
                {
                    var elementAttribute = (XmlElementAttribute[])Attribute.GetCustomAttributes(
                                 p,
                                 typeof(XmlElementAttribute)
                               );

                    foreach (var item in elementAttribute)
                    {
                        var et = item.Type;

                        var name = GetNamespace(et);

                        if (!String.IsNullOrEmpty(name) && propNames.GetValueOrDefault<string, string>(item.ElementName) == null)
                            propNames.Add(item.ElementName, name);

                        SaveNamespaceWithChilds(et);
                    }
                }
                else
                {
                    var name = GetNamespace(t);
                    if (!String.IsNullOrEmpty(name) && propNames.GetValueOrDefault<string, string>(p.Name) == null)
                        propNames.Add(p.Name, name);

                    if (!isPrimitiveType)
                        SaveNamespaceWithChilds(t);
                }
            }
        }

        public string GetNamespace(Type t)
        {
            var xmlAttribute = (XmlTypeAttribute)Attribute.GetCustomAttribute(
                      t,
                      typeof(XmlTypeAttribute)
                    );

            return xmlAttribute?.Namespace;
        }

        public string GetNamespace(PropertyInfo p)
        {
            var xmlAttribute = (XmlTypeAttribute)Attribute.GetCustomAttribute(
                      p,
                      typeof(XmlTypeAttribute)
                    );

            return xmlAttribute?.Namespace;
        }

        private void SaveNamespaceWithChilds(object obj)
        {
            var type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo p in properties)
            {
                var t = p.PropertyType.IsArray ? p.PropertyType.GetElementType() : p.PropertyType;
                bool isPrimitiveType = t.IsPrimitive || t.IsValueType || (t == typeof(string));

                var xmlAttribute = (XmlTypeAttribute)Attribute.GetCustomAttribute(
                              t,
                              typeof(XmlTypeAttribute)
                            );

                if (xmlAttribute != null && !String.IsNullOrEmpty(xmlAttribute.Namespace) && propNames.GetValueOrDefault<string, string>(p.Name) == null)
                    propNames.Add(p.Name, xmlAttribute.Namespace);

                if (!isPrimitiveType)
                    SaveNamespaceWithChilds(p.GetValue(obj, null));
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
