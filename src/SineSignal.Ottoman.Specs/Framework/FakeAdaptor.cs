using NSubstitute;

namespace SineSignal.Ottoman.Specs.Framework
{
	public static class FakeAdaptor
	{
		public static T Create<T>() where T : class
		{
			return Substitute.For<T>();
		}
	}
}
