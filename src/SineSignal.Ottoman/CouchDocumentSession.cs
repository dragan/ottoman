using System;
using System.Collections.Generic;
using System.Reflection;

using SineSignal.Ottoman.Commands;
using SineSignal.Ottoman.Exceptions;
using SineSignal.Ottoman.Http;

namespace SineSignal.Ottoman
{
	public class CouchDocumentSession : ICouchDocumentSession
	{
		public ICouchDatabase CouchDatabase { get; private set; }

		private Dictionary<string, object> IdentityMap { get; set; }
		private Dictionary<string, DocumentMetadata> MetaDataMap { get; set; }
		
		public CouchDocumentSession(ICouchDatabase couchDatabase)
		{
			CouchDatabase = couchDatabase;
			IdentityMap = new Dictionary<string, object>();
			MetaDataMap = new Dictionary<string, DocumentMetadata>();
		}
		
		public void Store(object entity)
		{
			Type entityType = entity.GetType();
			PropertyInfo identityProperty = CouchDatabase.CouchDocumentConvention.GetIdentityPropertyFor(entityType);
			
			object id = null;
			if (identityProperty != null)
			{
				id = GetIdentityValueFor(entity, identityProperty);
				
				if (id == null)
				{
					id = CouchDatabase.CouchDocumentConvention.GenerateIdentityFor(identityProperty.PropertyType);
					identityProperty.SetValue(entity, id, null);
				}
			}
			
			if (id != null)
			{
				if (IdentityMap.ContainsKey(id.ToString()))
				{
					if (ReferenceEquals(IdentityMap[id.ToString()], entity))
						return;
					
					throw new NonUniqueEntityException("Attempted to associate a different entity with id '" + id + "'.");
				}
				
				IdentityMap[id.ToString()] = entity;
			}
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
		
		public void SaveChanges()
		{
			var docs = new List<CouchDocument>();
			foreach (object entity in IdentityMap.Values)
			{
				PropertyInfo identityProperty = CouchDatabase.CouchDocumentConvention.GetIdentityPropertyFor(entity.GetType());
				var couchDocument = new CouchDocument(entity, identityProperty);
				docs.Add(couchDocument);
			}
			
			var bulkDocsMessage = new BulkDocsMessage(docs);
			var bulkDocsCommand = new BulkDocsCommand(CouchDatabase.Name, bulkDocsMessage);
			BulkDocsResult[] results = CouchDatabase.CouchProxy.Execute<BulkDocsResult[]>(bulkDocsCommand);
			
			for (int index = 0; index < results.Length; index++)
			{
				BulkDocsResult result = results[index];
				MetaDataMap[result.Id] = new DocumentMetadata { Id = result.Id, Rev = result.Rev };
			}
		}
		
		private static object GetIdentityValueFor(object entity, PropertyInfo identityProperty)
		{
			object id = identityProperty.GetValue(entity, null);
			
			Type propertyType = identityProperty.PropertyType;
			if (propertyType == typeof(Guid) && (Guid)id == Guid.Empty)
			{
				id = null;
			}
			
			return id;
		}
		
		public class DocumentMetadata
		{
			public string Id { get; set; }
			public string Rev { get; set; }
		}
	}
}
