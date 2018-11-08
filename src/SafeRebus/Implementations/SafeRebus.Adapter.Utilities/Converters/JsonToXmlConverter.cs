using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Rebus.Serialization;
using SafeRebus.Adapters.Abstractions;

namespace SafeRebus.Adapter.Utilities.Converters
{
    public static class JsonToXmlConverter
    {
        public const string ContentTypeConverter = ContentTypes.JsonContentType;

        public static byte[] Convert(byte[] incomingBody, Type bodyType)
        {
            var objectSerializer = new ObjectSerializer();
            var @object = objectSerializer.Deserialize(incomingBody);
            var convertedBody = SerializeToXml(bodyType, @object);
            return convertedBody;
        }

        private static byte[] SerializeToXml(Type type, object @object)
        {
            var stringWriter = new StringWriter();
            var writer = XmlWriter.Create(stringWriter);
            var xmlSerializer = new XmlSerializer(type);
            xmlSerializer.Serialize(writer, @object);
            var xml = stringWriter.ToString();
            return Encoding.Unicode.GetBytes(xml);
        }
    }
}