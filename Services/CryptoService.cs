using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public interface ICryptoService
{
    Task<string> GetBTCUSD();
    Task<string> GetETHUSD();
    Task<string> GetLTCUSD();
}

public class CryptoService : ICryptoService
{
    private string btcUsdRate;
    private string ethUsdRate;
    private string ltcUsdRate;
    private DateTime lastUpdated;
    private readonly HttpClient httpClient;

    public CryptoService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        lastUpdated = DateTime.MinValue;
    }

    public async Task<string> GetBTCUSD()
    {
        // Check if the rate is cached and not expired (e.g., cache for 1 hour)
        if (!string.IsNullOrEmpty(btcUsdRate) && (DateTime.UtcNow - lastUpdated).TotalHours < 1)
        {
            return btcUsdRate;
        }

        return await GetCryptoRate("BTC");
    }

    public async Task<string> GetETHUSD()
    {
        // Check if the rate is cached and not expired (e.g., cache for 1 hour)
        if (!string.IsNullOrEmpty(ethUsdRate) && (DateTime.UtcNow - lastUpdated).TotalHours < 1)
        {
            return ethUsdRate;
        }

        return await GetCryptoRate("ETH");
    }

    public async Task<string> GetLTCUSD()
    {
        // Check if the rate is cached and not expired (e.g., cache for 1 hour)
        if (!string.IsNullOrEmpty(ltcUsdRate) && (DateTime.UtcNow - lastUpdated).TotalHours < 1)
        {
            return ltcUsdRate;
        }

        return await GetCryptoRate("LTC");
    }

    private async Task<string> GetCryptoRate(string currency)
    {
        try
        {
            var response = await httpClient.GetAsync($"https://api.coinbase.com/v2/exchange-rates?currency={currency}");
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseBody);
            var rate = json["data"]["rates"]["USD"].ToObject<decimal>();
            var formattedRate = $"$ {Math.Round(rate, 1)}";
            UpdateCache(currency, formattedRate);
            return formattedRate;
        }

        catch (HttpRequestException e)
        {
            // Handle request exception
            Console.WriteLine($"Request error: {e.Message}");
            return null;
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

    private void UpdateCache(string currency, string rate)
    {
        switch (currency)
        {
            case "BTC":
                btcUsdRate = rate;
                break;
            case "ETH":
                ethUsdRate = rate;
                break;
            case "LTC":
                ltcUsdRate = rate;
                break;
        }
        lastUpdated = DateTime.UtcNow;
    }
}
