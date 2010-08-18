using System;

using NSubstitute;
using NUnit.Framework;
using SineSignal.Ottoman.Specs.Framework;

using SineSignal.Ottoman.Serialization;

namespace SineSignal.Ottoman.Specs.Serialization.JsonReaderSpecs
{
	public class ReadingObjectsSpecs
	{
		public class When_reading_a_json_string_that_contains_an_object_with_a_single_member : ConcernFor<JsonReader>
		{
			private string input;
			
			protected override void Given()
			{
				input = "{\"name1\":true}";
			}
			
			public override JsonReader CreateSystemUnderTest()
			{
				return new JsonReader(input);
			}
			
			public class And_when_calling_read_once : When_reading_a_json_string_that_contains_an_object_with_a_single_member
			{
				protected override void When()
				{
					Sut.Read();
				}
				
				[Test]
				public void Should_set_current_token_to_object_start()
				{
					Assert.That(Sut.CurrentToken, Is.EqualTo(JsonToken.ObjectStart));
				}
			}
			
			public class And_when_calling_read_twice : When_reading_a_json_string_that_contains_an_object_with_a_single_member
			{
				protected override void When()
				{
					Sut.Read();
					Sut.Read();
				}
				
				[Test]
				public void Should_set_current_token_to_member_name()
				{
					Assert.That(Sut.CurrentToken, Is.EqualTo(JsonToken.MemberName));
				}
				
				[Test]
				public void Should_set_current_token_value_to_name1()
				{
					Assert.That(Sut.CurrentTokenValue, Is.EqualTo("name1"));
				}
			}
			
