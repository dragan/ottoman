using System;
using System.Net;

using SineSignal.Ottoman.Exceptions;
using SineSignal.Ottoman.Http;

namespace SineSignal.Ottoman.Commands
{
	internal class GetDocumentCommand : ICouchCommand
	{
		public string Route { get; private set; }
		public string Operation { get; private set; }
		public object Message { get; private set; }
		public HttpStatusCode SuccessStatusCode { get; private set; }
		
		private string DatabaseName { get; set; }
		private string DocumentId { get; set; }
		
		public GetDocumentCommand(string databaseName, string documentId)
		{
			DatabaseName = databaseName;
			DocumentId = documentId;
			
			Route = DatabaseName + "/" + DocumentId;
			Operation = HttpMethod.Get;
			Message = null;
			SuccessStatusCode = HttpStatusCode.OK;
		}
		
		public void HandleError(string serverAddress, CommandErrorResult errorResult, UnexpectedHttpResponseException innerException)
		{
			throw new NotImplementedException();
		}
	}
}
