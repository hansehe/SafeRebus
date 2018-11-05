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
    public static class XmlConverter
    {
        public const string ContentTypeConverter = ContentTypes.NServiceBusContentType;

        public static bool TryConvert(byte[] incomingBody, out byte[] convertedBody)
        {
            var xmlString = Encoding.UTF8.GetString(incomingBody);
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlString);
            var namespaceAttributeValue = xmlDocument.DocumentElement.Attributes["xmlns"].Value;
            var typeName = xmlDocument.DocumentElement.Name;
            var typeNamespace = namespaceAttributeValue.Substring(namespaceAttributeValue.LastIndexOf('/') + 1);
            var @namespace = typeNamespace.Substring(0, typeNamespace.LastIndexOf('.'));
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var fullName = assembly.GetName().Name;
                if (fullName == @namespace)
                {
                    var bodyType = assembly.GetTypes().First(x => x.Name == typeName);
                    xmlDocument.RemoveUnknownAttributes();
                    var @object = DeserializeXml(bodyType, xmlDocument);
                    var objectSerializer = new ObjectSerializer();
                    convertedBody = objectSerializer.Serialize(@object);
                    return true;
                }
            }
            convertedBody = new byte[0];
            return false;
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
            xmlDocument.DocumentElement.Attributes["xmlns"].RemoveAll();
        }
    }
}