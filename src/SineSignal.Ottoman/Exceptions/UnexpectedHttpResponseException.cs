using System;
using System.Net;

using SineSignal.Ottoman.Http;

namespace SineSignal.Ottoman.Exceptions
{
	public class UnexpectedHttpResponseException : Exception
	{
		private const string ExceptionMessageFormat = "Received an unexpected response: Expected Status Code '{0}', Received Status Code '{1}'";
		
		public HttpResponse RawResponse { get; private set; }
		
		public UnexpectedHttpResponseException(HttpStatusCode expectedStatus, HttpResponse rawResponse) : 
			base(String.Format(ExceptionMessageFormat, expectedStatus, rawResponse.StatusCode))
		{
			RawResponse = rawResponse;
		}
	}
}
