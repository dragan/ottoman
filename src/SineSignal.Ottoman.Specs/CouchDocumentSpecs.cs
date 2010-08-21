using System;
using System.Reflection;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
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
				entity1 = new Employee { Id = Guid.NewGuid(), Name = "Bob", Login = "boblogin" };
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
				Assert.That(Sut["_id"], Is.EqualTo(entity1.Id));
				Assert.That(Sut["Type"], Is.EqualTo(entity1Type.Name));
				Assert.That(Sut["Name"], Is.EqualTo(entity1.Name));
				Assert.That(Sut["Login"], Is.EqualTo(entity1.Login));
			}
		}
	}
}
