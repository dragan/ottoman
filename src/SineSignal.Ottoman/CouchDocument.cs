using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace SineSignal.Ottoman
{
	public sealed class CouchDocument : DynamicObject
	{
		private Dictionary<string, object> Members { get; set; }
		private PropertyInfo IdentityProperty { get; set; }
		
		public CouchDocument(object entity, PropertyInfo identityProperty)
		{
			Members = new Dictionary<string, object>();
			IdentityProperty = identityProperty;
			CopyPropertiesFrom(entity);
			Members["type"] = entity.GetType().Name;
		}
		
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			string name = binder.Name.ToLower();

			return Members.TryGetValue(name, out result);
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			Members[binder.Name.ToLower()] = value;

			return true;
		}
		
		private void CopyPropertiesFrom(object source)
		{
			foreach (PropertyInfo property in source.GetType().GetProperties().Where(p => p.CanRead))
			{
				var key = property.Name;
				if (key == IdentityProperty.Name)
					key = "_id";
				
				var value = property.GetValue(source, null);
				Members[key.ToLower()] = value;
			}
		}
	}
}
