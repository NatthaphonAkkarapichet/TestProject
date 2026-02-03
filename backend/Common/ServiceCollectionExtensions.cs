using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Common;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFeatureServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var servicesToRegister = assembly.GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.Name.EndsWith("Service"));

        foreach (var type in servicesToRegister)
        {
            services.AddScoped(type);
        }

        return services;
    }
}
