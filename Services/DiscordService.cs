using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TrackFlow.Services
{
    public class DiscordService
    {
        
        public string captainHookUrl = "INSERT URL HERE";

        private readonly HttpClient _httpClient;

        public DiscordService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendViolationMessageAsync(string webhookUrl, string message)
        {
            var payload = new
            {
                content = message
            };

            var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(webhookUrl, content);

            // Optionally, you can handle the response here if needed
            if (!response.IsSuccessStatusCode)
            {
                // Handle error
            }
        }
    }
}
