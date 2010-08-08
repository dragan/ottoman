using System;

using SineSignal.Ottoman.Serialization;

namespace SineSignal.Ottoman.Http
{
	public class RestClient : IRestClient
	{
		private UriBuilder _requestUri;
		
		public Uri BaseUri { get; private set; }
		private IHttpClient HttpClient { get; set; }
		private ISerializer Serializer { get; set; }
		
		public RestClient(Uri baseUri, IHttpClient httpClient, ISerializer serializer)
		{
			_requestUri = new UriBuilder(baseUri);
			BaseUri = baseUri;
			HttpClient = httpClient;
			Serializer = serializer;
		}
		
		public RestResponse<T> Process<T>(RestRequest restRequest)
		{
			HttpRequest httpRequest = ConverToHttpRequestFrom(restRequest);
			HttpResponse httpResponse = HttpClient.Send(httpRequest);
			
			return ConvertToRestResponseFrom<T>(httpResponse, restRequest);
		}
		
		private HttpRequest ConverToHttpRequestFrom(RestRequest restRequest)
		{
			_requestUri.Path = restRequest.Path;
			
			return new HttpRequest {
				Url = _requestUri.Uri,
				Accept = "application/json",
				Method = restRequest.Method,
				ContentType = "application/json",
				Content = Serializer.Serialize(restRequest.Payload)
			};
		}
		
		private RestResponse<T> ConvertToRestResponseFrom<T>(HttpResponse httpResponse, RestRequest restRequest)
		{
			return new RestResponse<T> {
				RestRequest = restRequest,
				ContentType = httpResponse.ContentType,
				ContentLength = httpResponse.ContentLength,
				Content = httpResponse.Content,
				StatusCode = httpResponse.StatusCode,
				StatusDescription = httpResponse.StatusDescription,
				ContentDeserialized = Serializer.Deserialize<T>(httpResponse.Content)
			};
		}
	}
}
