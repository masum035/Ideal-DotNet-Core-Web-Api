using AspNetCoreRateLimit;
using Contracts;
using LoggerService;
using Microsoft.EntityFrameworkCore;
using Repository;
using Service;
using Service.Contracts;

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

    public static void ConfigureRateLimitingOptions(this IServiceCollection services)
    {
        var rateLimitRules = new List<RateLimitRule>
        {
            new RateLimitRule
            {
                Endpoint = "*", 
                Limit = 1000, 
                Period = "1m"
            }
        }; 
        services.Configure<IpRateLimitOptions>(opt =>
        {
            opt.GeneralRules = rateLimitRules;
        }); 
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>(); 
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>(); 
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>(); 
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
    }
    
    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();
    
    public static void ConfigureServiceManager(this IServiceCollection services) =>
        services.AddScoped<IServiceManager, ServiceManager>();
    
    public static void ConfigureSqlContext(this IServiceCollection services,
        IConfiguration configuration) =>
        services.AddDbContext<RepositoryContext>(opts =>
            opts.UseSqlServer(configuration.GetConnectionString("sqlConnection")));
}