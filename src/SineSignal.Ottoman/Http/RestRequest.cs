namespace SineSignal.Ottoman.Http
{
	public class RestRequest
	{
		public string Path { get; set; }
		public string Method { get; set; }
		public object Payload { get; set; }
	}
}
