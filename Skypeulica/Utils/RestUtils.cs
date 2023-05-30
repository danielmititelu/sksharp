using RestSharp;

namespace Skypeulica.Utils
{
    internal static class RestUtils
    {
        internal static string PostJson(string baseURL, string endpoint, string json)
        {
            var client = new RestClient(baseURL);
            var request = new RestRequest(endpoint, Method.Post);
            request.AddHeader("Accept", "application/json");
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = client.Execute(request);
            return response.Content;
        }

        internal static string PostData(string baseURL, string endpoint, object data, Dictionary<string, string> headers = null)
        {
            var client = new RestClient(baseURL);
            var request = new RestRequest(endpoint, Method.Post);
            request.AddHeader("Accept", "application/json");
            request.AddParameter("application/json", data, ParameterType.RequestBody);
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }

            var response = client.Execute(request);
            return response.Content;
        }
    }
}
