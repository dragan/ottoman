using System;
using System.Reflection;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SineSignal.Ottoman.Specs.Framework;

namespace SineSignal.Ottoman.Specs
{
	public class CouchDocumentSpecs
	{
		public class When_dehydrating_an_entity_into_a_couch_document_with_no_revision : StaticConcern
		{
			private Employee entity1;
			private Type entity1Type;
			private PropertyInfo identityProperty;
			private CouchDocument couchDocument;
			
			protected override void Given()
			{
				entity1 = new Employee { Id = Guid.NewGuid(), Name = "Bob", Login = "boblogin" };
				entity1Type = entity1.GetType();
				identityProperty = entity1Type.GetProperty("Id");
			}
			
			protected override void When()
			{
				couchDocument = CouchDocument.Dehydrate(entity1, identityProperty, String.Empty);
			}
			
			[Test]
			public void Should_create_couch_document_without_a_revision()
			{
				Assert.That(couchDocument["_id"], Is.EqualTo(entity1.Id));
				Assert.That(couchDocument["Type"], Is.EqualTo(entity1Type.Name));
				Assert.That(couchDocument["Name"], Is.EqualTo(entity1.Name));
				Assert.That(couchDocument["Login"], Is.EqualTo(entity1.Login));
			}
		}
		
		public class When_dehydrating_an_entity_into_a_couch_document_with_a_revision : StaticConcern
		{
			private Employee entity1;
			private Type entity1Type;
			private PropertyInfo identityProperty;
			private string revision;
			private CouchDocument couchDocument;
			
			protected override void Given()
			{
				entity1 = new Employee { Id = Guid.NewGuid(), Name = "Bob", Login = "boblogin" };
				entity1Type = entity1.GetType();
				identityProperty = entity1Type.GetProperty("Id");
				revision = "abc123";
			}
			
			protected override void When()
			{
				couchDocument = CouchDocument.Dehydrate(entity1, identityProperty, revision);
			}
			
			[Test]
			public void Should_create_couch_document_with_revision()
			{
				Assert.That(couchDocument["_id"], Is.EqualTo(entity1.Id));
				Assert.That(couchDocument["_rev"], Is.EqualTo(revision));
				Assert.That(couchDocument["Type"], Is.EqualTo(entity1Type.Name));
				Assert.That(couchDocument["Name"], Is.EqualTo(entity1.Name));
				Assert.That(couchDocument["Login"], Is.EqualTo(entity1.Login));
			}
		}
	}
}
