using System;

using SineSignal.Ottoman.Commands;

namespace SineSignal.Ottoman.Exceptions
{
	public class CannotConnectToServerException : CouchException
	{
		private const string ExceptionMessageFormat = "Unable to connect to '{0}'";
		
		public CannotConnectToServerException(string address, CommandErrorResult errorResult, UnexpectedHttpResponseException innerException) : 
			base(String.Format(ExceptionMessageFormat, address), errorResult, innerException)
		{
		}
	}
}
