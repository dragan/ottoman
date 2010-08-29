using System;
using System.Net;

namespace SineSignal.Ottoman.Http
{
	public interface IRestClient
	{
		Uri BaseUri { get; }
		RestResponse<T> Process<T>(RestRequest restRequest, HttpStatusCode successStatusCode);
	}
}
