using System;

using SineSignal.Ottoman.Commands;

namespace SineSignal.Ottoman.Exceptions
{
	public class CannotDeleteDatabaseException : CouchException
	{
		private const string ExceptionMessageFormat = "Failed to delete database '{0}'";

		public CannotDeleteDatabaseException(string databaseName, CommandErrorResult errorResult, UnexpectedHttpResponseException innerException)
			: base(String.Format(ExceptionMessageFormat, databaseName), errorResult, innerException)
		{
		}
	}
}
