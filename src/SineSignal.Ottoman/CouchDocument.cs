using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SineSignal.Ottoman
{
	internal class CouchDocument : Dictionary<string, object>, IDictionary<string, object>
	{
		public static CouchDocument Dehydrate(object entity, PropertyInfo identityProperty, string revision)
		{
			var couchDocument = new CouchDocument();
			var entityType = entity.GetType();
			
			couchDocument["Type"] = entityType.Name;
			
			if (revision != String.Empty)
				couchDocument["_rev"] = revision;
			
			foreach (PropertyInfo property in entityType.GetProperties().Where(p => p.CanRead))
			{
				var key = property.Name;
				if (key == identityProperty.Name)
					key = "_id";
				
				var propertyValue = property.GetValue(entity, null);
				couchDocument[key] = propertyValue;
			}
			
			return couchDocument;
		}
		
		public T Hydrate<T>(PropertyInfo identityProperty) where T : new()
		{
			var instance = new T();
			var instanceType = instance.GetType();
			
			foreach (KeyValuePair<string, object> pair in this)
			{
				if (pair.Key == "_rev")
					continue;
				
				if (pair.Key == "Type")
					continue;
				
				PropertyInfo property;
				
				if (pair.Key == "_id")
				{
					property = instanceType.GetProperty(identityProperty.Name);
				}
				else
				{
					property = instanceType.GetProperty(pair.Key);
				}
				
				if (property.PropertyType == typeof(Guid))
				{
					property.SetValue(instance, new Guid(pair.Value.ToString()), null);
				}
				else
				{
					property.SetValue(instance, pair.Value, null);
				}
			}
			
			return instance;
		}
	}
}
