using Rebus.Logging;

namespace SafeRebus.Abstractions
{
    public interface IRabbitMqUtility
    {
        void WaitForAvailableRabbitMq();

        void PurgeInputQueue();
        
        void DeleteInputQueue();
        
        void CreateQueue(string queue);
        
        string ConnectionString { get; }
        
        string InputQueue { get; }
        
        string OutputQueue { get; }
        
        LogLevel LogLevel { get; }
    }
}