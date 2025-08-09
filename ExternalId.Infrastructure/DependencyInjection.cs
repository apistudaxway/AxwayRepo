using ExternalId.Application.Abstractions;
using ExternalId.Infrastructure.Graph;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExternalId.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GraphSettings>(configuration.GetSection("AzureAd"));
        services.AddScoped<IExternalUserService, ExternalUserService>();
        return services;
    }
}