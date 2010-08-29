using System;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using TechTalk.SpecFlow;

using SineSignal.Ottoman;
using SineSignal.Ottoman.Exceptions;

namespace SineSignal.Ottoman.AcceptanceSpecs
{
	[Binding]
	public class DeveloperConnectsToServerSteps
	{
		private string address;
		private ICouchClient couchClient;
		private CannotConnectToServerException exception;
		
		[Given("I have a CouchDB instance running at http://127.0.0.1:5984")]
		public void GivenIHaveACouchDBInstanceRunningAtHttp127_0_0_15984()
		{
			address = "http://127.0.0.1:5984";
		}

		[When("I call ConnectTo on CouchClient")]
		public void WhenICallConnectToOnCouchClient()
		{
			try
			{
				couchClient = CouchClient.ConnectTo(address);
			}
			catch (CannotConnectToServerException e)
			{
				exception = e;
			}
		}

		[Then("the result should be an instance of CouchClient")]
		public void ThenTheResultShouldBeAnInstanceOfCouchClient()
		{
			Assert.That(couchClient, Is.Not.Null);
		}
		
		[Then("ServerVersion should not be null or empty")]
		public void ThenServerVersionShouldNotBeNullOrEmpty()
		{
			Assert.That(String.IsNullOrEmpty(couchClient.ServerVersion), Is.False);
		}
		
		[Given("I do not have a CouchDB instance running at http://127.0.0.1:5985")]
		public void GivenIDoNotHaveACouchDBInstanceRunningAtHttp127_0_0_15985()
		{
			address = "http://127.0.0.1:5985";
		}
		
		[Then("the result should be a CannotConnectToServerException")]
		public void ThenTheResultShouldBeACannotConnectToServerException()
		{
			Assert.That(exception, Is.TypeOf(typeof(CannotConnectToServerException)));
		}
	}
}
