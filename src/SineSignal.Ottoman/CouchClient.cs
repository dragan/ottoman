using System;

using SineSignal.Ottoman.Commands;

namespace SineSignal.Ottoman
{
	public class CouchClient
	{
		public string ServerVersion { get; private set; }
		
		private ICouchProxy CouchProxy { get; set; }
		
		private CouchClient(ICouchProxy couchProxy, string serverVersion)
		{
			CouchProxy = couchProxy;
			ServerVersion = serverVersion;
		}
		
		public static CouchClient ConnectTo(string address)
		{
			ICouchProxy couchProxy = new CouchProxy(new Uri(address));
			ICouchCommand couchCommand = new ConnectToServerCommand();
			ConnectToServerResult couchResult = couchProxy.Execute<ConnectToServerResult>(couchCommand);
			
			return new CouchClient(couchProxy, couchResult.Version);
		}
	}
}
