using System;

namespace SineSignal.Ottoman.Generators
{
	public class GuidIdentityGenerator : IIdentityGenerator<Guid>
	{
		public Guid Generate()
		{
			return Guid.NewGuid();
		}
	}
}
