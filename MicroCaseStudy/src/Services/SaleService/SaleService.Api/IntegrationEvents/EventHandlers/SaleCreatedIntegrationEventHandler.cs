using EventBus.Base.Abstraction;
using MediatR;
using SaleService.Api.IntegrationEvents.Events;
using SaleService.Application.Features.Sales.Commands;
using SaleService.Application.Features.Sales.Commands.CreateSale;

namespace SaleService.Api.IntegrationEvents.EventHandlers
{
    public class SaleCreatedIntegrationEventHandler : IIntegrationEventHandler<SaleCreatedIntegrationEvent>
    {
        private readonly IMediator mediator;
        private readonly ILogger<SaleCreatedIntegrationEventHandler> logger;

        public SaleCreatedIntegrationEventHandler(IMediator mediator, ILogger<SaleCreatedIntegrationEventHandler> logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        public async Task Handle(SaleCreatedIntegrationEvent @event)
        {
            try
            {
                logger.LogInformation("Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                @event.Id,
                typeof(Program).Namespace,
                @event);

                var createSaleCommand = new CreateSaleCommand();
              
                createSaleCommand.CustomerId = @event.CustomerId;
                createSaleCommand.CustomerName = @event.CustomerName;
                createSaleCommand.CustomerSurname = @event.CustomerSurname;
                createSaleCommand.CustomerPhone = @event.CustomerPhone;
                createSaleCommand.CustomerEmail = @event.CustomerEmail;
                createSaleCommand.SaleName = @event.SaleName;
                createSaleCommand.CreatedBy = @event.CreatedBy;
                createSaleCommand.Note = @event.Note;
                
                
                createSaleCommand.SaleName = @event.SaleName;
                await mediator.Send(createSaleCommand);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }
        }
    }
}
