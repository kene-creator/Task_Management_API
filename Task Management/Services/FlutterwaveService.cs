using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

public class FlutterwaveService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public FlutterwaveService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<string> GetBankListAsync()
    {
        var client = _httpClientFactory.CreateClient("FlutterwaveClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["Flutterwave:SecretKey"]);

        var response = await client.GetAsync("banks/NG");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
}
