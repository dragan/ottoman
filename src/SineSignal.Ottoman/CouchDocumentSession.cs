using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using SineSignal.Ottoman.Commands;
using SineSignal.Ottoman.Exceptions;
using SineSignal.Ottoman.Http;

namespace SineSignal.Ottoman
{
	public class CouchDocumentSession : ICouchDocumentSession
	{
		public ICouchDatabase CouchDatabase { get; private set; }

		private Dictionary<string, object> IdentityMap { get; set; }
		private Dictionary<object, EntityMetadata> EntityMetadataMap { get; set; }
		private HashSet<object> DeletedEntities { get; set; }
		
		public CouchDocumentSession(ICouchDatabase couchDatabase)
		{
			CouchDatabase = couchDatabase;
			IdentityMap = new Dictionary<string, object>();
			EntityMetadataMap = new Dictionary<object, EntityMetadata>();
			DeletedEntities = new HashSet<object>();
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
				
				var entityMetadata = new EntityMetadata{ Key = id.ToString(), Revision = String.Empty };
				EntityMetadataMap.Add(entity, entityMetadata);
				IdentityMap[id.ToString()] = entity;
			}
		}
		
		public T Load<T>(string id) where T : new()
		{
			object existingEntity;
		    if(IdentityMap.TryGetValue(id, out existingEntity))
		    {
		        return (T)existingEntity;
		    }
			
			var getDocumentCommand = new GetDocumentCommand(CouchDatabase.Name, id);
			CouchDocument couchDocument = CouchDatabase.CouchProxy.Execute<CouchDocument>(getDocumentCommand);
			
			return StalkEntity<T>(couchDocument);
		}
		
		public void Delete<T>(T entity)
		{
			if (EntityMetadataMap.ContainsKey(entity) == false)
				throw new InvalidOperationException(String.Format("{0} is not associated with the current couch document session, cannot delete unknown entity.", entity));
			
			DeletedEntities.Add(entity);
		}
		
		public void SaveChanges()
		{
			var docs = new List<CouchDocument>();
			var entities = new List<object>();
			
			foreach (var entity in DeletedEntities)
			{
				EntityMetadata entityMetadata;
				if (EntityMetadataMap.TryGetValue(entity, out entityMetadata))
				{
					EntityMetadataMap.Remove(entity);
					IdentityMap.Remove(entityMetadata.Key);
					
					Type entityType = entity.GetType();
					PropertyInfo identityProperty = CouchDatabase.CouchDocumentConvention.GetIdentityPropertyFor(entityType);
					
					docs.Add(CouchDocument.Dehydrate(entity, identityProperty, entityMetadata.Revision, true));
					entities.Add(entity);
				}
			}
			DeletedEntities.Clear();
			
			foreach (var entity in EntityMetadataMap.Where(pair => IsEntityDirty(pair.Key, pair.Value)))
			{
				Type entityType = entity.Key.GetType();
				PropertyInfo identityProperty = CouchDatabase.CouchDocumentConvention.GetIdentityPropertyFor(entityType);
				docs.Add(CouchDocument.Dehydrate(entity.Key, identityProperty, entity.Value.Revision, false));
				entities.Add(entity.Key);
			}
			
			if (docs.Count == 0 && entities.Count == 0)
				return;
			
			var bulkDocsMessage = new BulkDocsMessage(docs);
			var bulkDocsCommand = new BulkDocsCommand(CouchDatabase.Name, bulkDocsMessage);
			
			BulkDocsResult[] results = CouchDatabase.CouchProxy.Execute<BulkDocsResult[]>(bulkDocsCommand);
			
			for (int index = 0; index < results.Length; index++)
			{
				BulkDocsResult result = results[index];
				object entity = entities[index];
				
				EntityMetadata entityMetadata;
				if (EntityMetadataMap.TryGetValue(entity, out entityMetadata) == false)
					continue;
				
				IdentityMap[result.Id] = entity;
				entityMetadata.Revision = result.Rev;
				entityMetadata.OriginalEntity = entity;
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
		
		private bool IsEntityDirty(object entity, EntityMetadata entityMetadata)
		{
			// TODO: Need to find a better way to tell if the entity is dirty
			//return entity.Equals(entityMetadata.OriginalEntity) == false;
			return true;
		}
		
		private T StalkEntity<T>(CouchDocument couchDocument) where T : new()
		{
			PropertyInfo identityProperty = CouchDatabase.CouchDocumentConvention.GetIdentityPropertyFor(typeof(T));
			T entity = couchDocument.Hydrate<T>(identityProperty);
			
			EntityMetadata entityMetadata = new EntityMetadata
			{
				Key = couchDocument["_id"].ToString(),
				Revision = couchDocument["_rev"].ToString(),
				OriginalEntity = entity
			};
			
			EntityMetadataMap[entity] = entityMetadata;
			IdentityMap[entityMetadata.Key] = entity;
			
			return entity;
		}
		
		private class EntityMetadata
		{
			public string Key { get; set; }
			public string Revision { get; set; }
			public object OriginalEntity { get; set; }
		}
	}
}
