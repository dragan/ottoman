using System;

using NSubstitute;
using NUnit.Framework;
using SineSignal.Ottoman.Specs.Framework;

using SineSignal.Ottoman.Serialization;

namespace SineSignal.Ottoman.Specs.Serialization.JsonReaderSpecs
{
	public class ReadingArraySpecs
	{
		public class When_reading_a_json_string_that_contains_an_array_with_an_empty_string : ConcernFor<JsonReader>
		{
			private string input;
			
			protected override void Given()
			{
				input = "[ \"\" ]";
			}
			
			public override JsonReader CreateSystemUnderTest()
			{
				return new JsonReader(input);
			}
			
			public class And_when_calling_read_twice : When_reading_a_json_string_that_contains_an_array_with_an_empty_string
			{
				protected override void When()
				{
					Sut.Read();
					Sut.Read();
					Sut.Close();
				}
				
				[Test]
				public void Should_set_current_token_value_to_an_empty_string()
				{
					Assert.That(Sut.CurrentTokenValue, Is.EqualTo(String.Empty));
				}
			}
		}
		
		public class When_reading_a_json_string_that_contains_an_array_with_two_booleans : ConcernFor<JsonReader>
		{
			private string input;
			
			protected override void Given()
			{
				input = "[ true, false ]";
			}
			
			public override JsonReader CreateSystemUnderTest()
			{
				return new JsonReader(input);
			}
			
			public class And_when_calling_read_once : When_reading_a_json_string_that_contains_an_array_with_two_booleans
			{
				protected override void When()
				{
					Sut.Read();
				}
				
				[Test]
				public void Should_set_current_token_to_array_start()
				{
					Assert.That(Sut.CurrentToken, Is.EqualTo(JsonToken.ArrayStart));
				}
			}
			
			public class And_when_calling_read_twice : When_reading_a_json_string_that_contains_an_array_with_two_booleans
			{
				protected override void When()
				{
					Sut.Read();
					Sut.Read();
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
			
			public class And_when_calling_read_three_times : When_reading_a_json_string_that_contains_an_array_with_two_booleans
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
				public void Should_set_current_token_value_to_false()
				{
					Assert.That(Sut.CurrentTokenValue, Is.False);
				}
			}
			
