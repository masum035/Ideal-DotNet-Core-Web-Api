using Contracts;
using LoggerService;

namespace IdealDotnetApi.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services) => services.AddCors(options =>
    {
        options.AddPolicy(name: "CorsPolicy",
            configurePolicy: builder => builder.AllowAnyOrigin(). // WithOrigins("https://example.com")
                AllowAnyMethod(). // WithMethods("POST", "GET")
                AllowAnyHeader()); // WithHeaders("accept", "content-type")
    });

    public static void ConfigureIISIntegration(this IServiceCollection services) =>
        services.Configure<IISOptions>(options => { });

    public static void ConfigureLoggerService(this IServiceCollection services) =>
        services.AddSingleton<ILoggerManager, LoggerManager>();
}