using NUnit.Framework;

namespace SineSignal.Ottoman.Specs.Framework
{
	[TestFixture]
	public abstract class ConcernFor<TSystemUnderTest> : BaseConcern
	{
		public TSystemUnderTest Sut { get; private set; }
		
		protected override void AfterGivenEstablished()
		{
			Sut = CreateSystemUnderTest();
		}
		
		public abstract TSystemUnderTest CreateSystemUnderTest();
	}
}
