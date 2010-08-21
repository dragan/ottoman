using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SineSignal.Ottoman
{
	public sealed class CouchDocument : Dictionary<string, object>, IDictionary<string, object>
	{
		private Dictionary<string, object> Members { get; set; }
		private PropertyInfo IdentityProperty { get; set; }
		
		public CouchDocument(object entity, PropertyInfo identityProperty)
		{
			IdentityProperty = identityProperty;
			CopyPropertiesFrom(entity);
			this["Type"] = entity.GetType().Name;
		}
		
		private void CopyPropertiesFrom(object source)
		{
			foreach (PropertyInfo property in source.GetType().GetProperties().Where(p => p.CanRead))
			{
				var key = property.Name;
				if (key == IdentityProperty.Name)
					key = "_id";
				
				var value = property.GetValue(source, null);
				this[key] = value;
			}
		}
	}
}
