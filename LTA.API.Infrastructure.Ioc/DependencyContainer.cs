using LTA.API.Domain.Interfaces;
using LTA.API.Infrastructure.Data.Repositories;
using LTA.API.Infrastructure.Hubs.Interfaces;
using LTA.API.Infrastructure.Hubs.Services;
using LTA.API.Infrastructure.Loggers.Interfaces;
using LTA.API.Infrastructure.Loggers.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LTA.API.Infrastructure.Ioc;

public static class DependencyContainer
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<ILoggerService, LoggerService>();

        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITopicRepository, TopicRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        services.AddTransient<ITopicService, TopicService>();
        services.AddTransient<IIdentityService, IdentityService>();
    }
}