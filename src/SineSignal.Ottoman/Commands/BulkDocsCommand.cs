using System;
using System.Collections.Generic;
using System.Linq;

using SineSignal.Ottoman.Http;

namespace SineSignal.Ottoman.Commands
{
	public class BulkDocsCommand : ICouchCommand
	{
		public string Method { get; private set; }
		
		public BulkDocsCommand(string databaseName, BulkDocsMessage message)
		{
			Method = HttpMethod.Post;
		}
	}
	
	public class BulkDocsMessage
	{
		public bool NonAtomic { get; private set; }
		public bool AllOrNothing { get; private set; }
		public CouchDocument[] Docs { get; private set; }
			
		public BulkDocsMessage(IEnumerable<CouchDocument> docs) : this(false, false, docs)
		{
		}
		
		public BulkDocsMessage(bool nonAtomic, bool allOrNothing, IEnumerable<CouchDocument> docs)
		{
			NonAtomic = nonAtomic;
			AllOrNothing = allOrNothing;
			Docs = docs.ToArray();
		}
	}
	
	public class BulkDocsResult
	{
		public string Id { get; set; }
		public string Rev { get; set; }
		public string Error { get; set; }
		public string Reason { get; set; }
	}
}
