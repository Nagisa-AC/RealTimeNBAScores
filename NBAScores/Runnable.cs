using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace NBAScores
{
    public class Runnable
    {
        private readonly ILogger _logger;

        public Runnable(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Runnable>();
        }

        [Function("Function1")]
        public async Task Run([TimerTrigger("*/10 * * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            // NBA API call
            var client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://api-nba-v1.p.rapidapi.com/games?date=2024-04-03"),
                Headers =
                {
                    { "X-RapidAPI-Key", Environment.GetEnvironmentVariable("RapidAPIKey") },
                    { "X-RapidAPI-Host", "api-nba-v1.p.rapidapi.com" },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine(body);
            }

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
