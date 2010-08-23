using System;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using TechTalk.SpecFlow;

using SineSignal.Ottoman;
using SineSignal.Ottoman.Exceptions;

namespace SineSignal.Ottoman.AcceptanceSpecs
{
	[Binding]
	public class DeveloperAdministersDatabasesOnServer
	{
		private ICouchClient couchClient;
		private string databaseName;
		
		[Given("I have an instance of CouchClient")]
		public void GivenIHaveAnInstanceOfCouchClient()
		{
			couchClient = CouchClient.ConnectTo("http://127.0.0.1:5984");
		}
		
		[Given("I have a name for a database")]
		public void GivenIHaveANameForADatabase()
		{
			databaseName = "ottoman-test-database";
		}
		
		[When("I call CreateDatabase on CouchClient")]
		public void WhenICallCreateDatabaseOnCouchClient()
		{
			couchClient.CreateDatabase(databaseName);
		}

		[Then("the result should be the database was created on the server")]
		public void ThenTheResultShouldBeTheDatabaseWasCreatedOnTheServer()
		{
			ICouchDatabase database = couchClient.GetDatabase(databaseName);
			Assert.That(databaseName, Is.EqualTo(database.Name));
		}
		
		[When("I call DeleteDatabase on CouchClient")]
		public void WhenICallDeleteDatabaseOnCouchClient()
		{
			couchClient.DeleteDatabase(databaseName);
		}

		[Then("the result should be the database was deleted on the server")]
		public void ThenTheResultShouldBeTheDatabaseWasDeletedOnTheServer()
		{
			try
			{
				couchClient.GetDatabase(databaseName);
			}
			catch (CannotGetDatabaseException e)
			{
				Assert.That(e, Is.TypeOf(typeof(CannotGetDatabaseException)));
			}
		}
	}
}
