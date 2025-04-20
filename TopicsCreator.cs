using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ResourceCreatorFunction
{
    public class TopicsCreator
    {
        private readonly ILogger _logger;

        public TopicsCreator(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TopicsCreator>();
        }

        [Function("TopicsCreator")]
        public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
