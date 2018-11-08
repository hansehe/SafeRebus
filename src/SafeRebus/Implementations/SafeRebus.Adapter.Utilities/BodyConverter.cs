using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using SafeRebus.Adapter.Utilities.Converters;
using SafeRebus.Adapters.Abstractions;

namespace SafeRebus.Adapter.Utilities
{
    public class BodyConverter : IBodyConverter
    {
        private readonly ILogger Logger;

        public BodyConverter(ILogger<BodyConverter> logger)
        {
            Logger = logger;
        }
        
        public bool TryConvert(byte[] incomingBody, string contentType, string messageType, out byte[] convertedBody)
        {
            convertedBody = new byte[0];
            if (!TryParseMessageType(messageType, out var bodyType))
            {
                return false;
            }
            switch (contentType)
            {
                case XmlToJsonConverter.ContentTypeConverter:
                    return TryConvert(() => XmlToJsonConverter.Convert(incomingBody, bodyType), out convertedBody);
                case JsonToXmlConverter.ContentTypeConverter:
                    return TryConvert(() => JsonToXmlConverter.Convert(incomingBody, bodyType), out convertedBody);
                default:
                    return false;
            }
        }

        private static bool TryParseMessageType(string messageType, out Type bodyType)
        {
            if (messageType.Contains(","))
            {
                messageType = messageType.Substring(0, messageType.IndexOf(','));
            }

            bodyType = null;
            var @namespace = messageType.Substring(0, messageType.LastIndexOf('.'));
            var typeName = messageType.Substring(messageType.LastIndexOf('.') + 1);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var fullName = assembly.GetName().Name;
                if (@namespace.Contains(fullName))
                {
                    bodyType = assembly.GetTypes().FirstOrDefault(x => x.Name == typeName);
                    if (bodyType != default(Type))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool TryConvert(Func<byte[]> converter, out byte[] convertedBody)
        {
            try
            {
                convertedBody = converter.Invoke();
                return true;
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message);
            }
            convertedBody = new byte[0];
            return false;
        }
    }
}