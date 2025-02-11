using Core.Hangfire.Settings;
using Hangfire;

namespace Core.Hangfire.Managers;

public class HangfireBackgroundJobManager: IHangfireBackgroundJobManager
{
    private IServiceProvider _serviceProvider;

    public HangfireBackgroundJobManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void RegisterJob(IBackGroundJobWorker service)
    {
        try
        {
            service.Perform(new CancellationToken());

            RecurringJob.AddOrUpdate(service.JobId, () => service.Perform(new CancellationToken()),
                service.CronExp, queue: service.QueueName);
        }
        catch (Exception ex)
        {
        }
    }
}