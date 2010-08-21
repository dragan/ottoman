using System;

using SineSignal.Ottoman.Http;
using SineSignal.Ottoman.Serialization;

namespace SineSignal.Ottoman.Commands
{
	public class ConnectToServerCommand : ICouchCommand
	{
		public string Route { get; private set; }
		public string Operation { get; private set; }
		public object Message { get; private set; }
		
		public ConnectToServerCommand()
		{
			Route = "/";
			Operation = HttpMethod.Get;
			Message = null;
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
