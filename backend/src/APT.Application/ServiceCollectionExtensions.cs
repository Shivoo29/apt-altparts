
// backend/src/APT.Application/ServiceCollectionExtensions.cs
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace APT.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        return services;
    }
}
