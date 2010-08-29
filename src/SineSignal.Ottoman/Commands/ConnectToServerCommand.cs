using System;
using System.Net;

using SineSignal.Ottoman.Exceptions;
using SineSignal.Ottoman.Http;
using SineSignal.Ottoman.Serialization;

namespace SineSignal.Ottoman.Commands
{
	public class ConnectToServerCommand : ICouchCommand
	{
		public string Route { get; private set; }
		public string Operation { get; private set; }
		public object Message { get; private set; }
		public HttpStatusCode SuccessStatusCode { get; private set; }
		
		public ConnectToServerCommand()
		{
			Route = "/";
			Operation = HttpMethod.Get;
			Message = null;
			SuccessStatusCode = HttpStatusCode.OK;
		}
		
		public void HandleError(string serverAddress, CommandErrorResult errorResult, UnexpectedHttpResponseException innerException)
		{
			throw new CannotConnectToServerException(serverAddress, errorResult, innerException);
		}
	}
	
	public class ConnectToServerResult
	{
		[JsonMember("couchdb")]
		public string Message { get; set; }
		
		[JsonMember("version")]
		public string Version { get; set; }
	}
}
