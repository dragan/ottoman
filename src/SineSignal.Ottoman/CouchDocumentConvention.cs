using System;
using System.Collections.Generic;
using System.Reflection;

using SineSignal.Ottoman.Generators;

namespace SineSignal.Ottoman
{
	public class CouchDocumentConvention : ICouchDocumentConvention
	{
		private static IDictionary<Type, object> identityGenerators = InitializeIdentityGenerators();
		
		public virtual PropertyInfo GetIdentityPropertyFor(Type entityType)
		{
			return entityType.GetProperty("Id");
		}

		public object GenerateIdentityFor(Type identityType)
		{
			object generatedValue = null;
			
			if (identityType == typeof(Guid))
			{
				generatedValue = GetIdentityGeneratorFor<Guid>(identityType).Generate();
			}
			
			return generatedValue;
		}
		
		private static IDictionary<Type, object> InitializeIdentityGenerators()
		{
			IDictionary<Type, object> identityGenerators = new Dictionary<Type, object>();
			
			identityGenerators.Add(typeof(Guid), new GuidIdentityGenerator());
			
			return identityGenerators;
		}
		
		private static IIdentityGenerator<T> GetIdentityGeneratorFor<T>(Type type)
		{
			object generator;
			
			if (identityGenerators.TryGetValue(type, out generator))
			{
				return generator as IIdentityGenerator<T>;
			}
			
			return null;
		}
	}
}
