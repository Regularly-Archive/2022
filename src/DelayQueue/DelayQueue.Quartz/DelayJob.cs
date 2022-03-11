using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelayQueue.Quartz
{
    internal class DelayJob : IJob
    {

        public Task Execute(IJobExecutionContext context)
        {
            var jobDetail = context.JobDetail;

            var callback = jobDetail.JobDataMap[QuartzDelayQueue.JobDelegate] as Action;

            callback?.Invoke();

            return Task.CompletedTask;
        }
    }

    internal class DelayJob<T> : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var jobDetail = context.JobDetail;
            var callback = jobDetail.JobDataMap[
                QuartzDelayQueue.JobDelegate] as Action<T>;

            var jobData = context.MergedJobDataMap[QuartzDelayQueue.JobParameters]?.ToString();

            var jobParam = JsonConvert.DeserializeObject<T>(jobData);

            callback?.Invoke(jobParam);
          
            return Task.CompletedTask;
        }
    }

}
