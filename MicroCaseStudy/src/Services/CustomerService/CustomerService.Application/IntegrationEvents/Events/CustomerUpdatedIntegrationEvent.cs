using EventBus.Base.Events;

namespace CustomerService.Application.IntegrationEvents.Events;

public class CustomerUpdatedIntegrationEvent: IntegrationEvent
{
    public int? CustomerId { get; set; }
    public int UserId { get; set; }
    public string? CustomerName { get; set; }

    public string? CustomerSurname { get; set; }

    public string? CustomerPhone { get; set; }

    public string? CustomerEmail { get; set; }
}