using SafeRebus.Adapter.Utilities.Converters;
using SafeRebus.Adapters.Abstractions;

namespace SafeRebus.Adapter.Utilities
{
    public class BodyConverter : IBodyConverter
    {
        public bool TryConvert(byte[] incomingBody, string contentType, out byte[] convertedBody)
        {
            switch (contentType)
            {
                case XmlConverter.ContentTypeConverter:
                    return XmlConverter.TryConvert(incomingBody, out convertedBody);
                default:
                    convertedBody = new byte[0];
                    return false;
            }
        }
    }
}