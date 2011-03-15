using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace SineSignal.Ottoman
{
	// TODO: When we target 4.0 instead of 3.5, change this to inherit off of DynamicObject instead of Dictionary
	public class CouchDocument : Dictionary<string, object>
	{
		public const string ID_KEY = "_id";
		public const string TYPE_KEY = "Type";
		
		public T HydrateEntity<T>(PropertyInfo identityProperty)
		{
			var identityPropertyMap = new IdentityPropertyMap { IdentityPropertyKey = ID_KEY, IdentityProperty = identityProperty };
			
			object entity;
			if (!EntityHydrator.Get(typeof(T)).TryHydrateEntity(this, identityPropertyMap, out entity))
				return default(T);
			
			return (T)entity;
		}
	}
	
	internal class IdentityPropertyMap
	{
		public string IdentityPropertyKey { get; set; }
		public PropertyInfo IdentityProperty { get; set; }
	}
	
	// TODO: Move into it's own file
	internal class EntityHydrator
	{
		private static readonly Dictionary<Type, EntityHydrator> cache = new Dictionary<Type, EntityHydrator>();
		private readonly Type entityType;
		
		private EntityHydrator(Type entityType)
		{
			this.entityType = entityType;
		}
		
		public static EntityHydrator Get(Type entityType)
		{
			EntityHydrator entityHydrator;
			if (!cache.TryGetValue(entityType, out entityHydrator))
			{
				entityHydrator = new EntityHydrator(entityType);
				cache.Add(entityType, entityHydrator);
			}
			
			return entityHydrator;
		}
		
		public bool TryHydrateEntity(IDictionary<string, object> entityValues, IdentityPropertyMap identityPropertyMap, out object entity)
		{
			bool propertiesWereSet = false;
			object obj = Activator.CreateInstance(entityType);
			
			foreach (var propertyInfo in entityType.GetProperties())
			{
				HydrateProperty(obj, identityPropertyMap, propertyInfo, entityValues);
				propertiesWereSet = true;
			}
			
			entity = propertiesWereSet ? obj : null;
			
			return propertiesWereSet;
		}
		
		private static void HydrateProperty(object entity, IdentityPropertyMap identityPropertyMap, PropertyInfo propertyInfo, IDictionary<string, object> entityValues)
		{
			object propertyValue;
			
			if (identityPropertyMap.IdentityProperty == null || propertyInfo.Name != identityPropertyMap.IdentityProperty.Name)
			{
				propertyValue = entityValues[propertyInfo.Name];
				
				var subData = propertyValue as IDictionary<string, object>;
				if (subData != null && !EntityHydrator.Get(propertyInfo.PropertyType).TryHydrateEntity(subData, identityPropertyMap, out propertyValue))
					return;
			}
			else
			{
				propertyValue = entityValues[identityPropertyMap.IdentityPropertyKey];
			}
			
			SetProperty(entity, propertyInfo, propertyValue);
		}
		
		private static void SetProperty(object entity, PropertyInfo propertyInfo, object propertyValue)
		{
			// TODO: Build a cache for TypeDescriptors and PropertyInfo
			// TODO: Do we need to abstract TypeConverter?
			TypeConverter typeConverter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
			if (typeConverter.CanConvertFrom(propertyValue.GetType()))
			{
				propertyInfo.SetValue(entity, typeConverter.ConvertFrom(propertyValue), null);
			}
			else
			{
				propertyInfo.SetValue(entity, propertyValue, null);
			}
		}
	}
}
