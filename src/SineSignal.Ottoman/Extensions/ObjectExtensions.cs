using System;
using System.Reflection;

namespace SineSignal.Ottoman.Extensions
{
	internal static class ObjectExtensions
	{
		public static CouchDocument ToCouchDocument(this object entity, PropertyInfo identityProperty)
		{
			var couchDocument = new CouchDocument();
			var entityType = entity.GetType();
			
			couchDocument[CouchDocument.TYPE_KEY] = entityType.Name;
			
			foreach (var propertyInfo in entityType.GetProperties())
			{
				string propertyName = propertyInfo.Name;
				
				if (propertyName == identityProperty.Name)
					continue;
				
				couchDocument[propertyName] = propertyInfo.GetValue(entity, null);
			}
			
			return couchDocument;
		}
	}
}
