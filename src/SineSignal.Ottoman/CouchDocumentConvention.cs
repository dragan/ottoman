using System;
using System.Collections.Generic;
using System.Reflection;

using SineSignal.Ottoman.Generators;

namespace SineSignal.Ottoman
{
	public class CouchDocumentConvention : ICouchDocumentConvention
	{
		const string DEFAULT_IDENTITY_PROPERTY_NAME = "Id";
		
		static readonly IDictionary<Type, object> identityGenerators = InitializeIdentityGenerators();
		
		public virtual string IdentityPropertyName { get { return DEFAULT_IDENTITY_PROPERTY_NAME; } }

		public PropertyInfo GetIdentityPropertyFor(Type entityType)
		{
			return entityType.GetProperty(IdentityPropertyName);
		}
		
		public object GenerateIdentityFor(Type identityType)
		{
			// TODO: Introduce factory here and make sure user's can add their own generators
			object generatedValue = null;
			
			if (identityType == typeof(Guid))
			{
				generatedValue = GetIdentityGeneratorFor<Guid>(identityType).Generate();
			}
			
			return generatedValue;
		}
		
		private static IDictionary<Type, object> InitializeIdentityGenerators()
		{
			return new Dictionary<Type, object> { { typeof(Guid), new GuidIdentityGenerator() } };
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
