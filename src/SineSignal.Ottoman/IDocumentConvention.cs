using System;
using System.Reflection;

namespace SineSignal.Ottoman
{
	public interface IDocumentConvention
	{
		PropertyInfo GetIdentityPropertyFor(Type entityType);
		object GenerateIdentityFor(Type identityType);
	}
}
