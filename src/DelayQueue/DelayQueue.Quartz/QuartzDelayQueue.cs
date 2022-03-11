using DelayQueue.Shared;
using Newtonsoft.Json;
using Quartz;

namespace DelayQueue.Quartz
{
    public class QuartzDelayQueue : IDelayQueue
    {
        public const string JobGroup = "JobGroup";
        public const string JobDelegate = "JobDelegate";
        public const string JobParameters = "JobParameters";

        private readonly IScheduler _scheduler;
        public QuartzDelayQueue(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public async Task PutJob(TimeSpan delay, Action callback)
        {
            var jobDetail = JobBuilder.Create<DelayJob>()
                .WithIdentity(Guid.NewGuid().ToString("N"), JobGroup)
                .Build();

            jobDetail.JobDataMap[JobDelegate] = callback;

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{jobDetail.Key.Name}Trigger", JobGroup)
                .ForJob(jobDetail.Key)
                .StartAt(DateTimeOffset.UtcNow.Add(delay))
                .WithSimpleSchedule(x => x
                    .WithRepeatCount(0)
                    .WithIntervalInSeconds(0)
                )
                .Build();

            await _scheduler.ScheduleJob(jobDetail, trigger);
        }

        public async Task PutJob<T>(TimeSpan delay, T jobData, Action<T> callback)
        {
            var jobDetail = JobBuilder.Create<DelayJob<T>>()
                .WithIdentity(Guid.NewGuid().ToString("N"), JobGroup)
                .UsingJobData(JobParameters, JsonConvert.SerializeObject(jobData))
                .Build();

            jobDetail.JobDataMap[JobDelegate] = callback;

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{jobDetail.Key.Name}Trigger", JobGroup)
                .ForJob(jobDetail.Key)
                .StartAt(DateTimeOffset.UtcNow.Add(delay))
                .WithSimpleSchedule(x => x
                    .WithRepeatCount(0)
                    .WithIntervalInSeconds(0)
                )
                .Build();

            await _scheduler.ScheduleJob(jobDetail, trigger);
        }
    }
}

