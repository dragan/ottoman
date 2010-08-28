using System;

namespace SineSignal.Ottoman.Generators
{
	public interface IIdentityGenerator<T>
	{
		T Generate();
	}
}
