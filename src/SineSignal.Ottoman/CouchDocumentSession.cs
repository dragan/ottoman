using System;
using System.Collections.Generic;
using System.Reflection;

using SineSignal.Ottoman.Commands;
using SineSignal.Ottoman.Extensions;

namespace SineSignal.Ottoman
{
	public class CouchDocumentSession : ICouchDocumentSession
	{
		private readonly Dictionary<string, object> sessionCache = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
		
		public ICouchDatabase CouchDatabase { get; private set; }
		
		private ICouchDocumentConvention CouchDocumentConvention
		{
			get { return CouchDatabase.CouchDocumentConvention; }
		}
		
		private ICouchProxy CouchProxy
		{
			get { return CouchDatabase.CouchProxy; }
		}

		public CouchDocumentSession(ICouchDatabase couchDatabase)
		{
			CouchDatabase = couchDatabase;
		}
		
		public void Store(object entity)
		{
			Type entityType = entity.GetType();
			PropertyInfo identityProperty = CouchDocumentConvention.GetIdentityPropertyFor(entityType);
			object id = CouchDocumentConvention.GenerateIdentityFor(identityProperty.PropertyType);
			
			CouchDocument couchDocument = entity.ToCouchDocument(identityProperty);
			ICouchCommand couchCommand = new PutDocumentCommand(CouchDatabase.Name, id.ToString(), couchDocument);
			PutDocumentResult result = CouchProxy.Execute<PutDocumentResult>(couchCommand);
			
			if (result.Stored)
			{
				identityProperty.SetValue(entity, id, null);
				sessionCache[id.ToString()] = entity;
			}
		}
		
		public T Load<T>(string id)
		{
			object existingEntity;
			if (sessionCache.TryGetValue(id, out existingEntity))
			{
				return (T)existingEntity;
			}
			
			return default(T);
		}
	}
}
