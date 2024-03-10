using System.Threading.Channels;
using JobScheduling.Domain;
using JobScheduling.Web.Jobs;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Impl.Matchers;

namespace JobScheduling.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobScheduleController : ControllerBase
{
    const string schedulerName = "JobScheduling";

    private readonly ILogger<JobScheduleController> _logger;
    private readonly ISchedulerFactory _schedulerFactory;

    public JobScheduleController(ILogger<JobScheduleController> logger, ISchedulerFactory schedulerFactory)
    {
        _logger = logger;
        _schedulerFactory = schedulerFactory;
    }

    private async Task<IScheduler> GetSchedulerAsync(CancellationToken cancel)
    {
        var scheduler = await _schedulerFactory.GetScheduler(schedulerName, cancel);
        if (scheduler is null)
            throw new InvalidOperationException($"No scheduler instance was found for name \"{schedulerName}\"");
        return scheduler;
    }

    [HttpGet]
    public async Task<IEnumerable<JobScheduleModel>> Get(CancellationToken cancel)
    {
        var scheduler = await GetSchedulerAsync(cancel);

        var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
        var result = new List<(IJobDetail job, ITrigger trigger)>();
        foreach (var jobKey in jobKeys)
        {
            var jobDetail = await scheduler.GetJobDetail(jobKey);
            if (jobDetail is null)
                continue;

            var triggers = await scheduler.GetTriggersOfJob(jobKey);
            foreach (var trigger in triggers)
            {
                var next = trigger.GetNextFireTimeUtc();
                result.Add((jobDetail, trigger));
            }
        }

        return result.GroupBy(x => x.job.Key.Group).Select(x =>
        {
            var a = x.OrderBy(y => y.trigger.FinalFireTimeUtc).ToArray();
            return new JobScheduleModel
            {
                Start = a.First().trigger.FinalFireTimeUtc,
                End = a.Last().trigger.FinalFireTimeUtc,
                Description = x.First().job.Description,
            };
        });
    }

    [HttpPut]
    public async Task Create([FromBody] JobScheduleModel schedule, CancellationToken cancel)
    {
        if (schedule.Start is null)
            throw new ArgumentException($"Schedule Start date/time is required.", nameof(JobScheduleModel.Start));
        if (schedule.End is null)
            throw new ArgumentException($"Schedule End date/time is required.", nameof(JobScheduleModel.End));

        var scheduler = await GetSchedulerAsync(cancel);

        var group = Guid.NewGuid().ToString();

        var jobDetail1 = JobBuilder.Create<HelloJob>()
            .WithIdentity("START", group)
            .WithDescription(schedule.Description)
            .Build();

        var trigger1 = TriggerBuilder.Create()
            .StartAt(schedule.Start.Value)
            .Build();

        await scheduler.ScheduleJob(jobDetail1, trigger1);

        var jobDetail2 = JobBuilder.Create<HelloJob>()
            .WithIdentity("END", group)
            .WithDescription(schedule.Description)
            .Build();

        var trigger2 = TriggerBuilder.Create()
            .StartAt(schedule.End.Value)
            .Build();

        await scheduler.ScheduleJob(jobDetail2, trigger2);
    }
}
