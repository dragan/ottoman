using System;
using System.Reflection;

namespace SineSignal.Ottoman
{
	public interface ICouchDocumentConvention
	{
		PropertyInfo GetIdentityPropertyFor(Type entityType);
		object GenerateIdentityFor(Type identityType);
	}
}
