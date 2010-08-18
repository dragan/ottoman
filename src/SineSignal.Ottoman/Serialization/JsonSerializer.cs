using System;

namespace SineSignal.Ottoman.Serialization
{
	public class JsonSerializer : ISerializer
	{
		public string Serialize(object obj)
		{
			return JsonConvert.ToJson(obj);
		}

		public T Deserialize<T>(string text)
		{
			return JsonConvert.ToObject<T>(text);
		}
	}
}
