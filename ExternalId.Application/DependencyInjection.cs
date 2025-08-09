using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ExternalId.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(typeof(AssemblyMarker).Assembly);
        return services;
    }
}