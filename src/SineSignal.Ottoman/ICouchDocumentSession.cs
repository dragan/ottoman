namespace SineSignal.Ottoman
{
	public interface ICouchDocumentSession
	{
		void Store(object entity);
		T Load<T>(object id);
	}
}
