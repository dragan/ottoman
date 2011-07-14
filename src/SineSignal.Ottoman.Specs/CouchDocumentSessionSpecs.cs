using System;
using System.Net;
using System.Reflection;

using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SineSignal.Ottoman.Specs.Framework;

using SineSignal.Ottoman.Commands;
using SineSignal.Ottoman.Http;

namespace SineSignal.Ottoman.Specs
{
	public class CouchDocumentSessionSpecs
	{
		public class When_storing_a_new_entity : ConcernFor<CouchDocumentSession>
		{
			Employee entity1;
			Type entity1Type;
			PropertyInfo identityProperty;
			Type identityType;
			Guid id;
			
			ICouchProxy couchProxy;
			ICouchDocumentConvention documentConvention;
			string databaseName;
			ICouchDatabase couchDatabase;
			
			protected override void Given()
			{
				entity1 = new Employee { Name = "Bob", Login = "boblogin" };
				entity1Type = entity1.GetType();
				identityProperty = entity1Type.GetProperty("Id");
				identityType = identityProperty.PropertyType;
				id = Guid.NewGuid();
				
				couchProxy = Fake<ICouchProxy>();
				var putDocumentResult = new PutDocumentResult { Stored = true, Id = id.ToString(), Revision = "1-" + id.ToString() };
				couchProxy.Execute<PutDocumentResult>(Arg.Any<PutDocumentCommand>()).Returns(putDocumentResult);
				
				documentConvention = Fake<ICouchDocumentConvention>();
				documentConvention.GetIdentityPropertyFor(entity1Type).Returns(identityProperty);
				documentConvention.GenerateIdentityFor(identityType).Returns(id);
				
				couchDatabase = Fake<ICouchDatabase>();
				databaseName = "ottoman-test-database";
				couchDatabase.CouchDocumentConvention.Returns(documentConvention);
				couchDatabase.Name.Returns(databaseName);
				couchDatabase.CouchProxy.Returns(couchProxy);
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
			public void Should_execute_put_document_command_with_couch_proxy()
			{
				couchProxy.Received().Execute<PutDocumentResult>(
					Arg.Is<PutDocumentCommand>(c => c.Route == couchDatabase.Name + "/" + id && 
						c.Operation == HttpMethod.Put && 
						c.SuccessStatusCode == HttpStatusCode.Created && 
				        c.Message != null && 
				        c.Message is CouchDocument
				));
			}
			
			[Test]
			public void Should_set_id_for_entity()
			{
				Assert.That(entity1.Id, Is.EqualTo(id));
			}
			
			[Test]
			public void Should_store_entity_in_cache()
			{
				Employee entity = Sut.Load<Employee>(id.ToString());
				Assert.That(entity, Is.EqualTo(entity1));
			}
		}
		
		public class When_loading_an_entity_that_has_not_been_added_to_the_cache : ConcernFor<CouchDocumentSession>
		{
			private const string EmployeeName = "Bob";
			private const string EmployeeLogin = "boblogin";
			
			Employee entity;
			Guid employeeId;
			private Type entityType;
			PropertyInfo identityProperty;
			ICouchDatabase couchDatabase;
			private string databaseName;
			private ICouchProxy couchProxy;
			private ICouchDocumentConvention documentConvention;
			
			protected override void Given()
			{
				employeeId = Guid.NewGuid();
				entityType = typeof(Employee);
				identityProperty = entityType.GetProperty("Id");
				
				documentConvention = Fake<ICouchDocumentConvention>();
				documentConvention.GetIdentityPropertyFor(entityType).Returns(identityProperty);
				
				var couchDocument = new CouchDocument();
				couchDocument.Add("_id", employeeId.ToString());
				couchDocument.Add("_rev", "1-" + employeeId.ToString());
				couchDocument.Add("Type", entityType.Name);
				couchDocument.Add("Name", EmployeeName);
				couchDocument.Add("Login", EmployeeLogin);
				
				couchProxy = Fake<ICouchProxy>();
				couchProxy.Execute<CouchDocument>(Arg.Any<GetDocumentCommand>()).Returns(couchDocument);
				
				couchDatabase = Fake<ICouchDatabase>();
				databaseName = "ottoman-test-database";
				couchDatabase.Name.Returns(databaseName);
				couchDatabase.CouchProxy.Returns(couchProxy);
				couchDatabase.CouchDocumentConvention.Returns(documentConvention);
			}
			
			public override CouchDocumentSession CreateSystemUnderTest()
			{
				return new CouchDocumentSession(couchDatabase);
			}
			
			protected override void When()
			{
				entity = Sut.Load<Employee>(employeeId);
			}
			
			private bool DoArgsMatch(GetDocumentCommand c)
			{
				return c.Route == couchDatabase.Name + "/" + employeeId.ToString() && c.Operation == HttpMethod.Get && c.Message == null && c.SuccessStatusCode == HttpStatusCode.OK;
			}
			
			[Test]
			public void Should_execute_get_document_command_with_couch_proxy()
			{
				couchProxy.Received().Execute<CouchDocument>(
					Arg.Is<GetDocumentCommand>(c => c.Route == couchDatabase.Name + "/" + employeeId && 
						   c.Operation == HttpMethod.Get && 
						   c.Message == null && 
						   c.SuccessStatusCode == HttpStatusCode.OK
				));
			}
			
			[Test]
			public void Should_call_get_identity_property()
			{
				documentConvention.Received().GetIdentityPropertyFor(entityType);
			}
			
			[Test]
			public void Should_return_specified_entity()
			{
				Assert.That(entity.Id, Is.EqualTo(employeeId));
				Assert.That(entity.Name, Is.EqualTo(EmployeeName));
				Assert.That(entity.Login, Is.EqualTo(EmployeeLogin));
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
