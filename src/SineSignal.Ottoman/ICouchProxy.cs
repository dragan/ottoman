using SineSignal.Ottoman.Commands;

namespace SineSignal.Ottoman
{
	public interface ICouchProxy
	{
		TResult Execute<TResult>(ICouchCommand couchCommand);
	}
}
