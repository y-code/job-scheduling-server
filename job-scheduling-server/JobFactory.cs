﻿using Quartz;
using Quartz.Spi;

internal class JobFactory : IJobFactory
{
    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        throw new NotImplementedException();
    }

    public void ReturnJob(IJob job)
    {
        throw new NotImplementedException();
    }
}
