namespace SineSignal.Ottoman
{
	public interface ICouchDatabase
	{
		ICouchProxy CouchProxy { get; }
		IDocumentConvention DocumentConvention { get; }
		string Name { get; }
	}
}
