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
		
		public ICouchDocumentConvention CouchDocumentConvention { get; private set; }
		
		public string Name { get; private set; }
		
		public CouchDatabase(CouchClient couchClient, string name) : this(couchClient, name, new CouchDocumentConvention())
		{
		}
		
		public CouchDatabase(CouchClient couchClient, string name, ICouchDocumentConvention couchDocumentConvention)
		{
			CouchClient = couchClient;
			Name = name;
			CouchDocumentConvention = couchDocumentConvention;
		}
		
		public ICouchDocumentSession OpenDocumentSession()
		{
			return new CouchDocumentSession(this);
		}
	}
}
