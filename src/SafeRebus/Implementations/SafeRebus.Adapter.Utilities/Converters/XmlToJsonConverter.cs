using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Rebus.Serialization;
using SafeRebus.Adapters.Abstractions;

namespace SafeRebus.Adapter.Utilities.Converters
{
    public static class XmlToJsonConverter
    {
        public const string ContentTypeConverter = ContentTypes.XmlContentType;

        public static byte[] Convert(byte[] incomingBody, Type bodyType)
        {
            var xmlString = Encoding.UTF8.GetString(incomingBody);
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlString);
            xmlDocument.RemoveUnknownAttributes();
            var @object = DeserializeXml(bodyType, xmlDocument);
            var objectSerializer = new ObjectSerializer();
            var convertedBody = objectSerializer.Serialize(@object);
            return convertedBody;
        }

        private static object DeserializeXml(Type type, XmlDocument xmlDocument)
        {
            var xmlString = xmlDocument.OuterXml;
            var reader = new StringReader(xmlString);
            var xmlSerializer = new XmlSerializer(type);
            var @object = xmlSerializer.Deserialize(reader);
            return @object;
        }

        private static void RemoveUnknownAttributes(this XmlDocument xmlDocument)
        {
            xmlDocument.DocumentElement?.Attributes["xmlns"].RemoveAll();
        }
    }
}