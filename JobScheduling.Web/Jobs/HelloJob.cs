using System;
using Quartz;

namespace JobScheduling.Web.Jobs
{
	public class HelloJob : IJob
	{
		public HelloJob()
		{
		}

        public Task Execute(IJobExecutionContext context)
        {
			return Task.CompletedTask;
        }
    }
}

