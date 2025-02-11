namespace Core.Hangfire.Settings;

public interface IBackGroundJobWorker
{
    string JobId { get; }
    string CronExp { get; }
    string QueueName { get; }

    Task Perform(CancellationToken cancellationToken);
}