			public class And_when_calling_read_three_times : When_reading_a_json_string_that_contains_an_object_with_a_single_member
			{
				protected override void When()
				{
					for (int index = 1; index <= 3; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_current_token_to_json_boolean()
				{
					Assert.That(Sut.CurrentToken, Is.EqualTo(JsonToken.Boolean));
				}
				
				[Test]
				public void Should_set_current_token_value_to_true()
				{
					Assert.That(Sut.CurrentTokenValue, Is.True);
				}
			}
			
			public class And_when_calling_read_four_times : When_reading_a_json_string_that_contains_an_object_with_a_single_member
			{
				protected override void When()
				{
					for (int index = 1; index <= 4; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_current_token_to_object_end()
				{
					Assert.That(Sut.CurrentToken, Is.EqualTo(JsonToken.ObjectEnd));
				}
			}
			
			public class And_when_calling_read_five_times : When_reading_a_json_string_that_contains_an_object_with_a_single_member
			{
				protected override void When()
				{
					for (int index = 1; index <= 5; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_end_of_json_to_true()
				{
					Assert.That(Sut.EndOfJson, Is.True);
				}
			}
		}
		
		public class When_reading_a_json_string_that_contains_an_object_with_two_members_and_comments : ConcernFor<JsonReader>
		{
			private string input;
			
			protected override void Given()
			{
				input = @"
                {
                    // This is a single-line comment
                    ""name1"" : ""One"",

                    /**
                     * This is a multi-line another comment
                     **/
                     ""name2"": ""Two""
                }";
			}
			
			public override JsonReader CreateSystemUnderTest()
			{
				var jsonReader = new JsonReader(input);
				jsonReader.AllowComments = true;
				return jsonReader;
			}
			
			public class And_when_calling_read_twice : When_reading_a_json_string_that_contains_an_object_with_two_members_and_comments
			{
				protected override void When()
				{
					Sut.Read();
					Sut.Read();
				}
				
				[Test]
				public void Should_set_current_token_to_member_name()
				{
					Assert.That(Sut.CurrentToken, Is.EqualTo(JsonToken.MemberName));
				}
				
				[Test]
				public void Should_set_current_token_value_to_name1()
				{
					Assert.That(Sut.CurrentTokenValue, Is.EqualTo("name1"));
				}
			}
			
			public class And_when_calling_read_three_times : When_reading_a_json_string_that_contains_an_object_with_two_members_and_comments
			{
				protected override void When()
				{
					for (int index = 1; index <= 3; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_current_token_to_json_string()
				{
					Assert.That(Sut.CurrentToken, Is.EqualTo(JsonToken.String));
				}
				
				[Test]
				public void Should_set_current_token_value_to_One()
				{
					Assert.That(Sut.CurrentTokenValue, Is.EqualTo("One"));
				}
			}
			
			public class And_when_calling_read_four_times : When_reading_a_json_string_that_contains_an_object_with_two_members_and_comments
			{
				protected override void When()
				{
					for (int index = 1; index <= 4; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_current_token_to_member_name()
				{
					Assert.That(Sut.CurrentToken, Is.EqualTo(JsonToken.MemberName));
				}
				
				[Test]
				public void Should_set_current_token_value_to_name2()
				{
					Assert.That(Sut.CurrentTokenValue, Is.EqualTo("name2"));
				}
			}
			
			public class And_when_calling_read_five_times : When_reading_a_json_string_that_contains_an_object_with_two_members_and_comments
			{
				protected override void When()
				{
					for (int index = 1; index <= 5; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_current_token_to_json_string()
				{
					Assert.That(Sut.CurrentToken, Is.EqualTo(JsonToken.String));
				}
				
				[Test]
				public void Should_set_current_token_value_to_Two()
				{
					Assert.That(Sut.CurrentTokenValue, Is.EqualTo("Two"));
				}
			}
			
			public class And_when_calling_read_seven_times : When_reading_a_json_string_that_contains_an_object_with_two_members_and_comments
			{
				protected override void When()
				{
					for (int index = 1; index <= 7; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_end_of_json_to_true()
				{
					Assert.That(Sut.EndOfJson, Is.True);
				}
			}
		}
		
		public class When_reading_a_json_string_with_nested_objects : ConcernFor<JsonReader>
		{
			private string input;
			private int objectCount;
			
			protected override void Given()
			{
				input = "{ \"name1\" : { \"name2\": { \"name3\": true } } }";
				objectCount = 0;
			}
			
			public override JsonReader CreateSystemUnderTest()
			{
				return new JsonReader(input);
			}
			
			protected override void When()
			{
				while (Sut.Read())
				{
					if (Sut.CurrentToken == JsonToken.ObjectStart)
					{
						objectCount++;
					}
				}
			}
			
			[Test]
			public void Should_count_three_objects()
			{
				Assert.That(objectCount, Is.EqualTo(3));
			}
		}
		
		public class When_reading_a_json_string_that_contains_an_object_with_the_wrong_closing_token : ConcernFor<JsonReader>
		{
			private string input;
			private JsonException expectedException;
			
			protected override void Given()
			{
				input = "{ \"name1\" : [ \"name2\", \"name3\", \"name4\" ] ]";
			}
			
			public override JsonReader CreateSystemUnderTest()
			{
				return new JsonReader(input);
			}
			
			protected override void When()
			{
				try
				{
					while (Sut.Read());
				}
				catch (JsonException e)
				{
					expectedException = e;
				}
			}
			
			[Test]
			public void Should_throw_a_json_exception()
			{
				Assert.That(expectedException, Is.TypeOf(typeof(JsonException)));
			}
			
			[Test]
			public void Should_set_message_to_Invalid_token_93_in_input_string()
			{
				Assert.That(expectedException.Message, Is.EqualTo("Invalid token '93' in input string"));
			}
		}
		
		public class When_reading_a_json_string_that_contains_an_object_with_no_closing_token : ConcernFor<JsonReader>
		{
			private string input;
			private JsonException expectedException;
			
			protected override void Given()
			{
				input = "{ \"name1\" : true ";
			}
			
			public override JsonReader CreateSystemUnderTest()
			{
				return new JsonReader(input);
			}
			
			protected override void When()
			{
				try
				{
					while (Sut.Read());
				}
				catch (JsonException e)
				{
					expectedException = e;
				}
			}
			
			[Test]
			public void Should_throw_a_json_exception()
			{
				Assert.That(expectedException, Is.TypeOf(typeof(JsonException)));
			}
			
			[Test]
			public void Should_set_message_to_Input_does_not_evaluate_to_proper_JSON_text()
			{
				Assert.That(expectedException.Message, Is.EqualTo("Input doesn't evaluate to proper JSON text"));
			}
		}
		
		public class When_reading_a_json_string_that_contains_an_object_without_a_member_name : ConcernFor<JsonReader>
		{
			private string input;
			private JsonException expectedException;
			
			protected override void Given()
			{
				input = "{ {\"name1\": true} }";
			}
			
			public override JsonReader CreateSystemUnderTest()
			{
				return new JsonReader(input);
			}
			
			protected override void When()
			{
				try
				{
					while (Sut.Read());
				}
				catch (JsonException e)
				{
					expectedException = e;
				}
			}
			
			[Test]
			public void Should_throw_a_json_exception()
			{
				Assert.That(expectedException, Is.TypeOf(typeof(JsonException)));
			}
			
			[Test]
			public void Should_set_message_to_Invalid_token_123_in_input_string()
			{
				Assert.That(expectedException.Message, Is.EqualTo("Invalid token '123' in input string"));
			}
		}
	}
}
