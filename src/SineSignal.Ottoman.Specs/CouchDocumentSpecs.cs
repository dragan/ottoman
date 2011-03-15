using System;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SineSignal.Ottoman.Specs.Framework;

namespace SineSignal.Ottoman.Specs
{
	public class CouchDocumentSpecs
	{
		public class When_hydrating_a_simple_entity_from_a_couch_document : ConcernFor<CouchDocument>
		{
			Guid generatedGuid;
			Type entityType;
			PropertyInfo identityProperty;
			SimpleEntity entity;
			
			protected override void Given()
			{
				generatedGuid = Guid.NewGuid();
				entityType = typeof(SimpleEntity);
				identityProperty = entityType.GetProperty("IntegerProperty");
			}
			
			public override CouchDocument CreateSystemUnderTest()
			{
				var document = new CouchDocument();
				document["_id"] = 1.ToString();
				document["_rev"] = "1-" + 1.ToString();
				document["Type"] = "SimpleEntity";
            	document["DecimalProperty"] = 1.25M.ToString();
            	document["FloatProperty"] = 1.5f.ToString();
            	document["CharProperty"] = 'a'.ToString();
            	document["StringProperty"] = "abcdefghijklmnopqrstuvwxyz1234567890";
            	document["GuidProperty"] = generatedGuid.ToString();
				
				return document;
			}
			
			protected override void When()
			{
				entity = Sut.HydrateEntity<SimpleEntity>(identityProperty);
			}
			
			[Test]
			public void Should_return_a_populated_instance_of_simple_entity()
			{
				Assert.That(entity.IntegerProperty, Is.EqualTo(1));
				Assert.That(entity.DecimalProperty, Is.EqualTo(1.25M));
				Assert.That(entity.FloatProperty, Is.EqualTo(1.5f));
				Assert.That(entity.CharProperty, Is.EqualTo('a'));
				Assert.That(entity.StringProperty, Is.EqualTo("abcdefghijklmnopqrstuvwxyz1234567890"));
				Assert.That(entity.GuidProperty, Is.EqualTo(generatedGuid));
			}
		}
		
		public class When_hydrating_a_complex_entity_from_a_couch_document : ConcernFor<CouchDocument>
		{
			Guid generatedGuid;
			Type entityType;
			PropertyInfo identityProperty;
			ComplexEntity entity;
			
			protected override void Given()
			{
				generatedGuid = Guid.NewGuid();
				entityType = typeof(ComplexEntity);
				identityProperty = entityType.GetProperty("Id");
			}
			
			public override CouchDocument CreateSystemUnderTest()
			{
				var simpleEntity = new Dictionary<string, object>();
				simpleEntity["IntegerProperty"] = 10.ToString();
				simpleEntity["DecimalProperty"] = 1.25M.ToString();
            	simpleEntity["FloatProperty"] = 1.5f.ToString();
            	simpleEntity["CharProperty"] = 'a'.ToString();
            	simpleEntity["StringProperty"] = "abcdefghijklmnopqrstuvwxyz1234567890";
            	simpleEntity["GuidProperty"] = generatedGuid.ToString();
				
				var document = new CouchDocument();
				document["_id"] = 1.ToString();
				document["_rev"] = "1-" + 1.ToString();
				document["Type"] = entityType.Name;
				document["SimpleEntity"] = simpleEntity;
				
				return document;
			}
			
			protected override void When()
			{
				entity = Sut.HydrateEntity<ComplexEntity>(identityProperty);
			}
			
			[Test]
			public void Should_return_a_populated_instance_of_complex_entity()
			{
				Assert.That(entity.Id, Is.EqualTo(1));
				Assert.That(entity.SimpleEntity.IntegerProperty, Is.EqualTo(10));
				Assert.That(entity.SimpleEntity.DecimalProperty, Is.EqualTo(1.25M));
				Assert.That(entity.SimpleEntity.FloatProperty, Is.EqualTo(1.5f));
				Assert.That(entity.SimpleEntity.CharProperty, Is.EqualTo('a'));
				Assert.That(entity.SimpleEntity.StringProperty, Is.EqualTo("abcdefghijklmnopqrstuvwxyz1234567890"));
				Assert.That(entity.SimpleEntity.GuidProperty, Is.EqualTo(generatedGuid));
			}
		}
		
		class SimpleEntity
	    {
	        public int IntegerProperty { get; set; }
	        public decimal DecimalProperty { get; set; }
	        public float FloatProperty { get; set; }
	        public char CharProperty { get; set; }
	        public string StringProperty { get; set; }
	        public Guid GuidProperty { get; set; }
	    }
		
		class ComplexEntity
		{
			public int Id { get; set; }
			public SimpleEntity SimpleEntity { get; set; }
		}
	}
}
