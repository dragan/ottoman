namespace SineSignal.Ottoman.Http
{
	public class RestResponse<T>
	{
		public RestRequest RestRequest { get; set; }
		public string ContentType { get; set; }
		public long ContentLength { get; set; }
		public string Content { get; set; }
		public System.Net.HttpStatusCode StatusCode { get; set; }
		public string StatusDescription { get; set; }
		public T ContentDeserialized { get; set; }
	}
}
