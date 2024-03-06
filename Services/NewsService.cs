using System;
using System.Net.Http;

using System.Threading.Tasks;

public interface INewsService
{
    Task<Article[]> GetNewsAsync(string category);
}

public class NewsService : INewsService
{
    private readonly HttpClient _httpClient;

    public NewsService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<Article[]> GetNewsAsync(string category)
    {
        var primaryApiKey = "ce55466b778e4164ba3071bcf6f48927";
        var backupApiKey = "bbc1811d3fb14e6aa1b9e2b297a1c498";

        var apiUrl = $"https://newsapi.org/v2/top-headlines?country=us&apiKey={primaryApiKey}&category={category}&pageSize=4";

        try
        {
            var response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var articles = await response.Content.ReadFromJsonAsync<Article[]>();
            return articles;
        }
        catch (HttpRequestException)
        {
            // Retry with backup API key
            apiUrl = $"https://newsapi.org/v2/top-headlines?country=us&apiKey={backupApiKey}&category={category}&pageSize=4";

            var response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var articles = await response.Content.ReadFromJsonAsync<Article[]>();
            return articles;
        }
    }
}

public class Article
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public string UrlToImage { get; set; }
}
