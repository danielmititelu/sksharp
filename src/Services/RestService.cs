using SkSharp.Utils;
using System;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Web;

internal class RestService
{
    readonly HttpClient _httpClient;
    public RestService()
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
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

    internal async Task DownloadAsync(string requestUri, Dictionary<string, string>? headers, Stream destination, IProgress<float> progress = null, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(requestUri)
        };

        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }
        // Get the http headers first to examine the content length
        using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
        {
            var contentLength = response.Content.Headers.ContentLength;

            using (var download = await response.Content.ReadAsStreamAsync(cancellationToken))
            {

                // Ignore progress reporting when no progress reporter was 
                // passed or when the content length is unknown
                if (progress == null || !contentLength.HasValue)
                {
                    await download.CopyToAsync(destination);
                    return;
                }

                // Convert absolute progress (bytes downloaded) into relative progress (0% - 100%)
                var relativeProgress = new Progress<long>(totalBytes => progress.Report((float)totalBytes / contentLength.Value));
                // Use extension method to report progress while downloading
                await download.CopyToAsync(destination, 81920, relativeProgress, cancellationToken);
                progress.Report(1);
            }
        }
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

    internal async Task<RestResponse> PutJson(string url, object data, Dictionary<string, string>? headers = null)
    {
        var request = JsonContent.Create(data);
        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        var response = await _httpClient.PutAsync(url, request);

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