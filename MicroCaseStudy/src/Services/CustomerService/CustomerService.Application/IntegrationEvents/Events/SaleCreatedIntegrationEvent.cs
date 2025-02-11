using EventBus.Base.Events;

namespace CustomerService.Application.IntegrationEvents.Events
{
    public class SaleCreatedIntegrationEvent : IntegrationEvent
    {
        public int? CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerSurname { get; set; }

        public string? CustomerPhone { get; set; }

        public string? CustomerEmail { get; set; }

        public string? SaleName { get; set; }
        public int? CreatedBy { get; set; }
        public string? Note { get; set; }
  
    }
}
