using System;
using System.Reflection;

using NSubstitute;
using NUnit.Framework;
using SineSignal.Ottoman.Specs.Framework;

namespace SineSignal.Ottoman.Specs
{
	public class CouchDocumentSessionSpecs
	{
		public class When_storing_a_new_entity_into_the_session : ConcernFor<CouchDocumentSession>
		{
			private Employee entity1;
			private Type entity1Type;
			private PropertyInfo identityProperty;
			private Type identityType;
			private Guid id;
			private IDocumentConvention documentConvention;
			
			protected override void Given()
			{
				entity1 = new Employee { Name = "Bob", Login = "boblogin" };
				entity1Type = entity1.GetType();
				identityProperty = entity1Type.GetProperty("Id");
				identityType = identityProperty.PropertyType;
				id = Guid.NewGuid();
				documentConvention = Fake<IDocumentConvention>();
				documentConvention.GetIdentityPropertyFor(entity1Type).Returns(identityProperty);
				documentConvention.GenerateIdentityFor(identityType).Returns(id);
			}
			
			public override CouchDocumentSession CreateSystemUnderTest()
			{
				return new CouchDocumentSession(documentConvention);
			}
			
			protected override void When()
			{
				Sut.Store(entity1);
			}
			
			[Test]
			public void Should_call_get_identity_property()
			{
				documentConvention.Received().GetIdentityPropertyFor(entity1Type);
			}
			
			[Test]
			public void Should_call_generate_document_id()
			{
				documentConvention.Received().GenerateIdentityFor(identityType);
			}
			
			[Test]
			public void Should_generate_id_for_entity()
			{
				Assert.That(entity1.Id, Is.EqualTo(id));
			}
			
			[Test]
			public void Should_add_entity_to_identity_map()
			{
				Employee entity = Sut.Load<Employee>(id.ToString());
				Assert.That(entity, Is.EqualTo(entity1));
			}
		}
		
		public class When_storing_an_entity_with_an_id_already_assigned : ConcernFor<CouchDocumentSession>
		{
			private Guid id;
			private Employee entity1;
			private Type entity1Type;
			private PropertyInfo identityProperty;
			private IDocumentConvention documentConvention;
			
			protected override void Given()
			{
				id = Guid.NewGuid();
				entity1 = new Employee { Id = id, Name = "Bob", Login = "boblogin" };
				entity1Type = entity1.GetType();
				identityProperty = entity1Type.GetProperty("Id");
				documentConvention = Fake<IDocumentConvention>();
				documentConvention.GetIdentityPropertyFor(entity1Type).Returns(identityProperty);
			}
			
			public override CouchDocumentSession CreateSystemUnderTest()
			{
				return new CouchDocumentSession(documentConvention);
			}
			
			protected override void When()
			{
				Sut.Store(entity1);
			}
			
			[Test]
			public void Should_call_get_identity_property()
			{
				documentConvention.Received().GetIdentityPropertyFor(entity1Type);
			}
			
			[Test]
			public void Should_not_call_generate_document_id()
			{
				documentConvention.DidNotReceive().GenerateIdentityFor(identityProperty.PropertyType);
			}
			
			[Test]
			public void Should_not_modify_the_id()
			{
				Assert.That(id, Is.EqualTo(entity1.Id));
			}
			
			[Test]
			public void Should_add_entity_to_identity_map()
			{
				Employee entity = Sut.Load<Employee>(id.ToString());
				Assert.That(entity, Is.EqualTo(entity1));
			}
		}
	}
	
	public class Employee
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Login { get; set; }
	}
}
