using Microsoft.Extensions.Logging;
using Quartz;

namespace JobScheduling.Core;

public class JobHistoryUpdater : IJobListener
{
    private readonly ILogger<JobHistoryUpdater> _logger;

    public JobHistoryUpdater(ILogger<JobHistoryUpdater> logger)
	{
        _logger = logger;
	}

    public string Name => "JobSchedulingListener";

    public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        var group = context.JobDetail.Key.Group;
        var name = context.JobDetail.Key.Name;

        _logger.LogDebug($"Job {group}:{name} is in-progress.");

        return Task.CompletedTask;
    }

    public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
    {
        var group = context.JobDetail.Key.Group;
        var name = context.JobDetail.Key.Name;

        _logger.LogDebug($"Job {group}:{name} is completed.");

        return Task.CompletedTask;
    }
}
