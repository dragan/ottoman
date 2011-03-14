using System;
using System.Reflection;

namespace SineSignal.Ottoman
{
	public interface ICouchDocumentConvention
	{
		string IdentityPropertyName { get; }
		PropertyInfo GetIdentityPropertyFor(Type entityType);
		object GenerateIdentityFor(Type identityType);
	}
}
