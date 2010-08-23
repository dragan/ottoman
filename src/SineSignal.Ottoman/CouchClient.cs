using System;

using SineSignal.Ottoman.Commands;

namespace SineSignal.Ottoman
{
	public class CouchClient : ICouchClient
	{
		public string ServerVersion { get; private set; }
		
		public ICouchProxy CouchProxy { get; private set; }
		
		private CouchClient(ICouchProxy couchProxy, string serverVersion)
		{
			CouchProxy = couchProxy;
			ServerVersion = serverVersion;
		}
		
		public void CreateDatabase(string name)
		{
			var createDatabaseCommand = new CreateDatabaseCommand(name);
			CouchProxy.Execute<CommandDefaultResult>(createDatabaseCommand);
		}
		
		public ICouchDatabase GetDatabase(string name)
		{
			var getDatabaseCommand = new GetDatabaseCommand(name);
			GetDatabaseResult result = CouchProxy.Execute<GetDatabaseResult>(getDatabaseCommand);
			
			return new CouchDatabase(this, result.DatabaseName);
		}
		
		public void DeleteDatabase(string name)
		{
			var deleteDatabaseCommand = new DeleteDatabaseCommand(name);
			CouchProxy.Execute<CommandDefaultResult>(deleteDatabaseCommand);
		}
		
		public static ICouchClient ConnectTo(string address)
		{
			ICouchProxy couchProxy = new CouchProxy(new Uri(address));
			ICouchCommand couchCommand = new ConnectToServerCommand();
			ConnectToServerResult couchResult = couchProxy.Execute<ConnectToServerResult>(couchCommand);
			
			return new CouchClient(couchProxy, couchResult.Version);
		}
	}
}
