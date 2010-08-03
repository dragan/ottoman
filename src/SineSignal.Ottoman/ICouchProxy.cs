using SineSignal.Ottoman.Commands;

namespace SineSignal.Ottoman
{
	public interface ICouchProxy
	{
		void Execute(ICouchCommand couchCommand);
	}
}
