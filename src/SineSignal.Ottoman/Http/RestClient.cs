using System;
using System.Net;

using SineSignal.Ottoman.Exceptions;
using SineSignal.Ottoman.Serialization;

namespace SineSignal.Ottoman.Http
{
	public class RestClient : IRestClient
	{
		private UriBuilder _requestUri;
		
		public Uri BaseUri { get; private set; }
		private IHttpClient HttpClient { get; set; }
		private ISerializer Serializer { get; set; }
		
		public RestClient(Uri baseUri) : this(baseUri, new HttpClient(), new JsonSerializer())
		{
		}
		
		public RestClient(Uri baseUri, IHttpClient httpClient, ISerializer serializer)
		{
			_requestUri = new UriBuilder(baseUri);
			BaseUri = baseUri;
			HttpClient = httpClient;
			Serializer = serializer;
		}
		
		public RestResponse<T> Process<T>(RestRequest restRequest, HttpStatusCode successStatusCode)
		{
			HttpRequest httpRequest = ConverToHttpRequestFrom(restRequest);
			HttpResponse httpResponse = HttpClient.Send(httpRequest);
			
			if (httpResponse.StatusCode == successStatusCode)
			{
				return ConvertToRestResponseFrom<T>(httpResponse, restRequest);
			}
			else
			{
				throw new UnexpectedHttpResponseException(successStatusCode, httpResponse);
			}
		}
		
		private HttpRequest ConverToHttpRequestFrom(RestRequest restRequest)
		{
			_requestUri.Path = restRequest.Path;
			
			string content = String.Empty;
			string contentType = String.Empty;
			
			if (restRequest.Method == HttpMethod.Put || 
				restRequest.Method == HttpMethod.Post)
			{
				if (restRequest.Payload != null)
				{
					contentType = "application/json";
					content = Serializer.Serialize(restRequest.Payload);
				}
			}
			
			return new HttpRequest {
				Url = _requestUri.Uri,
				Accept = "application/json",
				Method = restRequest.Method,
				ContentType = contentType,
				Content = content
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
