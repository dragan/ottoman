namespace SineSignal.Ottoman
{
	public interface ICouchDatabase
	{
		ICouchProxy CouchProxy { get; }
		ICouchDocumentConvention CouchDocumentConvention { get; }
		string Name { get; }
		ICouchDocumentSession OpenDocumentSession();
	}
}
