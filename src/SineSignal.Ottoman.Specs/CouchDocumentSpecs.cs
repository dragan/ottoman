using System;
using System.Dynamic;
using System.Reflection;

using NUnit.Framework;
using SineSignal.Ottoman.Specs.Framework;

namespace SineSignal.Ottoman.Specs
{
	public class CouchDocumentSpecs
	{
		public class When_instantiating_a_new_couch_document : ConcernFor<CouchDocument>
		{
			private Employee entity1;
			private Type entity1Type;
			private PropertyInfo identityProperty;
			
			protected override void Given()
			{
				entity1 = new Employee { Name = "Bob", Login = "boblogin" };
				entity1Type = entity1.GetType();
				identityProperty = entity1Type.GetProperty("Id");
			}
			
			public override CouchDocument CreateSystemUnderTest()
			{
				return new CouchDocument(entity1, identityProperty);
			}
			
			[Test]
			public void Should_copy_passed_in_entity()
			{
				dynamic sut = (dynamic)Sut;
				
				Assert.That(sut._id, Is.EqualTo(entity1.Id));
				Assert.That(sut.Type, Is.EqualTo(entity1Type.Name));
				Assert.That(sut.Name, Is.EqualTo(entity1.Name));
				Assert.That(sut.Login, Is.EqualTo(entity1.Login));
			}
		}
	}
}
