using EventBus.Base.Events;

namespace SaleService.Api.IntegrationEvents.Events
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

        public SaleCreatedIntegrationEvent(int? customerId, string? customerName, string? customerSurname, string? customerPhone, string? customerEmail, string? saleName, string? note)
        {
            CustomerId = customerId;
            CustomerName = customerName;
            CustomerSurname = customerSurname;
            CustomerPhone = customerPhone;
            CustomerEmail = customerEmail;
            SaleName = saleName;
            Note = note;
        }
    }
}
