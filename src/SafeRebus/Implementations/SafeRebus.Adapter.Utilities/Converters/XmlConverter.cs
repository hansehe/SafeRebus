using System;
using System.Linq;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using SafeRebus.Adapters.Abstractions;

namespace SafeRebus.Adapter.Utilities.Converters
{
    public static class XmlConverter
    {
        public const string ContentTypeConverter = ContentTypes.NServiceBusContentType;

        public static byte[] Convert(byte[] incomingBody)
        {
            var xmlString = Encoding.UTF8.GetString(incomingBody);
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlString);
            var attribute = xmlDocument.DocumentElement.Attributes["xmlns"];
            var attributeValue = attribute.Value;
            var typeNamespace = attributeValue.Substring(attributeValue.LastIndexOf('/') + 1);
            var assemblyType = AppDomain.CurrentDomain.GetAssemblies().First(x => x.FullName == typeNamespace);
            var jsonString = JsonConvert.SerializeXmlNode(xmlDocument);
            var jsonBody = Encoding.UTF8.GetBytes(jsonString);
            return jsonBody;
        }

        private static T DeserializeXml<T>(string xmlString) where T : new()
        {
            return new T();
        }
    }
}