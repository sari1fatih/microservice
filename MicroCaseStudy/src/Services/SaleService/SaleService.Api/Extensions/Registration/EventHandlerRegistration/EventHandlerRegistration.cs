using SaleService.Api.IntegrationEvents.EventHandlers;

namespace SaleService.Api.Extensions.Registration.EventHandlerRegistration;

public static class EventHandlerRegistration
{
    public static IServiceCollection AddConfigureEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<SaleCreatedIntegrationEventHandler>();
        services.AddScoped<CustomerCreatedIntegrationEventHandler>();

        return services;
    }
}