using System.Net.Http.Headers;
using System.Net.Http.Json;

internal class RestService
{
    HttpClient _httpClient;
    public RestService()
    {
        _httpClient = new HttpClient();
    }

    internal async Task<RestResponse> Get(string url, Dictionary<string, string>? headers = null)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url)
        };

        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        var response = await _httpClient.SendAsync(request);
        return new RestResponse
        {
            Content = await response.Content.ReadAsStringAsync(),
            StatusCode = response.StatusCode,
            Headers = response.Headers.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault() ?? string.Empty)
        };
    }

    internal async Task<T> Get<T>(string url, Dictionary<string, string>? headers = null)
    {

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url)
        };

        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadFromJsonAsync<T>();
        if (content is null)
        {
            throw new Exception($"Could not get response content from {url}");
        }
        return content;
    }

    internal async Task<string> Post(string url, object data)
    {
        var response = await _httpClient.PostAsJsonAsync(url, data);
        if (response.Content == null || response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception($"Could not get response from {url}");
        }
        return await response.Content.ReadAsStringAsync();
    }

    internal async Task<T> Post<T>(string url, object data)
    {
        var response = await _httpClient.PostAsJsonAsync(url, data);
        if (response.Content == null || response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception($"Could not get response from {url}");
        }
        var content = await response.Content.ReadFromJsonAsync<T>();
        if (content is null)
        {
            throw new Exception($"Could not get response from {url}");
        }
        return content;
    }
}