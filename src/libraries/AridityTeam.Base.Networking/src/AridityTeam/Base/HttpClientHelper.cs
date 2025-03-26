using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AridityTeam.Base.Networking;

public class HttpClientHelper
{
    private readonly HttpClient _httpClient = new();

    public void SetBaseAddress(string baseAddress)
    {
        _httpClient.BaseAddress = new Uri(baseAddress);
    }

    public void SetDefaultRequestHeaders(string key, string value)
    {
        _httpClient.DefaultRequestHeaders.Add(key, value);
    }

    public async Task<string> GetAsync(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PostAsync(string endpoint, object data)
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PutAsync(string endpoint, object data)
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> DeleteAsync(string endpoint)
    {
        var response = await _httpClient.DeleteAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}