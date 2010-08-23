using System;

namespace SineSignal.Ottoman
{
	public interface ICouchClient
	{
		string ServerVersion { get; }
		void CreateDatabase(string name);
		ICouchDatabase GetDatabase(string name);
		void DeleteDatabase(string name);
	}
}
