using System;

namespace SineSignal.Ottoman
{
	public class CouchDocumentConvention : ICouchDocumentConvention
	{
		public string IdentityPropertyName
		{
			get { throw new NotImplementedException(); }
		}

		public System.Reflection.PropertyInfo GetIdentityPropertyFor(Type entityType)
		{
			throw new NotImplementedException();
		}
		
		public object GenerateIdentityFor(Type identityType)
		{
			throw new NotImplementedException();
		}
	}
}
