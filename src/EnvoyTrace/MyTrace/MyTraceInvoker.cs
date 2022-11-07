using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrace
{
    public class MyTraceInvoker
    {
        private readonly ILogger<MyTraceInvoker> _logger;
        private readonly ActivitySource _activitySource;

        public MyTraceInvoker(ActivitySource activitySource, ILogger<MyTraceInvoker> logger)
        {
            _logger = logger;
            _activitySource = activitySource;
        }

        public async Task<string> InvokeAsync()
        {
            using (_activitySource.StartActivity())
            {
                return await RunStep1Async();
            }
        }

        private async Task<string> RunStep1Async()
        {
            using (var activity = _activitySource.StartActivity())
            {
                await Task.Delay(100);
                activity.AddTag("Description", "This is Step1");
                _logger.LogInformation("Step1 is running");
                return await RunStep2Async();
            }
        }

        private async Task<string> RunStep2Async()
        {
            using (var activity = _activitySource.StartActivity())
            {
                await Task.Delay(200);
                activity.AddTag("Description", "This is Step2");
                _logger.LogInformation("Step2 is running");
                return await RunStep3Async();
            }
        }

        private async Task<string> RunStep3Async()
        {
            using (var activity = _activitySource.StartActivity())
            {
                await Task.Delay(300);
                activity.AddTag("Description", "This is Step3");
                _logger.LogInformation("Step3 is running");
                return "Hi, I am always here";
            }
        }
    }
}
