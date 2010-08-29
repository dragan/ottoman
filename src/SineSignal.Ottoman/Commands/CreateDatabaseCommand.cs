using System;
using System.Net;

using SineSignal.Ottoman.Exceptions;
using SineSignal.Ottoman.Http;

namespace SineSignal.Ottoman.Commands
{
	public class CreateDatabaseCommand : ICouchCommand
	{
		public string Route { get; private set; }
		public string Operation { get; private set; }
		public object Message { get; private set; }
		public HttpStatusCode SuccessStatusCode { get; private set; }
		
		private string DatabaseName { get; set; }
		
		public CreateDatabaseCommand(string databaseName)
		{
			DatabaseName = databaseName;
			
			Route = DatabaseName;
			Operation = HttpMethod.Put;
			Message = null;
			SuccessStatusCode = HttpStatusCode.Created;
		}
		
		public void HandleError(string serverAddress, CommandErrorResult errorResult, UnexpectedHttpResponseException innerException)
		{
			throw new CannotCreateDatabaseException(DatabaseName, errorResult, innerException);
		}
	}
}
