using System;

namespace SineSignal.Ottoman.Commands
{
	public interface ICouchCommand
	{
		string Route { get; }
		string Operation { get; }
		object Message { get; }
	}
}
