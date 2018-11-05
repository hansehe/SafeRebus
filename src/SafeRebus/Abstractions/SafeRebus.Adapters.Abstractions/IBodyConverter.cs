namespace SafeRebus.Adapters.Abstractions
{
    public interface IBodyConverter
    {
        bool TryConvert(byte[] incomingBody, string contentType, out byte[] convertedBody);
    }
}