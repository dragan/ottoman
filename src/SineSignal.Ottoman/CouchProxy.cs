using SineSignal.Ottoman.Commands;
using SineSignal.Ottoman.Http;

namespace SineSignal.Ottoman
{
	public class CouchProxy : ICouchProxy
	{
		public IRestClient RestClient { get; private set; }
		
		public CouchProxy(IRestClient restClient)
		{
			RestClient = restClient;
		}
		
		public TResult Execute<TResult>(ICouchCommand couchCommand)
		{
			var restRequest = CreateRestRequestFrom(couchCommand);
			
			RestResponse<TResult> restResponse = RestClient.Process<TResult>(restRequest);
			
			return restResponse.ContentDeserialized;
		}
		
		private RestRequest CreateRestRequestFrom(ICouchCommand couchCommand)
		{
			return new RestRequest { Path = couchCommand.Route, Method = couchCommand.Operation, Payload = couchCommand.Message };
		}
	}
}
