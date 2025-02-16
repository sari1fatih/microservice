using System.Net.Sockets;
using System.Text;
using EventBus.Base;
using EventBus.Base.Events;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EventBus.RabbitMQ;

public class EventBusRabbitMQ : BaseEventBus
{
    private RabbitMQPersistanceConnection _persistanceConnection;
    private readonly IConnectionFactory connectionFactory;

    private readonly IModel consumerChannel;

    public EventBusRabbitMQ(IServiceProvider serviceProvider, EventBusConfig eventBusConfig) : base(serviceProvider,
        eventBusConfig)
    {
        if (eventBusConfig.Connection != null)
        {
            if (EventBusConfig.Connection is ConnectionFactory)
                connectionFactory = EventBusConfig.Connection as ConnectionFactory;
            else
            {
                var connJson = JsonConvert.SerializeObject(EventBusConfig.Connection, new JsonSerializerSettings()
                {
                    // Self referencing loop detected for property 
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                connectionFactory = JsonConvert.DeserializeObject<ConnectionFactory>(connJson);
            }
        }
        else
        {
            connectionFactory = new ConnectionFactory();
        }

        _persistanceConnection =
            new RabbitMQPersistanceConnection(connectionFactory, eventBusConfig.ConnectionRetryCount);

        consumerChannel = CreateConsumerChannel();

        SubsManager.OnEventRemoved += SubsManager_OnEventRemoved;
    }


    #region Overrides of BaseEventBus

    public override void Publish(IntegrationEvent @event)
    {
        if (!_persistanceConnection.IsConnected)
        {
            _persistanceConnection.TryConnect();
        }

        var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(EventBusConfig.ConnectionRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                    retryAttempt)), (ex, time) =>
                {
                    // log
                }
            );

        var eventName = @event.GetType().Name;

        eventName = ProcessEventName(eventName);

        consumerChannel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName, type: "direct");

        var message = JsonConvert.SerializeObject(@event);
        var body = Encoding.UTF8.GetBytes(message);

        policy.Execute(() =>
        {
            var properties = consumerChannel.CreateBasicProperties();
            properties.DeliveryMode = 2; // persistent
            // Test için
            // Publish metodu içerisinde mesajı ilgili exchange e göndermemiz gerekiyor
            // Exchange ye göndermemiş gerekiyor
            /*
            consumerChannel.QueueDeclare(queue: GetSubName(eventName), // Ensure queue exists while publishing
                  durable: true,
                  exclusive: false,
                  autoDelete: false,
                  arguments: null
            );

            consumerChannel.QueueBind(queue: GetSubName(eventName),
                  exchange: EventBusConfig.DefaultTopicName,
                  eventName);
              */
            consumerChannel.BasicPublish(
                exchange: EventBusConfig.DefaultTopicName,
                routingKey: eventName,
                mandatory: true,
                basicProperties: properties,
                body: body
            );
        });
    }

    public override void Subscribe<T, TH>()
    {
        var eventName = typeof(T).Name;
        eventName = ProcessEventName(eventName);

        if (!SubsManager.HasSubscriptionsForEvent(eventName))
        {
            if (!_persistanceConnection.IsConnected)
            {
                _persistanceConnection.TryConnect();
            }

            consumerChannel.QueueDeclare(queue: GetSubName(eventName),
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            consumerChannel.QueueBind(queue: GetSubName(eventName),
                exchange: EventBusConfig.DefaultTopicName,
                eventName);
        }

        SubsManager.AddSubscription<T, TH>();
        StartBasicConsume(eventName);
    }

    public override void Unsubscribe<T, TH>()
    {
        SubsManager.RemoveSubscription<T, TH>();
    }

    #endregion

    private IModel CreateConsumerChannel()
    {
        if (!_persistanceConnection.IsConnected)
        {
            _persistanceConnection.TryConnect();
        }

        var channel = _persistanceConnection.CreateModel();

        channel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName, type: "direct");

        return channel;
    }

    private void SubsManager_OnEventRemoved(object? sender, string eventName)
    {
        eventName = ProcessEventName(eventName);

        if (!_persistanceConnection.IsConnected)
        {
            _persistanceConnection.TryConnect();
        }

        consumerChannel.QueueUnbind(queue: eventName,
            exchange: EventBusConfig.DefaultTopicName,
            routingKey: eventName);

        if (SubsManager.IsEmpty)
        {
            consumerChannel.Close();
        }
    }

    private void StartBasicConsume(string eventName)
    {
        if (consumerChannel != null)
        {
            var consumer = new EventingBasicConsumer(consumerChannel);

            consumer.Received += Consumer_Received;

            consumerChannel.BasicConsume(queue: GetSubName(eventName), autoAck: false, consumer: consumer);
        }
    }

    private async void Consumer_Received(object? sender, BasicDeliverEventArgs e)
    {
        var eventName = e.RoutingKey;
        eventName = ProcessEventName(eventName);
        var message = Encoding.UTF8.GetString(e.Body.Span);

        try
        {
            await ProcessEvent(eventName, message);
        }
        catch (Exception exception)
        {
            // logging
        }

        consumerChannel.BasicAck(e.DeliveryTag, false);
    }
}