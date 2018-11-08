namespace SafeRebus.Adapters.Abstractions
{
    public interface IBodyConverter
    {
        bool TryConvert(byte[] incomingBody, string contentType, string messageType, out byte[] convertedBody);
    }
}