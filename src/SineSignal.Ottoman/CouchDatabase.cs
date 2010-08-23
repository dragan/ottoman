using System;

namespace SineSignal.Ottoman
{
	public class CouchDatabase : ICouchDatabase
	{
		private CouchClient CouchClient { get; set; }
		
		public ICouchProxy CouchProxy
		{
			get { return CouchClient.CouchProxy; }
		}
		
		public IDocumentConvention DocumentConvention
		{
			get { throw new NotImplementedException(); }
		}
		
		public string Name { get; private set; }
		
		public CouchDatabase(CouchClient couchClient, string name)
		{
			CouchClient = couchClient;
			Name = name;
		}
	}
}
