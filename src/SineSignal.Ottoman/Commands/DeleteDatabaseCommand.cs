using System;
using System.Net;

using SineSignal.Ottoman.Exceptions;
using SineSignal.Ottoman.Http;

namespace SineSignal.Ottoman.Commands
{
	public class DeleteDatabaseCommand : ICouchCommand
	{
		public string Route { get; private set; }
		public string Operation { get; private set; }
		public object Message { get; private set; }
		public HttpStatusCode SuccessStatusCode { get; private set; }
		
		private string DatabaseName { get; set; }
		
		public DeleteDatabaseCommand(string databaseName)
		{
			DatabaseName = databaseName;
			
			Route = DatabaseName;
			Operation = HttpMethod.Delete;
			Message = null;
			SuccessStatusCode = HttpStatusCode.OK;
		}
		
		public void HandleError(string serverAddress, CommandErrorResult errorResult, UnexpectedHttpResponseException innerException)
		{
			throw new CannotDeleteDatabaseException(DatabaseName, errorResult, innerException);
		}
	}
}
