using System.Net;

namespace SineSignal.Ottoman.Http
{
	public interface IRestClient
	{
		RestResponse<T> Process<T>(RestRequest restRequest, HttpStatusCode successStatusCode);
	}
}
