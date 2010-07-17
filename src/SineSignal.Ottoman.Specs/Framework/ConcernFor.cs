namespace SineSignal.Ottoman.Specs.Framework
{
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
