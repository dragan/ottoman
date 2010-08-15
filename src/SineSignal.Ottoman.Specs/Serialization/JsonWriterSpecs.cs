using System;

using NSubstitute;
using NUnit.Framework;
using SineSignal.Ottoman.Specs.Framework;

using SineSignal.Ottoman.Serialization;

namespace SineSignal.Ottoman.Specs.Serialization
{
	public class JsonWriterSpecs
	{
		public class JsonWriterConcern : ConcernFor<JsonWriter>
		{
			public override JsonWriter CreateSystemUnderTest()
			{
				return new JsonWriter();
			}
		}
		
		public class When_writing_an_object_with_a_single_member : JsonWriterConcern
		{
			private string name1;
			private bool value1;
			private string output;
			
			protected override void Given()
			{
				name1 = "name1";
				value1 = true;
				output = "{\"name1\":true}";
			}
			
			protected override void When()
			{
				Sut.BeginObject();
				Sut.WriteMember(name1);
				Sut.WriteBoolean(value1);
				Sut.EndObject();
			}
			
			[Test]
			public void Should_output_string_in_valid_JSON_object_format()
			{
				Assert.That(Sut.ToString(), Is.EqualTo(output));
			}
		}
		
		public class When_writing_an_object_with_multiple_members : JsonWriterConcern
		{
			private string name1;
			private bool value1;
			private string name2;
			private bool value2;
			private string name3;
			private string value3;
			private string name4;
			private int value4;
			private string name5;
			private float value5;
			private string name6;
			private double value6;
			private string name7;
			private decimal value7;
			private string output;
			
			protected override void Given()
			{
				name1 = "name1";
				value1 = true;
				name2 = "name2";
				value2 = false;
				name3 = "name3";
				value3 = "hello";
				name4 = "name4";
				value4 = 10;
				name5 = "name5";
				value5 = 10.25F;
				name6 = "name6";
				value6 = 10.5;
				name7 = "name7";
				value7 = 10.75m;
				output = "{\"name1\":true,\"name2\":false,\"name3\":\"hello\",\"name4\":10,\"name5\":10.25,\"name6\":10.5,\"name7\":10.75}";
			}
			
			protected override void When()
			{
				Sut.BeginObject();
				Sut.WriteMember(name1);
				Sut.WriteBoolean(value1);
				Sut.WriteMember(name2);
				Sut.WriteBoolean(value2);
				Sut.WriteMember(name3);
				Sut.WriteString(value3);
				Sut.WriteMember(name4);
				Sut.WriteNumber(value4);
				Sut.WriteMember(name5);
				Sut.WriteNumber(value5);
				Sut.WriteMember(name6);
				Sut.WriteNumber(value6);
				Sut.WriteMember(name7);
				Sut.WriteNumber(value7);
				Sut.EndObject();
			}
			
			[Test]
			public void Should_output_string_in_valid_JSON_object_format()
			{
				Assert.That(Sut.ToString(), Is.EqualTo(output));
			}
		}
		
		public class When_writing_an_object_with_a_string_value_containing_special_characters : JsonWriterConcern
		{
			private string name1;
			private string value1;
			private string output;
			
			protected override void Given()
			{
				name1 = "name1";
				value1 = "\"h\\e\rl\nl\to w\bo\frld";
				output = "{\"name1\":\"\\\"h\\\\e\\rl\\nl\\to w\\bo\\frld\"}";
			}
			
			protected override void When()
			{
				Sut.BeginObject();
				Sut.WriteMember(name1);
				Sut.WriteString(value1);
				Sut.EndObject();
			}
			
			[Test]
			public void Should_output_string_in_valid_JSON_object_format()
			{
				Assert.That(Sut.ToString(), Is.EqualTo(output));
			}
		}
		
		public class When_writing_an_object_with_a_string_value_containing_unicode_characters : JsonWriterConcern
		{
			private string name1;
			private string value1;
			private string output;
			
			protected override void Given()
			{
				name1 = "name1";
				value1 = "These are unicode characters: Ç σ";
				output = "{\"name1\":\"These are unicode characters: \\u00C7 \\u03C3\"}";
			}
			
			protected override void When()
			{
				Sut.BeginObject();
				Sut.WriteMember(name1);
				Sut.WriteString(value1);
				Sut.EndObject();
			}
			
