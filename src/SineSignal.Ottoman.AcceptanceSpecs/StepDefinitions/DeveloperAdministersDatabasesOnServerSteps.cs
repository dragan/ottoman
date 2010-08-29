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
		private CannotCreateDatabaseException createDatabaseException;
		private CannotDeleteDatabaseException deleteDatabaseException;
		
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
		
		[Given("I have an invalid name for a database")]
		public void GivenIHaveAnInvalidNameForADatabase()
		{
			// CouchDB doesn't accept database names with upper-case characters
			databaseName = "OttomanTestDatabase";
		}
		
		[Given("I have a name for a database that doesn't exist on the server")]
		public void GivenIHaveANameForADatabaseThatDoesntExistOnTheServer()
		{
			databaseName = "x";
		}
		
		[When("I call CreateDatabase on CouchClient")]
		public void WhenICallCreateDatabaseOnCouchClient()
		{
			try
			{
				couchClient.CreateDatabase(databaseName);
			}
			catch (CannotCreateDatabaseException e)
			{
				createDatabaseException = e;
			}
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
			try
			{
				couchClient.DeleteDatabase(databaseName);
			}
			catch (CannotDeleteDatabaseException e)
			{
				deleteDatabaseException = e;
			}
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
		
		[Then("the result should be a CannotCreateDatabaseException")]
		public void ThenTheResultShouldBeACannotCreateDatabaseException()
		{
			Assert.That(createDatabaseException, Is.TypeOf(typeof(CannotCreateDatabaseException)));
		}
		
		[Then("the result should be a CannotDeleteDatabaseException")]
		public void ThenTheResultShouldBeACannotDeleteDatabaseException()
		{
			Assert.That(deleteDatabaseException, Is.TypeOf(typeof(CannotDeleteDatabaseException)));
		}
	}
}
