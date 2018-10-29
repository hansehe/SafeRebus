using Microsoft.Extensions.DependencyInjection;
using SafeRebus.MessageHandler.Abstractions;
using SafeRebus.MessageHandler.Database.Repositories;

namespace SafeRebus.MessageHandler.Database
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IResponseRepository, ResponseRepository>();
        }
    }
}