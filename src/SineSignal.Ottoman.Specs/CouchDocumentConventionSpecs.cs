using System;
using System.Reflection;

using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SineSignal.Ottoman.Specs.Framework;

namespace SineSignal.Ottoman.Specs
{
	public class CouchDocumentConventionSpecs
	{
		public class When_retrieving_the_identity_property_descriptor_for_a_given_type : ConcernFor<CouchDocumentConvention>
		{
			string defaultIdentityName;
			Employee entity;
			PropertyInfo identityProperty;
			
			protected override void Given()
			{
				defaultIdentityName = "Id";
				entity = new Employee { Id = Guid.NewGuid(), Name = "Bob", Login = "boblogin" };
			}
			
			public override CouchDocumentConvention CreateSystemUnderTest()
			{
				return new CouchDocumentConvention();
			}
			
			protected override void When()
			{
				identityProperty = Sut.GetIdentityPropertyFor(entity.GetType());
			}
			
			[Test]
			public void Should_be_able_to_get_identity_property_descriptor_given_the_name_of_the_property()
			{
				Assert.That(identityProperty.Name, Is.EqualTo(defaultIdentityName));
			}
		}
		
		public class When_generating_an_identity_value_for_a_given_identity_type : ConcernFor<CouchDocumentConvention>
		{
			private Employee entity;
			private object generatedValue;
			
			protected override void Given()
			{
				entity = new Employee { Name = "Bob", Login = "boblogin" };
			}
			
			public override CouchDocumentConvention CreateSystemUnderTest ()
			{
				return new CouchDocumentConvention();
			}
			
			protected override void When()
			{
				generatedValue = Sut.GenerateIdentityFor(entity.Id.GetType());
			}
			
			[Test]
			public void Should_return_a_Guid_type()
			{
				Assert.That(generatedValue, Is.TypeOf(typeof(Guid)));
			}
			
			[Test]
			public void Should_not_return_an_empty_Guid()
			{
				Assert.That((Guid)generatedValue, Is.Not.EqualTo(Guid.Empty));
			}
		}
	}
}
