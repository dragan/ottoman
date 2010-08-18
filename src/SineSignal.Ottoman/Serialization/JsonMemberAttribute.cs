using System;

namespace SineSignal.Ottoman.Serialization
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class JsonMemberAttribute : Attribute
	{
		public string Name { get; private set; }
		
		public JsonMemberAttribute(string name)
		{
			Name = name;
		}
	}
}
