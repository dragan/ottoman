using System;

using SineSignal.Ottoman.Commands;
using SineSignal.Ottoman.Http;

namespace SineSignal.Ottoman.Exceptions
{
	public class CouchException : Exception
	{
		public CommandErrorResult CouchError { get; private set; }

		public CouchException(string message, CommandErrorResult errorResult, UnexpectedHttpResponseException innerException) 
			: base(message, innerException)
		{
			CouchError = errorResult;
		}
	}
}
