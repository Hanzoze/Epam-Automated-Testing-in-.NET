using RestSharp;
using RestSharp.Serializers.Json;
using Core.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Api
{
    public class ApiClient
    {
        private readonly RestClient _client;

        public ApiClient(string baseUrl)
        {
            var serializerOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true
            };

            var options = new RestClientOptions(baseUrl);

            _client = new RestClient(
                options,
                configureSerialization: s => s.UseSystemTextJson(serializerOptions)
            );
        }

        public async Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request)
        {
            TafLogger.Info($"Executing {request.Method} API request to: {_client.Options.BaseUrl}{request.Resource}");

            var response = await _client.ExecuteAsync<T>(request);

            TafLogger.Info($"Received response with Status Code: {(int)response.StatusCode} {response.StatusCode}");
            return response;
        }

        public async Task<RestResponse> ExecuteAsync(RestRequest request)
        {
            TafLogger.Info($"Executing {request.Method} API request to: {_client.Options.BaseUrl}{request.Resource}");

            var response = await _client.ExecuteAsync(request);

            TafLogger.Info($"Received response with Status Code: {(int)response.StatusCode} {response.StatusCode}");
            return response;
        }
    }
}