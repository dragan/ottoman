namespace SineSignal.Ottoman.Http
{
	public interface IHttpClient
	{
		HttpResponse Send(HttpRequest httpRequest);
	}
}
