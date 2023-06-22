using System.Net.Http.Json;
using System.Text;
using System.Web;
using SkSharp.Models.SkypeApiModels;

internal class RestService
{
    readonly HttpClient _httpClient;
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

    internal async Task<T> Get<T>(
        string url,
        Dictionary<string, string>? headers = null,
        Dictionary<string, string>? queryParameters = null)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url),
        };

        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        if (queryParameters != null)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach (var parameter in queryParameters)
            {
                query[parameter.Key] = parameter.Value;
            }
            request.RequestUri = new Uri($"{url}?{query}");
        }

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadFromJsonAsync<T>();
        if (content is null)
        {
            throw new Exception($"Could not get response content from {url}");
        }
        return content;
    }

    internal async Task<RestResponse> PostJson(string url, object data, Dictionary<string, string>? headers = null)
    {
        var request = JsonContent.Create(data);
        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        var response = await _httpClient.PostAsync(url, request);

        return new RestResponse
        {
            Content = await response.Content.ReadAsStringAsync(),
            StatusCode = response.StatusCode,
            Headers = response.Headers.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault() ?? string.Empty)
        };
    }

    internal async Task<T> PostJson<T>(string url, object data, Dictionary<string, string>? headers = null)
    {
        var request = JsonContent.Create(data);
        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        var response = await _httpClient.PostAsync(url, request);
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

    internal async Task<string> PostXml(string url, string data, Dictionary<string, string>? headers = null)
    {
        var request = new StringContent(data, Encoding.UTF8, "text/xml");
        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        var response = await _httpClient.PostAsync(url, request);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception($"Could not get response from {url}");
        }

        var content = await response.Content.ReadAsStringAsync();
        if (content is null)
        {
            throw new Exception($"Could not get response content from {url}");
        }

        return content;
    }

    internal async Task<RestResponse<T>> Post<T>(string url, Dictionary<string, string>? headers = null)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
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
        Console.WriteLine(response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<T>();
        if (content is null)
        {
            throw new Exception($"Could not get response content from {url}");
        }

        return new RestResponse<T>
        {
            Content = content,
            StatusCode = response.StatusCode,
            Headers = response.Headers.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault() ?? string.Empty)
        };
    }
}