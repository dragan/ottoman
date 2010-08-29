using System;

using SineSignal.Ottoman.Commands;
using SineSignal.Ottoman.Exceptions;
using SineSignal.Ottoman.Http;
using SineSignal.Ottoman.Serialization;

namespace SineSignal.Ottoman
{
	public class CouchProxy : ICouchProxy
	{
		public IRestClient RestClient { get; private set; }
		
		private ISerializer Serializer { get; set; }
		
		public CouchProxy(Uri serverLocation) : this(new RestClient(serverLocation))
		{
			Serializer = new JsonSerializer();
		}
		
		public CouchProxy(IRestClient restClient)
		{
			RestClient = restClient;
		}
		
		public TResult Execute<TResult>(ICouchCommand couchCommand)
		{
			var restRequest = CreateRestRequestFrom(couchCommand);
			
			RestResponse<TResult> restResponse = null;
			
			try
			{
				restResponse = RestClient.Process<TResult>(restRequest, couchCommand.SuccessStatusCode);
			}
			catch (UnexpectedHttpResponseException e)
			{
				CommandErrorResult errorResult;
				if (!String.IsNullOrEmpty(e.RawResponse.Content))
				{
					errorResult = Serializer.Deserialize<CommandErrorResult>(e.RawResponse.Content);
				}
				else
				{
					errorResult = new CommandErrorResult { Error = "Unexpected Exception", Reason = e.RawResponse.Error.Message };
				}
				
				couchCommand.HandleError(RestClient.BaseUri.ToString(), errorResult, e);
			}
			
			return restResponse.ContentDeserialized;
		}
		
		private RestRequest CreateRestRequestFrom(ICouchCommand couchCommand)
		{
			return new RestRequest { Path = couchCommand.Route, Method = couchCommand.Operation, Payload = couchCommand.Message };
		}
	}
}
