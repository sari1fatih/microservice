using Core.Hangfire.Settings;

namespace Core.Hangfire.Managers;

public interface IHangfireBackgroundJobManager
{
    public void RegisterJob(IBackGroundJobWorker service);
}