using System;
using System.Collections.Generic;
using System.Reflection;

namespace SineSignal.Ottoman
{
	public class CouchDocumentSession
	{
		public IDocumentConvention DocumentConvention { get; private set; }
		private Dictionary<string, object> IdentityMap { get; set; }
		
		public CouchDocumentSession(IDocumentConvention documentConvention)
		{
			DocumentConvention = documentConvention;
			IdentityMap = new Dictionary<string, object>();
		}
		
		public void Store(object entity)
		{
			// What do we want the store method to do
			// 1.  Using the default convention, find the id property
			// 2.  Using the default convention, generate a new id
			// 3.  Set our found id property to the generated id
			// 4.  Add the entity to the identity map
			Type entityType = entity.GetType();
			PropertyInfo identityProperty = DocumentConvention.GetIdentityPropertyFor(entityType);
			var id = DocumentConvention.GenerateIdentityFor(identityProperty.PropertyType);
			identityProperty.SetValue(entity, id, null);
			IdentityMap[id.ToString()] = entity;
		}
		
		public T Load<T>(string id)
		{
			object existingEntity;
		    if(IdentityMap.TryGetValue(id, out existingEntity))
		    {
		        return (T)existingEntity;
		    }
			
			return default(T);
		}
	}
}
