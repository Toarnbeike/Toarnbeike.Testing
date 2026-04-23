using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Toarnbeike.Testing.Logging;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddTestLoggerProvider(this IServiceCollection services)
    {
        var provider = new TestLoggerProvider();

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.Services.AddSingleton<ILoggerProvider>(provider);
        });
        return services;
    }

    public static TestLoggerProvider GetTestLoggerProvider(this IServiceProvider provider)
    {
        var loggerProvider = provider.GetService<ILoggerProvider>() as TestLoggerProvider;
        return loggerProvider ?? throw new InvalidOperationException("TestLoggerProvider is not registered in the service collection.");
    }
}