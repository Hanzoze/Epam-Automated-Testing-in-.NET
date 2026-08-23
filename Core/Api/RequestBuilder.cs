using RestSharp;

namespace Core.Api
{
    public class RequestBuilder
    {
        private readonly RestRequest _restRequest;

        public RequestBuilder(string resource, Method method)
        {
            _restRequest = new RestRequest(resource, method);
        }

        public RequestBuilder AddHeader(string name, string value)
        {
            _restRequest.AddHeader(name, value);
            return this;
        }

        public RequestBuilder AddQueryParameter(string name, string value)
        {
            _restRequest.AddQueryParameter(name, value);
            return this;
        }

        public RequestBuilder AddBody(object body)
        {
            _restRequest.AddJsonBody(body);
            return this;
        }

        public RestRequest Build()
        {
            return _restRequest;
        }
    }
}