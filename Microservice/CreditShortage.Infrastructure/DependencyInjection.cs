using CreditShortage.Application.Interfaces;
using CreditShortage.Infrastructure.Data;
using CreditShortage.Infrastructure.ExternalServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace CreditShortage.Infrastructure;

/// <summary>
/// Infrastructure layer dependency injection configuration
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register repositories
        services.AddScoped<IShortageValidationRepository, ShortageValidationRepository>();

        // Register HTTP clients with resilience (retry + circuit breaker)
        var retryCount = int.Parse(configuration["IWSIntegration:RetryCount"] ?? "3");
        var timeoutSeconds = int.Parse(configuration["IWSIntegration:Timeout"] ?? "30");

        services.AddHttpClient<IIWSIntegrationService, IWSIntegrationService>()
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = retryCount;
                options.Retry.BackoffType = Polly.DelayBackoffType.Exponential;
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            });

        return services;
    }
}
