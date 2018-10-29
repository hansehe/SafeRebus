﻿using Microsoft.Extensions.DependencyInjection;
using SafeRebus.Abstractions;
using SafeRebus.Database.Repositories;

namespace SafeRebus.Database
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IDbProvider, DbProvider>()
                .AddScoped<IOutboxRepository, OutboxRepository>()
                .AddScoped<IResponseRepository, ResponseRepository>();
        }
    }
}