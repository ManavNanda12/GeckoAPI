namespace GeckoAPI.Common
{
    public class EmailJob
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EmailJob(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task SendWelcomeEmailsViaApi()
        {
            // Fix: Use the correct method to create an HttpClient instance
            var client = _httpClientFactory.CreateClient(); // Correct method is CreateClient
            client.BaseAddress = new Uri("https://localhost:44300");

            var response = await client.GetAsync("/api/customers/send-welcome-email");
            response.EnsureSuccessStatusCode();
        }
    }
}
