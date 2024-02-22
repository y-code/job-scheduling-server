using System.Collections.Specialized;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

var configBuilder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json");
var config = configBuilder.Build();

var properties = new NameValueCollection
{
    //["quartz.jobStore.tablePrefix"] = "quartz.qrtz_",
};

//IScheduler scheduler = await SchedulerBuilder.Create(properties)
//    .UseDefaultThreadPool(x => x.MaxConcurrency = 5)
//    .UsePersistentStore(x =>
//    {
//        var dbConnStr = config.GetConnectionString("JobScheduling");
//        if (dbConnStr is null)
//            throw new InvalidOperationException($"Connection string \"JobScheduling\" is not configured.");

//        x.UseProperties = true;
//        x.UsePostgres(dbConnStr);
//        x.UseNewtonsoftJsonSerializer();
//    })
//    .BuildScheduler();

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((builder, services) =>
    {
        //services.AddSingleton<IJobFactory, JobFactory>();
        services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
        //services.AddSingleton<QuartzHostedService>();

        // NOTE: `AddQuartzHostedService` has a problem where the class is registered as IHostedService using `AddSingleton`.
        // This breaks other hosted service registrations.
        services.Configure<QuartzHostedServiceOptions>(options =>
        {
            options.WaitForJobsToComplete = true;
        });
        services.AddHostedService<QuartzHostedService>();
    })
    .Build();
await host.RunAsync();

//await scheduler.Start();
