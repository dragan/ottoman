using System;
using System.Reflection;

namespace SineSignal.Ottoman
{
	public interface IDocumentConvention
	{
		PropertyInfo GetIdentityPropertyFor(Type entityType);
		Guid GenerateIdentityFor(Type identityType);
	}
}
