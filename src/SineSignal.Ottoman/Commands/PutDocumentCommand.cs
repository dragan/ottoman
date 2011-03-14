using System;
using System.Net;

using SineSignal.Ottoman.Exceptions;
using SineSignal.Ottoman.Http;
using SineSignal.Ottoman.Serialization;

namespace SineSignal.Ottoman.Commands
{
	public class PutDocumentCommand : ICouchCommand
	{
		public string Route { get; private set; }
		public string Operation { get; private set; }
		public object Message { get; private set; }
		public HttpStatusCode SuccessStatusCode { get; private set; }
		
		private string DatabaseName { get; set; }
		private string DocumentId { get; set; }
		
		public PutDocumentCommand(string databaseName, string documentId, object couchDocument)
		{
			DatabaseName = databaseName;
			DocumentId = documentId;
			
			Route = DatabaseName + "/" + DocumentId;
			Operation = HttpMethod.Put;
			Message = couchDocument;
			SuccessStatusCode = HttpStatusCode.Created;
		}
		
		public void HandleError(string serverAddress, CommandErrorResult errorResult, UnexpectedHttpResponseException innerException)
		{
			throw new NotImplementedException();
		}
	}
	
	internal class PutDocumentResult
	{
		[JsonMember("ok")]
		public bool Stored { get; set; }
		
		[JsonMember("id")]
		public string Id { get; set; }
		
		[JsonMember("rev")]
		public string Revision { get; set; }
	}
}
