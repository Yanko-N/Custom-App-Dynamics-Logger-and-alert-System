using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;

namespace Persistence.Extensions
{
    public static class PersistenceExtension
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<ILogsRepository, LogsRepository>();
            services.AddTransient<IServiceRepository, ServiceRepository>();
            services.AddTransient<IApiKeyRepository, ApiKeyRepository>();
            services.AddTransient<IHookRepository, HookRepository>();
            services.AddTransient<IAlertRepository, AlertRepository>();

            return services;
        }
    }
}
