using System;
using System.Net;

using SineSignal.Ottoman.Exceptions;
using SineSignal.Ottoman.Http;
using SineSignal.Ottoman.Serialization;

namespace SineSignal.Ottoman.Commands
{
	internal class GetDatabaseCommand : ICouchCommand
	{
		public string Route { get; private set; }
		public string Operation { get; private set; }
		public object Message { get; private set; }
		public HttpStatusCode SuccessStatusCode { get; private set; }
		
		private string DatabaseName { get; set; }
		
		public GetDatabaseCommand(string databaseName)
		{
			DatabaseName = databaseName;
			
			Route = DatabaseName;
			Operation = HttpMethod.Get;
			Message = null;
			SuccessStatusCode = HttpStatusCode.OK;
		}
		
		public void HandleError(string serverAddress, CommandErrorResult errorResult, UnexpectedHttpResponseException innerException)
		{
			throw new CannotGetDatabaseException(DatabaseName, errorResult, innerException);
		}
	}
	
	internal class GetDatabaseResult
	{
		[JsonMember("db_name")]
		public string DatabaseName { get; set; }
		
		[JsonMember("doc_count")]
		public int DocumentCount { get; set; }
		
		[JsonMember("doc_del_count")]
		public int DeletedDocumentCount { get; set; }
		
		[JsonMember("update_seq")]
		public int CurrentUpdateCount { get; set; }
		
		[JsonMember("purge_seq")]
		public int CurrentPurgeCount { get; set; }
		
		[JsonMember("compact_running")]
		public bool CompactRunning { get; set; }
		
		[JsonMember("disk_size")]
		public int DiskSize { get; set; }
		
		[JsonMember("instance_start_time")]
		public string InstanceStartTime { get; set; }
		
		[JsonMember("disk_format_version")]
		public int DiskFormatVersion { get; set; }
		
		[JsonMember("committed_update_seq")]
		public int CommittedUpdateSequence { get; set; }
	}
}
