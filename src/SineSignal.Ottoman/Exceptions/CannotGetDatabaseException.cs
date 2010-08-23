using System;

using SineSignal.Ottoman.Commands;
using SineSignal.Ottoman.Http;

namespace SineSignal.Ottoman.Exceptions
{
	public class CannotGetDatabaseException : CouchException
	{
		private const string ExceptionMessageFormat = "Failed to get database '{0}'";
		
		public CannotGetDatabaseException(string databaseName, CommandErrorResult errorResult, UnexpectedHttpResponseException innerException)
			: base(String.Format(ExceptionMessageFormat, databaseName), errorResult, innerException)
		{
		}
	}
}
