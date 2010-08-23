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
		public Action<CommandErrorResult, UnexpectedHttpResponseException> OnErrorHandler { get { throw new NotImplementedException(); } }
		
		public CreateDatabaseCommand(string databaseName)
		{
			Route = databaseName;
			Operation = HttpMethod.Put;
			Message = null;
			SuccessStatusCode = HttpStatusCode.Created;
		}
	}
}
