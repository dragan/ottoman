using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace SineSignal.Ottoman.Serialization
{
	internal class JsonConvert
	{
		private static JsonWriter StaticJsonWriter { get; set; }
		private static readonly object StaticJsonWriterLock = new object();
		
		private static IDictionary<Type, Action<object, JsonWriter>> DefaultWriters { get; set; }
		private static IDictionary<Type, IDictionary<Type, Func<object, object>>> DefaultReaders { get; set; }
		
		private static readonly object implicitConversionOperatorsLock = new object();
		private static IDictionary<Type, IDictionary<Type, MethodInfo>> ImplicitConversionOperators { get; set; }
		
		private static readonly object arrayMetadataLock = new object();
		private static IDictionary<Type, ArrayMetadata> ArrayData { get; set; }
		
		private static readonly object objectMetadataLock = new object();
		private static IDictionary<Type, ObjectMetadata> ObjectData { get; set; }
		
		private static readonly object typePropertiesLock = new object();
		private static IDictionary<Type, IList<PropertyInfo>> propertyMetadata { get; set; }
		
		private static IFormatProvider datetimeFormat;
		
		static JsonConvert()
		{
			ArrayData = new Dictionary<Type, ArrayMetadata>();
			ImplicitConversionOperators = new Dictionary<Type, IDictionary<Type, MethodInfo>>();
			ObjectData = new Dictionary<Type, ObjectMetadata>();
			propertyMetadata = new Dictionary<Type, IList<PropertyInfo>>();
			
			StaticJsonWriter = new JsonWriter();
			
			DefaultWriters = new Dictionary<Type, Action<object, JsonWriter>>();
			DefaultReaders = new Dictionary<Type, IDictionary<Type, Func<object, object>>>();
			
			datetimeFormat = DateTimeFormatInfo.InvariantInfo;
			
			RegisterDefaultWriters();
			RegisterDefaultReaders();
		}
		
		public static string ToJson(object obj)
		{
			lock (StaticJsonWriterLock)
			{
				StaticJsonWriter.Reset();
				
				WriteValue(obj, StaticJsonWriter);
				
				return StaticJsonWriter.ToString();
			}
		}
		
		public static T ToObject<T>(string json)
		{
			var jsonReader = new JsonReader(json);
			
			return (T)ReadValue(typeof(T), jsonReader);
		}
		
		// TODO: Refactor JsonConvert.WriteValue, way too long for my taste
		private static void WriteValue(object obj, JsonWriter jsonWriter)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			
			if (obj is String)
			{
				jsonWriter.WriteString((string)obj);
				return;
			}
			
			if (obj is Int32)
			{
				jsonWriter.WriteNumber((int)obj);
				return;
			}
			
			if (obj is Int64)
			{
				jsonWriter.WriteNumber((long)obj);
				return;
			}
			
			if (obj is float)
			{
				jsonWriter.WriteNumber((float)obj);
				return;
			}
			
			if (obj is Double)
			{
				jsonWriter.WriteNumber((double)obj);
				return;
			}
			
			if (obj is Boolean)
			{
				jsonWriter.WriteBoolean((bool)obj);
				return;
			}
			
			if (obj is IDictionary)
			{
				jsonWriter.BeginObject();
				
				foreach (DictionaryEntry entry in (IDictionary)obj)
				{
					jsonWriter.WriteMember(entry.Key.ToString());
					WriteValue(entry.Value, jsonWriter);
				}
				
				jsonWriter.EndObject();
				return;
			}
			
			if (obj is ICollection)
			{
				jsonWriter.BeginArray();
				
				foreach (object item in (ICollection)obj)
				{
					WriteValue(item, jsonWriter);
				}
				
				jsonWriter.EndArray();
				
				return;
			}
			
			Type type = obj.GetType();
			
			if (DefaultWriters.ContainsKey(type))
			{
				Action<object, JsonWriter> action = DefaultWriters[type];
				action(obj, jsonWriter);
				
				return;
			}
			
			if (obj is Enum)
			{
				Type enumType = Enum.GetUnderlyingType(type);
				
				if (enumType == typeof(long) || 
					enumType == typeof(uint) || 
					enumType == typeof(ulong))
				{
					jsonWriter.WriteNumber((ulong)obj);
				}
				else
				{
					jsonWriter.WriteNumber((int)obj);
				}
				
				return;
            }
			
			// At this point obj must be a complex type
			GetPropertiesFor(type);
			
			jsonWriter.BeginObject();
				
			foreach (PropertyInfo propertyInfo in propertyMetadata[type])
			{	
				object propertyValue = propertyInfo.GetValue(obj, null);
				
				if (propertyValue != null)
				{
					string propertyName = GetPropertyName(propertyInfo);
					jsonWriter.WriteMember(propertyName);
					WriteValue(propertyValue, jsonWriter);
				}
			}
			
			jsonWriter.EndObject();
		}
		
		// TODO: Refactor JsonConvert.ReadValue, way too long for my taste
		private static object ReadValue(Type type, JsonReader jsonReader)
		{
			jsonReader.Read();
			
			if (jsonReader.CurrentToken == JsonToken.Double || 
				jsonReader.CurrentToken == JsonToken.Int || 
				jsonReader.CurrentToken == JsonToken.Long || 
				jsonReader.CurrentToken == JsonToken.String || 
				jsonReader.CurrentToken == JsonToken.Boolean)
			{
				Type jsonType = jsonReader.CurrentTokenValue.GetType();
				
				if (type.IsAssignableFrom(jsonType))
				{
					return jsonReader.CurrentTokenValue;
				}
				
				if (DefaultReaders.ContainsKey(jsonType) && 
					DefaultReaders[jsonType].ContainsKey(type))
				{
					Func<object, object> reader = DefaultReaders[jsonType][type];
					return reader(jsonReader.CurrentTokenValue);
				}
				
				if (type.IsEnum)
				{
					return Enum.ToObject(type, jsonReader.CurrentTokenValue);
				}
				
				MethodInfo implicitConversionOperator = GetImplicitConversionOperator(type, jsonType);
				
				if (implicitConversionOperator != null)
				{
					return implicitConversionOperator.Invoke (null, new object[] { jsonReader.CurrentTokenValue });
				}
				
				throw new JsonException(String.Format("Can't assign value '{0}' (type {1}) to type {2}", 
					jsonReader.CurrentTokenValue, jsonType, type));
			}
			
			object instance = null;
			
			if (jsonReader.CurrentToken == JsonToken.ArrayStart)
			{
				AddArrayMetadata(type);
				ArrayMetadata arratData = ArrayData[type];
				
				if (!arratData.IsArray && ! arratData.IsList)
					throw new JsonException(String.Format("Type {0} can't act as an array", type));
				
				IList list;
				Type elementType;
				
				if (!arratData.IsArray)
				{
					list = (IList)Activator.CreateInstance(type);
					elementType = arratData.ElementType;
				}
				else
				{
					list = new ArrayList();
					elementType = type.GetElementType();
				}
				
				while (true)
				{
					object item = ReadValue(elementType, jsonReader);
					if (jsonReader.CurrentToken == JsonToken.ArrayEnd)
						break;
					
					list.Add(item);
				}
				
				if (arratData.IsArray)
				{
					int n = list.Count;
					instance = Array.CreateInstance(elementType, n);
					
					for (int i = 0; i < n; i++)
					{
						((Array) instance).SetValue(list[i], i);
					}
				}
				else
				{
					instance = list;
				}
			}
			else if (jsonReader.CurrentToken == JsonToken.ObjectStart)
			{	
				AddObjectMetadata(type);
				ObjectMetadata objectData = ObjectData[type];
				
				instance = Activator.CreateInstance(type);
				
				while (true)
				{
					jsonReader.Read();
					
					if (jsonReader.CurrentToken == JsonToken.ObjectEnd)
						break;
					
					string property = (string)jsonReader.CurrentTokenValue;
					
					if (objectData.Properties.ContainsKey(property))
					{
						PropertyInfo propertyInfo = objectData.Properties[property];
							
						if (propertyInfo.CanWrite)
						{
							propertyInfo.SetValue(instance, ReadValue(propertyInfo.PropertyType, jsonReader), null);
						}
						else
						{
							ReadValue(propertyInfo.PropertyType, jsonReader);
						}
					}
					else
					{
						if (!objectData.IsDictionary)
							throw new JsonException(String.Format(
								"The type {0} doesn't have the property '{1}'", type, property));
						
						((IDictionary) instance).Add(property, ReadValue(objectData.ElementType, jsonReader));
					}	
				}
			}
			
			return instance;
		}
		
		private static MethodInfo GetImplicitConversionOperator(Type type1, Type type2)
		{
			lock (implicitConversionOperatorsLock)
			{
				if (! ImplicitConversionOperators.ContainsKey(type1))
					ImplicitConversionOperators.Add (type1, new Dictionary<Type, MethodInfo>());
			}
			
			if (ImplicitConversionOperators[type1].ContainsKey(type2))
				return ImplicitConversionOperators[type1][type2];
			
			MethodInfo implicitConversionOperator = type1.GetMethod("op_Implicit", new Type[] { type2 });
			
			lock (implicitConversionOperatorsLock)
			{
				try
				{
					ImplicitConversionOperators[type1].Add(type2, implicitConversionOperator);
				}
				catch (ArgumentException)
				{
					return ImplicitConversionOperators[type1][type2];
				}
			}
			
			return implicitConversionOperator;
		}
		
		private static void AddArrayMetadata(Type type)
		{
			if (ArrayData.ContainsKey(type))
				return;
			
			ArrayMetadata data = new ArrayMetadata();
			
			data.IsArray = type.IsArray;
			
			if (type.GetInterface("System.Collections.IList") != null)
				data.IsList = true;
			
			foreach (PropertyInfo p_info in type.GetProperties())
			{
				if (p_info.Name != "Item")
					continue;
				
				ParameterInfo[] parameters = p_info.GetIndexParameters();
				
				if (parameters.Length != 1)
					continue;
				
				if (parameters[0].ParameterType == typeof (int))
					data.ElementType = p_info.PropertyType;
			}
			
			lock (arrayMetadataLock)
			{
				try
				{
					ArrayData.Add(type, data);
				}
				catch (ArgumentException)
				{
					return;
				}
			}
		}
		
		private static void AddObjectMetadata(Type type)
		{
			if (ObjectData.ContainsKey(type))
				return;
			
			ObjectMetadata data = new ObjectMetadata();
			
			if (type.GetInterface("System.Collections.IDictionary") != null)
				data.IsDictionary = true;
			
			data.Properties = new Dictionary<string, PropertyInfo>();
			
			foreach (PropertyInfo propertyInfo in type.GetProperties())
			{
				string propertyName = GetPropertyName(propertyInfo);
				
				if (propertyName == "Item")
				{
					ParameterInfo[] parameters = propertyInfo.GetIndexParameters();
					
					if (parameters.Length != 1)
						continue;
					
					if (parameters[0].ParameterType == typeof(string))
						data.ElementType = propertyInfo.PropertyType;
					
					continue;
				}
				
				data.Properties.Add(propertyName, propertyInfo);
			}
			
			lock (objectMetadataLock)
			{
				try
				{
					ObjectData.Add(type, data);
				}
				catch (ArgumentException)
				{
					return;
				}
			}
		}
		
		private static void GetPropertiesFor(Type type)
		{
			if (propertyMetadata.ContainsKey(type))
				return;
			
			IList<PropertyInfo> properties = new List<PropertyInfo>();
			
			foreach (PropertyInfo propertyInfo in type.GetProperties())
			{
				if (propertyInfo.Name == "Item")
					continue;
				
				properties.Add(propertyInfo);
			}
			
			lock (typePropertiesLock)
			{
				try
				{
					propertyMetadata.Add(type, properties);
				}
				catch (ArgumentException)
				{
					return;
				}
			}
		}
		
		private static string GetPropertyName(PropertyInfo propertyInfo)
		{
			string propertyName = propertyInfo.Name;
			
			object[] jsonMemberAttributes = propertyInfo.GetCustomAttributes(typeof(JsonMemberAttribute), true);
			
			if (jsonMemberAttributes.Length == 1)
			{
				JsonMemberAttribute jsonMemberAttribute = jsonMemberAttributes[0] as JsonMemberAttribute;
				
				if (jsonMemberAttribute != null)
				{
					propertyName = jsonMemberAttribute.Name;
				}
			}
			
			return propertyName;
		}
		
		private static void RegisterDefaultWriters()
		{
			DefaultWriters[typeof(byte)] = (object obj, JsonWriter jsonWriter) => {
				jsonWriter.WriteNumber(Convert.ToInt32((byte)obj));
			};
			
			DefaultWriters[typeof(decimal)] = (object obj, JsonWriter jsonWriter) => {
				jsonWriter.WriteNumber((decimal)obj);
			};
			
			DefaultWriters[typeof(sbyte)] = (object obj, JsonWriter jsonWriter) => {
				jsonWriter.WriteNumber(Convert.ToInt32((sbyte)obj));
			};
			
			DefaultWriters[typeof(short)] = (object obj, JsonWriter jsonWriter) => {
				jsonWriter.WriteNumber(Convert.ToInt32((short)obj));
			};
			
			DefaultWriters[typeof(ushort)] = (object obj, JsonWriter jsonWriter) => {
				jsonWriter.WriteNumber(Convert.ToInt32((ushort)obj));
			};
			
			DefaultWriters[typeof(uint)] = (object obj, JsonWriter jsonWriter) => {
				jsonWriter.WriteNumber(Convert.ToUInt64((uint)obj));
			};
			
			DefaultWriters[typeof(ulong)] = (object obj, JsonWriter jsonWriter) => {
				jsonWriter.WriteNumber((ulong)obj);
			};
			
			DefaultWriters[typeof(char)] = (object obj, JsonWriter jsonWriter) => {
				jsonWriter.WriteString(Convert.ToString((char)obj));
			};
			
			DefaultWriters[typeof(DateTime)] = (object obj, JsonWriter jsonWriter) => {
				jsonWriter.WriteString(((DateTime)obj).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssK"));
			};
			
			DefaultWriters[typeof(Guid)] = (object obj, JsonWriter jsonWriter) => {
				jsonWriter.WriteString(Convert.ToString((Guid)obj));
			};
		}
		
		private static void RegisterDefaultReaders()
		{
			Func<object, object> reader;
			
			reader = (object input) => {
				return Convert.ToByte((int)input);
			};
			RegisterReader(DefaultReaders, typeof(int), typeof(byte), reader);
			
			reader = (object input) => {
				return Convert.ToUInt64((int)input);
			};
			RegisterReader(DefaultReaders, typeof(int), typeof(ulong), reader);
			
			reader = (object input) => {
				return Convert.ToSByte((int)input);
			};
			RegisterReader(DefaultReaders, typeof(int), typeof(sbyte), reader);
			
			reader = (object input) => {
				return Convert.ToInt16((int)input);
			};
			RegisterReader(DefaultReaders, typeof(int), typeof(short), reader);
			
			reader = (object input) => {
				return Convert.ToUInt16((int)input);
			};
			RegisterReader(DefaultReaders, typeof(int), typeof(ushort), reader);
			
			reader = (object input) => {
				return Convert.ToUInt32((int)input);
			};
			RegisterReader(DefaultReaders, typeof(int), typeof(uint), reader);
			
			reader = (object input) => {
				return Convert.ToSingle((int)input);
			};
			RegisterReader(DefaultReaders, typeof(int), typeof(float), reader);
			
			reader = (object input) => {
				return Convert.ToDouble((int)input);
			};
			RegisterReader(DefaultReaders, typeof(int), typeof(double), reader);
			
			reader = (object input) => {
				return Convert.ToDecimal((double)input);
			};
			RegisterReader(DefaultReaders, typeof(double), typeof(decimal), reader);
			
			reader = (object input) => {
				return Convert.ToUInt32((long)input);
			};
			RegisterReader(DefaultReaders, typeof(long), typeof(uint), reader);
			
			reader = (object input) => {
				return Convert.ToChar((string)input);
			};
			RegisterReader(DefaultReaders, typeof(string), typeof(char), reader);
			
			reader = (object input) => {
				return new Guid(input.ToString());
			};
			RegisterReader(DefaultReaders, typeof(string), typeof(Guid), reader);
			
			reader = (object input) => {
				return Convert.ToDateTime((string)input, datetimeFormat);
			};
			RegisterReader(DefaultReaders, typeof(string), typeof(DateTime), reader);
		}
				
		private static void RegisterReader(IDictionary<Type, IDictionary<Type, Func<object, object>>> table, Type jsonType, Type valueType, Func<object, object> reader)
		{
			if (!table.ContainsKey(jsonType))
			{
				table.Add(jsonType, new Dictionary<Type, Func<object, object>>());
			}
			
			table[jsonType][valueType] = reader;
		}
		
		internal struct ArrayMetadata
		{
			public Type ElementType { get; set; }
			public bool IsArray { get; set; }
			public bool IsList { get; set; }
		}
		
		internal struct ObjectMetadata
		{
			public Type ElementType { get; set; }
			public bool IsDictionary { get; set; }
			public IDictionary<string, PropertyInfo> Properties { get; set; }
		}
	}
}
