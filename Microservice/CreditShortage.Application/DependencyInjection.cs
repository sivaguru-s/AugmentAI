using CreditShortage.Application.Interfaces;
using CreditShortage.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CreditShortage.Application;

/// <summary>
/// Application layer dependency injection configuration
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register application services
        services.AddScoped<IShortageValidationService, ShortageValidationService>();
        services.AddScoped<IReferenceDataService, ReferenceDataService>();

        return services;
    }
}