			public class And_when_calling_read_four_times : When_reading_a_json_string_that_contains_an_array_with_two_booleans
			{
				protected override void When()
				{
					for (int index = 1; index <= 4; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_current_token_to_array_end()
				{
					Assert.That(Sut.CurrentToken, Is.EqualTo(JsonToken.ArrayEnd));
				}
			}
			
			public class And_when_calling_read_five_times : When_reading_a_json_string_that_contains_an_array_with_two_booleans
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
		
		public class When_reading_a_json_string_that_contains_an_array_containing_four_strings : ConcernFor<JsonReader>
		{
			private string input;
			private string stringValue3;
			private string stringValue4;
			
			protected override void Given()
			{
				input = "[ \"One\", \"Two\", \"abc 123 \\n\\f\\b\\t\\r \\\" \\\\ \\u263a \\u25CF\", \"\\\"Hello\\\" \\'world\\'\" ]";
				stringValue3 = "abc 123 \n\f\b\t\r \" \\ \u263a \u25cf";
				stringValue4 = "\"Hello\" 'world'";
			}
			
			public override JsonReader CreateSystemUnderTest()
			{
				return new JsonReader(input);
			}
			
			public class And_when_calling_read_once : When_reading_a_json_string_that_contains_an_array_containing_four_strings
			{
				protected override void When()
				{
					Sut.Read();
				}
				
				[Test]
				public void Should_set_current_token_to_array_start()
				{
					Assert.That(Sut.CurrentToken, Is.EqualTo(JsonToken.ArrayStart));
				}
			}
			
			public class And_when_calling_read_twice : When_reading_a_json_string_that_contains_an_array_containing_four_strings
			{
				protected override void When()
				{
					Sut.Read();
					Sut.Read();
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
			
			public class And_when_calling_read_three_times : When_reading_a_json_string_that_contains_an_array_containing_four_strings
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
				public void Should_set_current_token_value_to_Two()
				{
					Assert.That(Sut.CurrentTokenValue, Is.EqualTo("Two"));
				}
			}
			
			public class And_when_calling_read_four_times : When_reading_a_json_string_that_contains_an_array_containing_four_strings
			{
				protected override void When()
				{
					for (int index = 1; index <= 4; index++)
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
				public void Should_set_current_token_value_to_stringValue3()
				{
					Assert.That(Sut.CurrentTokenValue, Is.EqualTo(stringValue3));
				}
			}
			
			public class And_when_calling_read_five_times : When_reading_a_json_string_that_contains_an_array_containing_four_strings
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
				public void Should_set_current_token_value_to_stringValue4()
				{
					Assert.That(Sut.CurrentTokenValue, Is.EqualTo(stringValue4));
				}
			}
			
			public class And_when_calling_read_six_times : When_reading_a_json_string_that_contains_an_array_containing_four_strings
			{
				protected override void When()
				{
					for (int index = 1; index <= 6; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_current_token_to_array_end()
				{
					Assert.That(Sut.CurrentToken, Is.EqualTo(JsonToken.ArrayEnd));
				}
			}
			
			public class And_when_calling_read_seven_times : When_reading_a_json_string_that_contains_an_array_containing_four_strings
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
		
		public class When_reading_a_json_string_that_contains_an_array_of_ints : ConcernFor<JsonReader>
		{
			private string input;
			
			protected override void Given()
			{
				input = @"[ -10, -5, -0, 0, 5, 10 ]";
			}
			
			public override JsonReader CreateSystemUnderTest()
			{
				return new JsonReader(input);
			}
			
			public class And_when_calling_read_twice : When_reading_a_json_string_that_contains_an_array_of_ints
			{
				protected override void When()
				{
					Sut.Read();
					Sut.Read();
				}
				
				[Test]
				public void Should_set_current_token_value_to_negative_10()
				{
					Assert.That((int)Sut.CurrentTokenValue, Is.EqualTo(-10));
				}
			}
			
			public class And_when_calling_read_three_times : When_reading_a_json_string_that_contains_an_array_of_ints
			{
				protected override void When()
				{
					for (int index = 1; index <= 3; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_current_token_value_to_negative_5()
				{
					Assert.That((int)Sut.CurrentTokenValue, Is.EqualTo(-5));
				}
			}
			
			public class And_when_calling_read_four_times : When_reading_a_json_string_that_contains_an_array_of_ints
			{
				protected override void When()
				{
					for (int index = 1; index <= 4; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_current_token_value_to_0()
				{
					Assert.That((int)Sut.CurrentTokenValue, Is.EqualTo(0));
				}
			}
			
			public class And_when_calling_read_five_times : When_reading_a_json_string_that_contains_an_array_of_ints
			{
				protected override void When()
				{
					for (int index = 1; index <= 5; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_current_token_value_to_0()
				{
					Assert.That((int)Sut.CurrentTokenValue, Is.EqualTo(0));
				}
			}
			
			public class And_when_calling_read_six_times : When_reading_a_json_string_that_contains_an_array_of_ints
			{
				protected override void When()
				{
					for (int index = 1; index <= 6; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_current_token_value_to_5()
				{
					Assert.That((int)Sut.CurrentTokenValue, Is.EqualTo(5));
				}
			}
			
			public class And_when_calling_read_seven_times : When_reading_a_json_string_that_contains_an_array_of_ints
			{
				protected override void When()
				{
					for (int index = 1; index <= 7; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_current_token_value_to_10()
				{
					Assert.That((int)Sut.CurrentTokenValue, Is.EqualTo(10));
				}
			}
		}
		
		public class When_reading_a_json_string_that_contains_an_array_of_doubles : ConcernFor<JsonReader>
		{
			private string input;
			
			protected override void Given()
			{
				input = @"[ -125.000009, 10.0, -10.0, 0.0, -0.0, 3.1415926536, 5e-4, 233e+5, 0.6e2, 2E-5 ]";
			}
			
			public override JsonReader CreateSystemUnderTest()
			{
				return new JsonReader(input);
			}
			
			public class And_when_calling_read_twice : When_reading_a_json_string_that_contains_an_array_of_doubles
			{
				protected override void When()
				{
					Sut.Read();
					Sut.Read();
				}
				
				[Test]
				public void Should_set_current_token_value_to_negative_125_dot_000009()
				{
					Assert.That((double)Sut.CurrentTokenValue, Is.EqualTo(-125.000009));
				}
			}
			
			public class And_when_calling_read_three_times : When_reading_a_json_string_that_contains_an_array_of_doubles
			{
				protected override void When()
				{
					for (int index = 1; index <= 3; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_current_token_value_to_10_dot_0()
				{
					Assert.That((double)Sut.CurrentTokenValue, Is.EqualTo(10.0));
				}
			}
			
			public class And_when_calling_read_four_times : When_reading_a_json_string_that_contains_an_array_of_doubles
			{
				protected override void When()
				{
					for (int index = 1; index <= 4; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_current_token_value_to_negative_10_dot_0()
				{
					Assert.That((double)Sut.CurrentTokenValue, Is.EqualTo(-10.0));
				}
			}
			
			public class And_when_calling_read_five_times : When_reading_a_json_string_that_contains_an_array_of_doubles
			{
				protected override void When()
				{
					for (int index = 1; index <= 5; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_current_token_value_to_0_dot_0()
				{
					Assert.That((double)Sut.CurrentTokenValue, Is.EqualTo(0.0));
				}
			}
			
			public class And_when_calling_read_six_times : When_reading_a_json_string_that_contains_an_array_of_doubles
			{
				protected override void When()
				{
					for (int index = 1; index <= 6; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_current_token_value_to_0_dot_0()
				{
					Assert.That((double)Sut.CurrentTokenValue, Is.EqualTo(0.0));
				}
			}
			
			public class And_when_calling_read_seven_times : When_reading_a_json_string_that_contains_an_array_of_doubles
			{
				protected override void When()
				{
					for (int index = 1; index <= 7; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_current_token_value_to_3_dot_1415926536()
				{
					Assert.That((double)Sut.CurrentTokenValue, Is.EqualTo(3.1415926536));
				}
			}
			
			public class And_when_calling_read_eight_times : When_reading_a_json_string_that_contains_an_array_of_doubles
			{
				protected override void When()
				{
					for (int index = 1; index <= 8; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_current_token_value_to_0_dot_0005()
				{
					Assert.That((double)Sut.CurrentTokenValue, Is.EqualTo(0.0005));
				}
			}
			
			public class And_when_calling_read_nine_times : When_reading_a_json_string_that_contains_an_array_of_doubles
			{
				protected override void When()
				{
					for (int index = 1; index <= 9; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_current_token_value_to_23300000_dot_0()
				{
					Assert.That((double)Sut.CurrentTokenValue, Is.EqualTo(23300000.0));
				}
			}
			
			public class And_when_calling_read_ten_times : When_reading_a_json_string_that_contains_an_array_of_doubles
			{
				protected override void When()
				{
					for (int index = 1; index <= 10; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_current_token_value_to_60_dot_0()
				{
					Assert.That((double)Sut.CurrentTokenValue, Is.EqualTo(60.0));
				}
			}
			
			public class And_when_calling_read_eleven_times : When_reading_a_json_string_that_contains_an_array_of_doubles
			{
				protected override void When()
				{
					for (int index = 1; index <= 11; index++)
					{
						Sut.Read();
					}
				}
				
				[Test]
				public void Should_set_current_token_value_to_0_dot_00002()
				{
					Assert.That((double)Sut.CurrentTokenValue, Is.EqualTo(0.00002));
				}
			}
		}
		
		public class When_reading_a_json_string_that_contains_an_error_in_the_escape_sequence : ConcernFor<JsonReader>
		{
			private string input;
			private JsonException expectedException;
			
			protected override void Given()
			{
				input = "[ \"Hello World \\ufffg \" ]";
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
			public void Should_set_message_to_Invalid_character_g_in_input_string()
			{
				Assert.That(expectedException.Message, Is.EqualTo("Invalid character 'g' in input string"));
			}
		}
		
		public class When_reading_a_json_string_that_contains_an_invalid_real_number : ConcernFor<JsonReader>
		{
			private string input;
			private JsonException expectedException;
			
			protected override void Given()
			{
				input = "[ 0.e5 ]";
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
			public void Should_set_message_to_Invalid_character_e_in_input_string()
			{
				Assert.That(expectedException.Message, Is.EqualTo("Invalid character 'e' in input string"));
			}
		}
		
		public class When_reading_a_json_string_that_contains_an_invalid_boolean_value : ConcernFor<JsonReader>
		{
			private string input;
			private JsonException expectedException;
			
			protected override void Given()
			{
				input = "[ TRUE ]";
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
			public void Should_set_message_to_Invalid_character_T_in_input_string()
			{
				Assert.That(expectedException.Message, Is.EqualTo("Invalid character 'T' in input string"));
			}
		}
		
		public class When_reading_a_json_string_that_contains_an_array_with_the_wrong_closing_token : ConcernFor<JsonReader>
		{
			private string input;
			private JsonException expectedException;
			
			protected override void Given()
			{
				input = "[ 1, 2, 3 }";
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
			public void Should_set_message_to_Invalid_token_125_in_input_string()
			{
				Assert.That(expectedException.Message, Is.EqualTo("Invalid token '125' in input string"));
			}
		}
		
		public class When_reading_a_json_string_that_contains_an_array_with_no_closing_token : ConcernFor<JsonReader>
		{
			private string input;
			private JsonException expectedException;
			
			protected override void Given()
			{
				input = "[ \"name1\" ";
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
		
		public class When_reading_a_json_string_that_contains_no_array_or_object_tokens : ConcernFor<JsonReader>
		{
			private string input;
			private JsonException expectedException;
			
			protected override void Given()
			{
				input = " true ";
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
			public void Should_set_message_to_Invalid_token_True_in_input_string()
			{
				Assert.That(expectedException.Message, Is.EqualTo("Invalid token 'True' in input string"));
			}
		}
		
		public class When_reading_a_json_string_with_nested_arrays : ConcernFor<JsonReader>
		{
			private string input;
			private int arrayCount;
			
			protected override void Given()
			{
				input = "[ [ [ [ [ 1, 2, 3 ] ] ] ] ]";
				arrayCount = 0;
			}
			
			public override JsonReader CreateSystemUnderTest()
			{
				return new JsonReader(input);
			}
			
			protected override void When()
			{
				while (Sut.Read())
				{
					if (Sut.CurrentToken == JsonToken.ArrayStart)
					{
						arrayCount++;
					}
				}
			}
			
			[Test]
			public void Should_count_five_arrays()
			{
				Assert.That(arrayCount, Is.EqualTo(5));
			}
		}
		
		public class When_reading_a_json_string_that_contains_an_array_with_comments : ConcernFor<JsonReader>
		{
			private string input;
			private JsonException expectedException;
			
			protected override void Given()
			{
				input = @"
		                [
		                    // This is a comment
		                    1,
		                    2,
		                    3
		                ]";
			}
			
			public override JsonReader CreateSystemUnderTest()
			{
				return new JsonReader(input);
			}
			
			public class And_when_comments_are_not_allowed : When_reading_a_json_string_that_contains_an_array_with_comments
			{
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
				public void Should_set_message_to_Invalid_character_forward_slash_in_input_string()
				{
					Assert.That(expectedException.Message, Is.EqualTo("Invalid character '/' in input string"));
				}
			}
		}
		
		public class When_reading_a_json_string_that_contains_an_array_with_a_string_in_single_quotes : ConcernFor<JsonReader>
		{
			private string input;
			private JsonException expectedException;
			
			protected override void Given()
			{
				input = "[ 'Single quotes' ]";
			}
			
			public override JsonReader CreateSystemUnderTest()
			{
				return new JsonReader(input);
			}
			
			public class And_when_single_quotes_are_not_allowed : When_reading_a_json_string_that_contains_an_array_with_a_string_in_single_quotes
			{
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
				public void Should_set_message_to_Invalid_character_single_quote_in_input_string()
				{
					Assert.That(expectedException.Message, Is.EqualTo("Invalid character ''' in input string"));
				}
			}
		}
	}
}
