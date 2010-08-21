using System;

using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SineSignal.Ottoman.Specs.Framework;

using SineSignal.Ottoman.Commands;
using SineSignal.Ottoman.Http;

namespace SineSignal.Ottoman.Specs
{
	public class CouchProxySpecs
	{
		public class When_executing_a_post_couch_command : ConcernFor<CouchProxy>
		{
			private Employee entity1;
			private IRestClient restClient;
			private ICouchCommand couchCommand;
			private ResultStub resultStub;
			private RestResponse<ResultStub> restResponse;
			
			protected override void Given()
			{
				entity1 = new Employee { Name = "Bob", Login = "boblogin" };
				restClient = Fake<IRestClient>();
				couchCommand = Fake<ICouchCommand>();
				couchCommand.Route.Returns("some/path");
				couchCommand.Operation.Returns(HttpMethod.Post);
				couchCommand.Message.Returns(entity1);
				
				restResponse = new RestResponse<ResultStub>
				{
					ContentType = "application/json",
					ContentLength = 5,
					Content = "{\"status\":\"completed\"}",
					StatusCode = System.Net.HttpStatusCode.Created,
					StatusDescription = System.Net.HttpStatusCode.Created.ToString(),
					ContentDeserialized = new ResultStub()
				};
				
				restClient.Process<ResultStub>(Arg.Any<RestRequest>()).Returns(restResponse);
			}
			
			public override CouchProxy CreateSystemUnderTest()
			{
				return new CouchProxy(restClient);
			}

			protected override void When()
			{
				resultStub = Sut.Execute<ResultStub>(couchCommand);
			}
			
			[Test]
			public void Should_call_process_on_rest_client_with_generated_rest_request()
			{
				restClient.Received().Process<ResultStub>(Arg.Is<RestRequest>(r => {
					return r.Path == "some/path" && 
						   r.Method == HttpMethod.Post && 
						   (r.Payload is Employee && r.Payload == entity1);
				}));
			}
			
			[Test]
			public void Should_return_deserialized_object()
			{
				Assert.That(resultStub.Status, Is.EqualTo("completed"));
			}
		}
	}
	
	public class ResultStub
	{
		public string Status { get; private set; }
			
		public ResultStub()
		{
			Status = "completed";
		}
	}
}
