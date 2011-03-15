using System;
using System.Collections.Generic;
using System.Reflection;

using SineSignal.Ottoman.Commands;
using SineSignal.Ottoman.Extensions;

namespace SineSignal.Ottoman
{
	public class CouchDocumentSession : ICouchDocumentSession
	{
		private readonly CouchDocumentSessionCache sessionCache = new CouchDocumentSessionCache();
		
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
				sessionCache.Store(entityType, id, entity);
			}
		}
		
		public T Load<T>(object id)
		{
			Type entityType = typeof(T);
			object entity = sessionCache.TryToFind(entityType, id);
			
			if (entity == null)
			{
				CouchDocument couchDocument = CouchProxy.Execute<CouchDocument>(new GetDocumentCommand(CouchDatabase.Name, id.ToString()));
				PropertyInfo identityProperty = CouchDocumentConvention.GetIdentityPropertyFor(entityType);
				entity = couchDocument.HydrateEntity<T>(identityProperty);
				sessionCache.Store(entityType, id, entity);
			}
			
			return (T)entity;
		}
		
		private class CouchDocumentSessionCache
		{
			private readonly Dictionary<Type, Dictionary<string, WeakReference>> cache = new Dictionary<Type, Dictionary<string, WeakReference>>();
			
			public object TryToFind(Type type, object id)
			{
				if (!cache.ContainsKey(type))
					return null;
				
				string identifier = id.ToString();
				if (!cache[type].ContainsKey(identifier))
					return null;
				
				return cache[type][identifier].Target;
			}
			
			public void Store(Type type, object id, object entity)
			{
				if (!cache.ContainsKey(type))
					cache.Add(type, new Dictionary<string, WeakReference>());
				
				cache[type][id.ToString()] = new WeakReference(entity);
			}
		}
	}
}
