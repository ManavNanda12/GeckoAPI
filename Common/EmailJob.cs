using StackExchange.Redis;


namespace GeckoAPI.Common
{
    public class EmailJob
    {
        private readonly IHttpClientFactory _httpClientFactory;
        //private readonly IConnectionMultiplexer _redis;

        //, IConnectionMultiplexer redis
        public EmailJob(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            //_redis = redis;
        }

        public async Task SendWelcomeEmailsViaApi()
        {
            // Fix: Use the correct method to create an HttpClient instance
            var client = _httpClientFactory.CreateClient(); // Correct method is CreateClient
            client.BaseAddress = new Uri("https://localhost:44300");

            var response = await client.GetAsync("/api/customers/send-welcome-email");
            response.EnsureSuccessStatusCode();
        }

        public async Task SendMonthlySalesReportViaApi()
        {
            // Fix: Use the correct method to create an HttpClient instance
            var client = _httpClientFactory.CreateClient(); // Correct method is CreateClient
            client.BaseAddress = new Uri("https://localhost:44300");

            var response = await client.GetAsync("/api/dashboard/send-monthly-reports");
            response.EnsureSuccessStatusCode();
        }

        //public async Task ClearAllCache()
        //{
        //    var endpoints = _redis.GetEndPoints();
        //    var db = _redis.GetDatabase();

        //    foreach (var endpoint in endpoints)
        //    {
        //        var server = _redis.GetServer(endpoint);

        //        var keys = server.Keys(pattern: "products_*");

        //        foreach (var key in keys)
        //        {
        //            await db.KeyDeleteAsync(key);
        //        }
        //    }

        //    Console.WriteLine("Only product cache cleared at " + DateTime.Now);
        //}
    }
}
