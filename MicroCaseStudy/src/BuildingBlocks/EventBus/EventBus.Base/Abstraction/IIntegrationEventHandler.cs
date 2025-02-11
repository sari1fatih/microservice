using EventBus.Base.Events;

namespace EventBus.Base.Abstraction;

public interface IIntegrationEventHandler<TIntegrationEvent> : IntegrationEventHandler
{
    Task Handle(TIntegrationEvent @event);
}

public interface IntegrationEventHandler
{
}