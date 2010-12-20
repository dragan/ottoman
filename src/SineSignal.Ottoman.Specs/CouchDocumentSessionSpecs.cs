using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SineSignal.Ottoman.Specs.Framework;

using SineSignal.Ottoman.Commands;
using SineSignal.Ottoman.Exceptions;
using SineSignal.Ottoman.Http;

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
			private ICouchDocumentConvention documentConvention;
			private ICouchDatabase couchDatabase;
			
			protected override void Given()
			{
				entity1 = new Employee { Name = "Bob", Login = "boblogin" };
				entity1Type = entity1.GetType();
				identityProperty = entity1Type.GetProperty("Id");
				identityType = identityProperty.PropertyType;
				id = Guid.NewGuid();
				
				documentConvention = Fake<ICouchDocumentConvention>();
				documentConvention.GetIdentityPropertyFor(entity1Type).Returns(identityProperty);
				documentConvention.GenerateIdentityFor(identityType).Returns(id);
				
				couchDatabase = Fake<ICouchDatabase>();
				couchDatabase.CouchDocumentConvention.Returns(documentConvention);
			}
			
			public override CouchDocumentSession CreateSystemUnderTest()
			{
				return new CouchDocumentSession(couchDatabase);
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
			private ICouchDocumentConvention documentConvention;
			private ICouchDatabase couchDatabase;
			
			protected override void Given()
			{
				id = Guid.NewGuid();
				entity1 = new Employee { Id = id, Name = "Bob", Login = "boblogin" };
				entity1Type = entity1.GetType();
				identityProperty = entity1Type.GetProperty("Id");
				
				documentConvention = Fake<ICouchDocumentConvention>();
				documentConvention.GetIdentityPropertyFor(entity1Type).Returns(identityProperty);
				
				couchDatabase = Fake<ICouchDatabase>();
				couchDatabase.CouchDocumentConvention.Returns(documentConvention);
			}
			
			public override CouchDocumentSession CreateSystemUnderTest()
			{
				return new CouchDocumentSession(couchDatabase);
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
		
		public class When_attempting_to_store_a_different_entity_with_the_same_id_as_another : ConcernFor<CouchDocumentSession>
		{
			private Guid id;
			private Employee entity1;
			private Employee entity2;
			private Type entity2Type;
			private PropertyInfo identityProperty;
			private NonUniqueEntityException thrownException;
			private ICouchDocumentConvention documentConvention;
			private ICouchDatabase couchDatabase;
			
			protected override void Given()
			{
				id = Guid.NewGuid();
				entity1 = new Employee { Id = id, Name = "Bob", Login = "boblogin" };
				entity2 = new Employee { Id = id, Name = "Carl", Login = "carllogin" };
				entity2Type = entity2.GetType();
				identityProperty = entity2Type.GetProperty("Id");
				
				documentConvention = Fake<ICouchDocumentConvention>();
				documentConvention.GetIdentityPropertyFor(entity2Type).Returns(identityProperty);
				
				couchDatabase = Fake<ICouchDatabase>();
				couchDatabase.CouchDocumentConvention.Returns(documentConvention);
			}
			
			public override CouchDocumentSession CreateSystemUnderTest()
			{
				var couchDocumentSession = new CouchDocumentSession(couchDatabase);
				couchDocumentSession.Store(entity1);
				return couchDocumentSession;
			}
			
			protected override void When()
			{
				try
				{
					Sut.Store(entity2);
				}
				catch (NonUniqueEntityException ex)
				{
					thrownException = ex;
				}
			}
			
			[Test]
			public void Should_throw_non_unique_entity_exception()
			{
				Assert.That(thrownException, Is.Not.Null);
				Assert.That(thrownException.Message, Is.EqualTo("Attempted to associate a different entity with id '" + id + "'."));
			}
		}
		
		public class When_saving_changes_after_storing_a_new_entity : ConcernFor<CouchDocumentSession>
		{
			private Employee entity1;
			private Type entity1Type;
			private PropertyInfo identityProperty;
			private Type identityType;
			private Guid id;
			private ICouchDocumentConvention documentConvention;
			private BulkDocsResult[] bulkDocsResults;
			private ICouchProxy couchProxy;
			private ICouchDatabase couchDatabase;
			
			protected override void Given()
			{
				entity1 = new Employee { Name = "Bob", Login = "boblogin" };
				entity1Type = entity1.GetType();
				identityProperty = entity1Type.GetProperty("Id");
				identityType = identityProperty.PropertyType;
				id = Guid.NewGuid();
				
				documentConvention = Fake<ICouchDocumentConvention>();
				documentConvention.GetIdentityPropertyFor(entity1Type).Returns(identityProperty);
				documentConvention.GenerateIdentityFor(identityType).Returns(id);
				
				bulkDocsResults = new BulkDocsResult[1];
				bulkDocsResults[0] = new BulkDocsResult { Id = id.ToString(), Rev = "123456" };
				couchProxy = Fake<ICouchProxy>();
				couchProxy.Execute<BulkDocsResult[]>(Arg.Any<BulkDocsCommand>()).Returns(bulkDocsResults);
				
				couchDatabase = Fake<ICouchDatabase>();
				couchDatabase.CouchDocumentConvention.Returns(documentConvention);
				couchDatabase.Name.Returns("ottoman-test-database");
				couchDatabase.CouchProxy.Returns(couchProxy);
			}
			
			public override CouchDocumentSession CreateSystemUnderTest()
			{
				return new CouchDocumentSession(couchDatabase);
			}

			protected override void When()
			{
				Sut.Store(entity1);
				Sut.SaveChanges();
			}
			
			[Test]
			public void Should_call_get_identity_property()
			{
				documentConvention.Received().GetIdentityPropertyFor(entity1Type);
			}
			
			[Test]
			public void Should_execute_bulk_docs_command_with_couch_proxy()
			{
				couchProxy.Received().Execute<BulkDocsResult[]>(Arg.Is<BulkDocsCommand>(c => {
					var message = (BulkDocsMessage)c.Message;
					return c.Route == couchDatabase.Name + "/_bulk_docs" && 
						   c.Operation == HttpMethod.Post && 
						   (message.NonAtomic == false && message.AllOrNothing == false && message.Docs.Length == 1);
				}));
			}
		}
		
		public class When_loading_a_simple_entity : ConcernFor<CouchDocumentSession>
		{
			private Guid documentId;
			private Employee entity1;
			private Type entity1Type;
			private PropertyInfo identityProperty;
			private ICouchDocumentConvention documentConvention;
			private ICouchProxy couchProxy;
			private ICouchDatabase couchDatabase;
			
			protected override void Given()
			{
				documentId = Guid.NewGuid();
				entity1Type = typeof(Employee);
				identityProperty = entity1Type.GetProperty("Id");
				
				documentConvention = Fake<ICouchDocumentConvention>();
				documentConvention.GetIdentityPropertyFor(entity1Type).Returns(identityProperty);
				
				var couchDocument = new CouchDocument();
				couchDocument.Add("_id", documentId.ToString());
				couchDocument.Add("_rev", "123456");
				couchDocument.Add("Type", entity1Type.Name);
				couchDocument.Add("Name", "Bob");
				couchDocument.Add("Login", "boblogin");
				
				couchProxy = Fake<ICouchProxy>();
				couchProxy.Execute<CouchDocument>(Arg.Any<GetDocumentCommand>()).Returns(couchDocument);
				
				couchDatabase = Fake<ICouchDatabase>();
				couchDatabase.Name.Returns("ottoman-test-database");
				couchDatabase.CouchProxy.Returns(couchProxy);
				couchDatabase.CouchDocumentConvention.Returns(documentConvention);
			}
			
			public override CouchDocumentSession CreateSystemUnderTest()
			{
				return new CouchDocumentSession(couchDatabase);
			}
			
			protected override void When()
			{
				entity1 = Sut.Load<Employee>(documentId.ToString());
			}
			
			[Test]
			public void Should_execute_get_document_command_with_couch_proxy()
			{
				couchProxy.Received().Execute<CouchDocument>(Arg.Is<GetDocumentCommand>(c => {
					return c.Route == couchDatabase.Name + "/" + documentId.ToString() && 
						   c.Operation == HttpMethod.Get && 
						   c.Message == null && 
						   c.SuccessStatusCode == HttpStatusCode.OK;
				}));
			}
			
			[Test]
			public void Should_call_get_identity_property()
			{
				documentConvention.Received().GetIdentityPropertyFor(entity1Type);
			}
			
			[Test]
			public void Should_return_populated_entity()
			{
				Assert.That(entity1.Id, Is.EqualTo(documentId));
				Assert.That(entity1.Name, Is.EqualTo("Bob"));
				Assert.That(entity1.Login, Is.EqualTo("boblogin"));
			}
		}
	}
	
	public class Employee
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Login { get; set; }
		
		public override bool Equals (object obj)
		{
			if (obj == null)
				return false;
			
			Employee employee = obj as Employee;
			if (employee == null)
				return false;
			
			return (this.Id == employee.Id) && (this.Name == employee.Name) && (this.Login == employee.Login);
		}
		
		public override int GetHashCode ()
		{
			return Id.GetHashCode() ^ Name.GetHashCode() ^ Login.GetHashCode();
		}
	}
}
