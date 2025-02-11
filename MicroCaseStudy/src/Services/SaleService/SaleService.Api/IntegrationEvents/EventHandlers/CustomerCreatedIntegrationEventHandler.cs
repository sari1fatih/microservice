using EventBus.Base.Abstraction;
using MediatR;
using SaleService.Api.IntegrationEvents.Events;
using SaleService.Application.Features.Sales.Commands.UpdateSaleCustomer;

namespace SaleService.Api.IntegrationEvents.EventHandlers;

public class CustomerCreatedIntegrationEventHandler : IIntegrationEventHandler<CustomerUpdatedIntegrationEvent>
{
    private readonly IMediator mediator;
    private readonly ILogger<CustomerCreatedIntegrationEventHandler> logger;

    public CustomerCreatedIntegrationEventHandler(IMediator mediator, ILogger<CustomerCreatedIntegrationEventHandler> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    public async Task Handle(CustomerUpdatedIntegrationEvent @event)
    {
        try
        {
            logger.LogInformation("Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                @event.Id,
                typeof(Program).Namespace,
                @event);

            var createSaleCommand = new UpdateSaleCustomerCommand();
         
            createSaleCommand.CustomerId = @event.CustomerId;
            createSaleCommand.UserId = @event.UserId;
            createSaleCommand.CustomerName = @event.CustomerName;
            createSaleCommand.CustomerSurname = @event.CustomerSurname;
            createSaleCommand.CustomerPhone = @event.CustomerPhone;
            createSaleCommand.CustomerEmail = @event.CustomerEmail; 
                
                 
            await mediator.Send(createSaleCommand);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.ToString());
        }
    }
}