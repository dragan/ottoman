using System;
using System.Net;

using SineSignal.Ottoman.Exceptions;
using SineSignal.Ottoman.Http;
using SineSignal.Ottoman.Serialization;

namespace SineSignal.Ottoman.Commands
{
	public interface ICouchCommand
	{
		string Route { get; }
		string Operation { get; }
		object Message { get; }
		HttpStatusCode SuccessStatusCode { get; }
		void HandleError(string serverAddress, CommandErrorResult errorResult, UnexpectedHttpResponseException innerException);
	}
	
	public class CommandDefaultResult
	{
		[JsonMember("ok")]
		public bool Succeeded { get; set; }
	}
	
	public class CommandErrorResult
	{
		[JsonMember("error")]
		public string Error { get; set; }
		
		[JsonMember("reason")]
		public string Reason { get; set; }
	}
}
