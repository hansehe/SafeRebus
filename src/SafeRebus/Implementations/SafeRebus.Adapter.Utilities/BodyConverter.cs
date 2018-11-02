using System;
using System.Collections.Generic;
using SafeRebus.Adapter.Utilities.Converters;
using SafeRebus.Adapters.Abstractions;

namespace SafeRebus.Adapter.Utilities
{
    public class BodyConverter : IBodyConverter
    {
        public byte[] Convert(byte[] incomingBody, string contentType)
        {
            switch (contentType)
            {
                case XmlConverter.ContentTypeConverter:
                    return XmlConverter.Convert(incomingBody);
                default:
                    return incomingBody;
            }
        }
    }
}