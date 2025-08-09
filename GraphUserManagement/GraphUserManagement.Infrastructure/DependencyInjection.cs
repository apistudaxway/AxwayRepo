using Azure.Identity;
using GraphUserManagement.Domain.Interfaces;
using GraphUserManagement.Infrastructure.Configuration;
using GraphUserManagement.Infrastructure.Repositories;
using GraphUserManagement.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;

namespace GraphUserManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind configuration
        services.Configure<GraphApiSettings>(configuration.GetSection(GraphApiSettings.SectionName));
        var graphApiSettings = configuration.GetSection(GraphApiSettings.SectionName).Get<GraphApiSettings>() ?? new GraphApiSettings();

        // Register Graph Service Client
        services.AddSingleton<GraphServiceClient>(serviceProvider =>
        {
            var credentials = new ClientSecretCredential(
                graphApiSettings.TenantId,
                graphApiSettings.ClientId,
                graphApiSettings.ClientSecret);

            var graphServiceClient = new GraphServiceClient(credentials);
            return graphServiceClient;
        });

        // Register services
        services.AddScoped<IGraphApiService, GraphApiService>();
        services.AddScoped<IUserRepository, GraphUserRepository>();

        return services;
    }
}