			[Test]
			public void Should_output_string_in_valid_JSON_object_format()
			{
				Assert.That(Sut.ToString(), Is.EqualTo(output));
			}
		}
		
		public class When_nesting_an_object_inside_an_object : JsonWriterConcern
		{
			private string name1;
			private bool value1;
			private string name2;
			private bool value2;
			private string name3;
			private bool value3;
			private string output;
			
			protected override void Given()
			{
				name1 = "name1";
				// value1 for name1
				name2 = "name2";
				value2 = true;
				name3 = "name3";
				value3 = false;
				output = "{\"name1\":{\"name2\":true,\"name3\":false}}";
			}
			
			protected override void When()
			{
				Sut.BeginObject();
				Sut.WriteMember(name1);
				Sut.BeginObject();
				Sut.WriteMember(name2);
				Sut.WriteBoolean(value2);
				Sut.WriteMember(name3);
				Sut.WriteBoolean(value3);
				Sut.EndObject();
				Sut.EndObject();
			}
			
			[Test]
			public void Should_output_string_in_valid_JSON_object_format()
			{
				Assert.That(Sut.ToString(), Is.EqualTo(output));
			}
		}
		
		public class When_writing_an_array_with_a_single_value : JsonWriterConcern
		{
			private bool value1;
			private string output;
			
			protected override void Given()
			{
				value1 = true;
				output = "[true]";
			}
			
			protected override void When()
			{
				Sut.BeginArray();
				Sut.WriteBoolean(value1);
				Sut.EndArray();
			}
			
			[Test]
			public void Should_output_string_in_valid_JSON_array_format()
			{
				Assert.That(Sut.ToString(), Is.EqualTo(output));
			}
		}
		
		public class When_writing_an_array_with_multiple_values : JsonWriterConcern
		{
			private bool value1;
			private bool value2;
			private string value3;
			private int value4;
			private float value5;
			private double value6;
			private decimal value7;
			private string output;
			
			protected override void Given()
			{
				value1 = true;
				value2 = false;
				value3 = "hello";
				value4 = 10;
				value5 = 10.25F;
				value6 = 10.5;
				value7 = 10.75m;
				output = "[true,false,\"hello\",10,10.25,10.5,10.75]";
			}
			
			protected override void When()
			{
				Sut.BeginArray();
				Sut.WriteBoolean(value1);
				Sut.WriteBoolean(value2);
				Sut.WriteString(value3);
				Sut.WriteNumber(value4);
				Sut.WriteNumber(value5);
				Sut.WriteNumber(value6);
				Sut.WriteNumber(value7);
				Sut.EndArray();
			}
			
			[Test]
			public void Should_output_string_in_valid_JSON_object_format()
			{
				Assert.That(Sut.ToString(), Is.EqualTo(output));
			}
		}
		
		public class When_writing_an_array_with_a_string_value_containing_special_characters : JsonWriterConcern
		{
			private string value1;
			private string output;
			
			protected override void Given()
			{
				value1 = "\"h\\e\rl\nl\to w\bo\frld";
				output = "[\"\\\"h\\\\e\\rl\\nl\\to w\\bo\\frld\"]";
			}
			
			protected override void When()
			{
				Sut.BeginArray();
				Sut.WriteString(value1);
				Sut.EndArray();
			}
			
			[Test]
			public void Should_output_string_in_valid_JSON_object_format()
			{
				Assert.That(Sut.ToString(), Is.EqualTo(output));
			}
		}
		
		public class When_writing_an_array_with_a_string_value_containing_unicode_characters : JsonWriterConcern
		{
			private string value1;
			private string output;
			
			protected override void Given()
			{
				value1 = "These are unicode characters: Ç σ";
				output = "[\"These are unicode characters: \\u00C7 \\u03C3\"]";
			}
			
			protected override void When()
			{
				Sut.BeginArray();
				Sut.WriteString(value1);
				Sut.EndArray();
			}
			
			[Test]
			public void Should_output_string_in_valid_JSON_object_format()
			{
				Assert.That(Sut.ToString(), Is.EqualTo(output));
			}
		}
		
		public class When_nesting_an_array_inside_an_array : JsonWriterConcern
		{
			private bool value1;
			private bool value2;
			private bool value3;
			private string output;
			
			protected override void Given()
			{
				value1 = true;
				value2 = true;
				value3 = false;
				output = "[true,[true,false]]";
			}
			
