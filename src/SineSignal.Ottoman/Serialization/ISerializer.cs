namespace SineSignal.Ottoman.Serialization
{
	public interface ISerializer
	{
		string Serialize(object obj);
		T Deserialize<T>(string text);
	}
}
