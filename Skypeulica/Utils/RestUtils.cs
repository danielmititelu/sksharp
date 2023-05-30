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

        internal static string GetData(string baseURL, string endpoint, Dictionary<string, string> headers = null, Dictionary<string, string> parameters = null)
        {
            var client = new RestClient(baseURL);
            var request = new RestRequest(endpoint, Method.Get);

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    request.AddParameter(parameter.Key, parameter.Value);
                }
            }

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

        internal static string PostData(string baseURL, string endpoint, object data, Dictionary<string, string> headers = null)
        {
            return DoRequest(baseURL, endpoint, data, headers, Method.Post);
        }

        internal static RestResponse PostDataResponse(string baseURL, string endpoint, object data, Dictionary<string, string> headers = null)
        {
            return DoRequestResponse(baseURL, endpoint, data, headers, Method.Post);
        }

        private static string DoRequest(string baseURL, string endpoint, object data, Dictionary<string, string> headers = null, Method method = Method.Post)
        {
            return DoRequestResponse(baseURL, endpoint, data, headers, method).Content;
        }

        private static RestResponse DoRequestResponse(string baseURL, string endpoint, object data, Dictionary<string, string> headers = null, Method method = Method.Post)
        {
            var client = new RestClient(baseURL);
            var request = new RestRequest(endpoint, method);
            request.AddHeader("Accept", "application/json");
            request.AddParameter("application/json", data, ParameterType.RequestBody);
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }

            return client.Execute(request);
        }
    }
}
