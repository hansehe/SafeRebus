namespace SafeRebus.Adapters.Abstractions
{
    public interface IBodyConverter
    {
        byte[] Convert(byte[] incomingBody, string contentType);
    }
}