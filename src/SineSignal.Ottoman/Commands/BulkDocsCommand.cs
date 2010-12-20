using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using SineSignal.Ottoman.Exceptions;
using SineSignal.Ottoman.Http;
using SineSignal.Ottoman.Serialization;

namespace SineSignal.Ottoman.Commands
{
	internal class BulkDocsCommand : ICouchCommand
	{
		private readonly BulkDocsMessage _message;
		
		public string Route { get; private set; }
		
		public string Operation { get; private set; }
		
		public object Message
		{
			get { return _message; }
		}
		
		public HttpStatusCode SuccessStatusCode { get; private set; }
		
		public BulkDocsCommand(string databaseName, BulkDocsMessage message)
		{
			Route = databaseName + "/_bulk_docs";
			Operation = HttpMethod.Post;
			_message = message;
			SuccessStatusCode = HttpStatusCode.Created;
		}
		
		public void HandleError(string serverAddress, CommandErrorResult errorResult, UnexpectedHttpResponseException innerException)
		{
			throw new NotImplementedException();
		}
	}
	
	internal class BulkDocsMessage
	{
		[JsonMember("non_atomic")]
		public bool NonAtomic { get; private set; }
		
		[JsonMember("all_or_nothing")]
		public bool AllOrNothing { get; private set; }
		
		[JsonMember("docs")]
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
	
	internal class BulkDocsResult
	{
		[JsonMember("id")]
		public string Id { get; set; }
		
		[JsonMember("rev")]
		public string Rev { get; set; }
		
		[JsonMember("error")]
		public string Error { get; set; }
		
		[JsonMember("reason")]
		public string Reason { get; set; }
	}
}
