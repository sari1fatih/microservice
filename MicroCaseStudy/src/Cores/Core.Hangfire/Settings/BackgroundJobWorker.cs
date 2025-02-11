namespace Core.Hangfire.Settings;

public abstract class BackgroundJobWorker : IBackGroundJobWorker
{
    public string JobId { get; private set; }
    public string CronExp { get; private set; }
    public string QueueName { get; private set; }

    public BackgroundJobWorker(string jobId, string cronExp, string queueName = "default")
    {
        JobId = jobId;
        CronExp = cronExp;
        QueueName = queueName;
    }

    public abstract Task Perform(CancellationToken cancellationToken);
}