using System;
using System.Collections.Generic;

using NSubstitute;
using NUnit.Framework;
using SineSignal.Ottoman.Specs.Framework;

using SineSignal.Ottoman.Serialization;

namespace SineSignal.Ottoman.Specs.Serialization
{
	public class JsonSerializerSpecs
	{
		public abstract class JsonSerializerConcern : ConcernFor<JsonSerializer>
		{
			public override JsonSerializer CreateSystemUnderTest()
			{
				return new JsonSerializer();
			}
		}
		
		public class When_serializing_an_array : JsonSerializerConcern
		{
			private string[] val;
			private string output;
			
			protected override void Given()
			{
				val = new string[] { "One", "Two", "Three", "Four", "Five" };
			}

			protected override void When()
			{
				output = Sut.Serialize(val);
			}
			
			[Test]
			public void Should_write_out_array_items_in_JSON_format()
			{
				Assert.That(output, Is.EqualTo("[\"One\",\"Two\",\"Three\",\"Four\",\"Five\"]"));
			}
		}
		
		public class When_serializing_a_dictionary : JsonSerializerConcern
		{
			private IDictionary<string,int> val;
			private string output;
			
			protected override void Given()
			{
				val = new Dictionary<string,int>();
				val.Add("One", 1);
				val.Add("Two", 2);
				val.Add("Three", 3);
				val.Add("Four", 4);
				val.Add("Five", 5);
			}

			protected override void When()
			{
				output = Sut.Serialize(val);
			}
			
			[Test]
			public void Should_write_out_dictionary_items_in_JSON_format()
			{
				Assert.That(output, Is.EqualTo("{\"One\":1,\"Two\":2,\"Three\":3,\"Four\":4,\"Five\":5}"));
			}
		}
		
		public class When_serializing_a_complex_type : JsonSerializerConcern
		{
			private ComplexType val;
			private DateTime now;
			private DateTime independenceDay;
			private Guid newGuid;
			private string output;
			
			protected override void Given()
			{
				now = DateTime.Now;
				independenceDay = new DateTime(1776, 7, 4);
				newGuid = Guid.NewGuid();
				val = new ComplexType
				{
					Property1 = "One",
					Property2 = false,
					Property3 = 1,
					Property4 = 1.5F,
					Property5 = 1.75,
					Property6 = now,
					Property7 = independenceDay,
					Property8 = newGuid,
					Property9 = null,
					Property10 = new AnotherComplexType { Property1 = "Two", Property2 = 2 }
				};
			}

			protected override void When()
			{
				output = Sut.Serialize(val);
			}
			
			[Test]
			public void Should_write_out_complex_type_in_JSON_format()
			{
				Assert.That(output, Is.EqualTo("{\"Property1\":\"One\",\"Property2\":false,\"Property3\":1,\"Property4\":1.5,\"Property5\":1.75,\"Property6\":\"" + now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssK") + "\",\"Property7\":\"" + independenceDay.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssK") + "\",\"Property8\":\"" + newGuid + "\",\"Property10\":{\"Property1\":\"Two\",\"Property2\":2}}"));
			}
		}
		
		public class When_serializing_a_complex_type_with_a_custom_json_member_attribute : JsonSerializerConcern
		{
			private ComplexTypeWithCustomProperty complexType;
			private string output;
			
			protected override void Given()
			{
				complexType = new ComplexTypeWithCustomProperty { Property1 = 10 };
			}
			
			protected override void When()
			{
				output = Sut.Serialize(complexType);
			}
			
			[Test]
			public void Should_use_custom_property_name_instead_of_property_name_when_writing()
			{
				Assert.That(output, Is.EqualTo("{\"property_one\":10}"));
			}
		}
		
		public class When_deserializing_a_complex_type : JsonSerializerConcern
		{
			private DateTime now;
			private DateTime independenceDay;
			private Guid newGuid;
			private string input;
			private ComplexType output;
			
			protected override void Given()
			{
				now = DateTime.Now;
				independenceDay = new DateTime(1776, 7, 4);
				newGuid = Guid.NewGuid();
				
				input = "{\"Property1\":\"One\",\"Property2\":false,\"Property3\":1,\"Property4\":1.5,\"Property5\":1.75,\"Property6\":\"" + now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssK") + "\",\"Property7\":\"" + independenceDay.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssK") + "\",\"Property8\":\"" + newGuid + "\",\"Property10\":{\"Property1\":\"Two\",\"Property2\":2}}";
			}

			protected override void When()
			{
				output = Sut.Deserialize<ComplexType>(input);
			}
			
			[Test]
			public void Should_create_an_instance_of_our_complex_type_from_JSON_string()
			{
				Assert.That(output, Is.Not.Null);
			}
			
			[Test]
			public void Should_set_all_the_complex_types_properties_from_JSON_string()
			{
				Assert.That(output.Property1, Is.EqualTo("One"));
				Assert.That(output.Property2, Is.False);
				Assert.That(output.Property3, Is.EqualTo(1));
				Assert.That(output.Property4, Is.EqualTo(1.5F));
				Assert.That(output.Property5, Is.EqualTo(1.75));
				Assert.That(output.Property6.ToString(), Is.EqualTo(now.ToString()));
				Assert.That(output.Property7, Is.EqualTo(independenceDay));
				Assert.That(output.Property8, Is.EqualTo(newGuid));
				Assert.That(output.Property9, Is.Null);
				Assert.That(output.Property10.Property1, Is.EqualTo("Two"));
				Assert.That(output.Property10.Property2, Is.EqualTo(2));
			}
		}
		
		// TODO: Uncomment and get this test passing by adding logic to read the member attribute if one exists.
//		public class When_deserializing_a_complex_type_with_a_custom_json_member_attribute : JsonSerializerConcern
//		{
//			private string input;
//			private ComplexTypeWithCustomProperty output;
//			
//			protected override void Given()
//			{
//				input = "{\"property_one\":10}";
//			}
//			
//			protected override void When()
//			{
//				output = Sut.Deserialize<ComplexTypeWithCustomProperty>(input);
//			}
//			
//			[Test]
//			public void Should_use_custom_property_name_instead_of_property_name_when_reading()
//			{
//				Assert.That(output.Property1, Is.EqualTo(10));
//			}
//		}
	}
	
	public class ComplexType
	{
		public string Property1 { get; set; }
		public bool Property2 { get; set; }
		public int Property3 { get; set; }
		public double Property4 { get; set; }
		public double Property5 { get; set; }
		public DateTime Property6 { get; set; }
		public DateTime Property7 { get; set; }
		public Guid Property8 { get; set; }
		public string Property9 { get; set; }
		public AnotherComplexType Property10 { get; set; }
	}
	
	public class AnotherComplexType
	{
		public string Property1 { get; set; }
		public int Property2 { get; set; }
	}
	
	public class ComplexTypeWithCustomProperty
	{
		[JsonMember("property_one")]
		public int Property1 { get; set; }
	}
}
