using System;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using TechTalk.SpecFlow;

using SineSignal.Ottoman;

namespace SineSignal.Ottoman.AcceptanceSpecs
{
	[Binding]
	public class DeveloperConnectsToServerSteps
	{
		private string address;
		private ICouchClient couchClient;
		
		[Given("I have a CouchDB instance running at http://127.0.0.1:5984")]
		public void GivenIHaveACouchDBInstanceRunningAtHttp127_0_0_15984()
		{
			address = "http://127.0.0.1:5984";
		}

		[When("I call ConnectTo on CouchClient")]
		public void WhenICallConnectToOnCouchClient()
		{
			couchClient = CouchClient.ConnectTo(address);
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
	}
}
