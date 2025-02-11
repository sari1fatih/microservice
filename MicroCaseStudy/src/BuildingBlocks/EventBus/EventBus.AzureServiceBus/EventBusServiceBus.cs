using System.Text;
using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EventBus.AzureServiceBus;

public class EventBusServiceBus : BaseEventBus
{
    private ITopicClient _topicClient;
    private ManagementClient _managementClient;
    private ILogger logger;


    public EventBusServiceBus(IServiceProvider serviceProvider, EventBusConfig eventBusConfig) : base(serviceProvider,
        eventBusConfig)
    {
        logger = serviceProvider.GetService(typeof(ILogger<EventBusServiceBus>)) as ILogger<EventBusServiceBus>;

        _managementClient = new ManagementClient(eventBusConfig.EventBusConnectionString);

        _topicClient = createTopicClient();
    }

    private ITopicClient createTopicClient()
    {
        if (_topicClient == null || _topicClient.IsClosedOrClosing)
            _topicClient = new TopicClient(EventBusConfig.EventBusConnectionString, EventBusConfig.DefaultTopicName,
                RetryPolicy.Default);

        if (!_managementClient.TopicExistsAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult())
            _managementClient.CreateTopicAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult();

        return _topicClient;
    }

    #region Overrides of BaseEventBus

    public override void Publish(IntegrationEvent @event)
    {
        var eventName = @event.GetType().Name; //example : OrderCreatedIntegrationEvent

        eventName = ProcessEventName(eventName); // example: OrderCreated

        var eventStr = JsonConvert.SerializeObject(@event);

        var bodyArr = Encoding.UTF8.GetBytes(eventStr);

        var message = new Message()
        {
            MessageId = Guid.NewGuid().ToString(),
            Body = bodyArr,
            Label = eventName,
        };

        _topicClient.SendAsync(message).GetAwaiter().GetResult();
    }

    public override void Subscribe<T, TH>()
    {
        var eventName = typeof(T).Name;

        eventName = ProcessEventName(eventName); // example

        if (!SubsManager.HasSubscriptionsForEvent(eventName))
        {
            var subscriptionClient = CreateSubscriptionClientIfNotExists(eventName);
            RegisterSubscriptionClientMessageHandler(subscriptionClient);
        }

        logger.LogInformation($"Subscribing to {eventName} with {typeof(TH).Name}");
        SubsManager.AddSubscription<T, TH>();
    }


    public override void Unsubscribe<T, TH>()
    {
        var eventName = typeof(T).Name;

        try
        {
            var subscriptionClient = createSubscriptionClient(eventName);

            subscriptionClient
                .RemoveRuleAsync(eventName)
                .GetAwaiter()
                .GetResult();
        }
        catch (MessagingEntityNotFoundException)
        {
            logger.LogWarning("The messaging entity {eventName} could not found.", eventName);
        }

        logger.LogInformation($"Unsubscribing from {eventName}");

        SubsManager.RemoveSubscription<T, TH>();
    }

    private void RegisterSubscriptionClientMessageHandler(ISubscriptionClient subscriptionClient)
    {
        subscriptionClient.RegisterMessageHandler(
            async (message, token) =>
            {
                var eventName = $"{message.Label}";
                var messageData = Encoding.UTF8.GetString(message.Body);
                if (await ProcessEvent(ProcessEventName(eventName), messageData))
                {
                    await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                }
            },
            new MessageHandlerOptions(ExceptionReceivedHandler) { MaxConcurrentCalls = 10, AutoComplete = false }
        );
    }

    private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
    {
        var ex = exceptionReceivedEventArgs.Exception;

        var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
        logger.LogError(ex, "Error handling message: {ExceptionMessage} - Context {@ExceptionContext}", ex.Message,
            context);
        return Task.CompletedTask;
    }

    private ISubscriptionClient CreateSubscriptionClientIfNotExists(string eventName)
    {
        var subClient = createSubscriptionClient(eventName);

        var exist = _managementClient.SubscriptionExistsAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName))
            .GetAwaiter()
            .GetResult();

        if (!exist)
        {
            _managementClient.CreateSubscriptionAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName))
                .GetAwaiter()
                .GetResult();
            RemoveDefaultRule(subClient);
        }

        CreateRuleIfNotExists(ProcessEventName(eventName), subClient);

        return subClient;
    }

    private void CreateRuleIfNotExists(string eventName, ISubscriptionClient subscriptionClient)
    {
        bool ruleExists;

        try
        {
            var rule = _managementClient.GetRuleAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName), eventName)
                .GetAwaiter().GetResult();

            ruleExists = rule != null;
        }
        catch (MessagingEntityNotFoundException)
        {
            ruleExists = false;
        }

        if (!ruleExists)
        {
            subscriptionClient.AddRuleAsync(new RuleDescription
            {
                Filter = new CorrelationFilter { Label = eventName },
                Name = eventName,
            }).GetAwaiter().GetResult();
        }
    }

    private void RemoveDefaultRule(SubscriptionClient subscriptionClient)
    {
        try
        {
            subscriptionClient
                .RemoveRuleAsync(RuleDescription.DefaultRuleName)
                .GetAwaiter()
                .GetResult();
        }
        catch (Exception e)
        {
            logger.LogWarning("The messaging entity (DefaultRuleName) Could not be found.",
                RuleDescription.DefaultRuleName);
        }
    }

    private SubscriptionClient createSubscriptionClient(string eventName)
    {
        return new SubscriptionClient(EventBusConfig.EventBusConnectionString, EventBusConfig.DefaultTopicName,
            GetSubName(eventName));
    }


    public override void Dispose()
    {
        base.Dispose();
        _topicClient.CloseAsync().GetAwaiter().GetResult();
        _managementClient.CloseAsync().GetAwaiter().GetResult();
        _topicClient = null;
        _managementClient = null;
    }

    #endregion
}