			protected override void When()
			{
				Sut.BeginArray();
				Sut.WriteBoolean(value1);
				Sut.BeginArray();
				Sut.WriteBoolean(value2);
				Sut.WriteBoolean(value3);
				Sut.EndArray();
				Sut.EndArray();
			}
			
			[Test]
			public void Should_output_string_in_valid_JSON_array_format()
			{
				Assert.That(Sut.ToString(), Is.EqualTo(output));
			}
		}
		
		public class When_nesting_a_single_object_inside_an_array : JsonWriterConcern
		{
			private string name1;
			private bool value1;
			private string output;
			
			protected override void Given()
			{
				name1 = "name1";
				value1 = true;
				output = "[{\"name1\":true}]";
			}
			
			protected override void When()
			{
				Sut.BeginArray();
				Sut.BeginObject();
				Sut.WriteMember(name1);
				Sut.WriteBoolean(value1);
				Sut.EndObject();
				Sut.EndArray();
			}
			
			[Test]
			public void Should_output_string_in_valid_JSON_array_format()
			{
				Assert.That(Sut.ToString(), Is.EqualTo(output));
			}
		}
		
		public class When_nesting_multiple_objects_inside_an_array : JsonWriterConcern
		{
			private string name1;
			private bool value1;
			private string name2;
			private bool value2;
			private string name3;
			private bool value3;
			private string output;
			
			protected override void Given()
			{
				name1 = "name1";
				value1 = true;
				name2 = "name2";
				value2 = true;
				name3 = "name3";
				value3 = false;
				output = "[{\"name1\":true},{\"name2\":true,\"name3\":false}]";
			}
			
			protected override void When()
			{
				Sut.BeginArray();
				Sut.BeginObject();
				Sut.WriteMember(name1);
				Sut.WriteBoolean(value1);
				Sut.EndObject();
				Sut.BeginObject();
				Sut.WriteMember(name2);
				Sut.WriteBoolean(value2);
				Sut.WriteMember(name3);
				Sut.WriteBoolean(value3);
				Sut.EndObject();
				Sut.EndArray();
			}
			
			[Test]
			public void Should_output_string_in_valid_JSON_array_format()
			{
				Assert.That(Sut.ToString(), Is.EqualTo(output));
			}
		}
		
		public class When_nesting_a_single_array_inside_an_object : JsonWriterConcern
		{
			private string name1;
			private bool arrayValue1;
			private bool arrayValue2;
			private string output;
			
			protected override void Given()
			{
				name1 = "name1";
				arrayValue1 = true;
				arrayValue2 = false;
				output = "{\"name1\":[true,false]}";
			}
			
			protected override void When()
			{
				Sut.BeginObject();
				Sut.WriteMember(name1);
				Sut.BeginArray();
				Sut.WriteBoolean(arrayValue1);
				Sut.WriteBoolean(arrayValue2);
				Sut.EndArray();
				Sut.EndObject();
			}
			
			[Test]
			public void Should_output_string_in_valid_JSON_object_format()
			{
				Assert.That(Sut.ToString(), Is.EqualTo(output));
			}
		}
		
		public class When_nesting_multiple_arrays_inside_an_object : JsonWriterConcern
		{
			private string name1;
			private bool array1Value1;
			private bool array1Value2;
			private string name2;
			private string array2Value1;
			private string array2Value2;
			private string output;
			
			protected override void Given()
			{
				name1 = "name1";
				array1Value1 = true;
				array1Value2 = false;
				name2 = "name2";
				array2Value1 = "foo";
				array2Value2 = "bar";
				output = "{\"name1\":[true,false],\"name2\":[\"foo\",\"bar\"]}";
			}
			
			protected override void When()
			{
				Sut.BeginObject();
				Sut.WriteMember(name1);
				Sut.BeginArray();
				Sut.WriteBoolean(array1Value1);
				Sut.WriteBoolean(array1Value2);
				Sut.EndArray();
				Sut.WriteMember(name2);
				Sut.BeginArray();
				Sut.WriteString(array2Value1);
				Sut.WriteString(array2Value2);
				Sut.EndArray();
				Sut.EndObject();
			}
			
			[Test]
			public void Should_output_string_in_valid_JSON_object_format()
			{
				Assert.That(Sut.ToString(), Is.EqualTo(output));
			}
		}
	}
}
