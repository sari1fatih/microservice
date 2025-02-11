using EventBus.Base.Events;

namespace SaleService.Api.IntegrationEvents.Events;

public class CustomerUpdatedIntegrationEvent: IntegrationEvent
{
    public int? CustomerId { get; set; }
    public int UserId { get; set; }
    public string? CustomerName { get; set; }

    public string? CustomerSurname { get; set; }

    public string? CustomerPhone { get; set; }

    public string? CustomerEmail { get; set; }

    public CustomerUpdatedIntegrationEvent(int? customerId,int userId, string? customerName, string? customerSurname, string? 
            customerPhone, string? customerEmail)
    {
        
        CustomerId = customerId;
        UserId = userId;
        CustomerName = customerName;
        CustomerSurname = customerSurname;
        CustomerPhone = customerPhone;
        CustomerEmail = customerEmail;
    }
}