using NUnit.Framework;

namespace SineSignal.Ottoman.Specs.Framework
{
	public abstract class BaseConcern
	{
		[SetUp]
		public void SetUp()
		{
			Given();
			AfterGivenEstablished();
			When();
		}
		
		protected virtual void Given() { }
		
		protected virtual void AfterGivenEstablished() { }
		
		protected virtual void When() { }
		
		protected T Fake<T>() where T : class
		{
			return FakeAdaptor.Create<T>();
		}
	}
}
