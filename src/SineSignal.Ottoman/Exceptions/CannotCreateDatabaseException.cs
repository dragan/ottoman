using System;

using SineSignal.Ottoman.Commands;

namespace SineSignal.Ottoman.Exceptions
{
	public class CannotCreateDatabaseException : CouchException
	{
		private const string ExceptionMessageFormat = "Failed to create database '{0}'";

		public CannotCreateDatabaseException(string databaseName, CommandErrorResult errorResult, UnexpectedHttpResponseException innerException)
			: base(String.Format(ExceptionMessageFormat, databaseName), errorResult, innerException)
		{
		}
	}
}
