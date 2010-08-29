using System;
using System.Net;

using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SineSignal.Ottoman.Specs.Framework;

using SineSignal.Ottoman.Commands;
using SineSignal.Ottoman.Exceptions;
using SineSignal.Ottoman.Http;

namespace SineSignal.Ottoman.Specs
{
	public class CouchProxySpecs
	{
		public class When_executing_a_couch_command_with_a_message : ConcernFor<CouchProxy>
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
				couchCommand.SuccessStatusCode.Returns(HttpStatusCode.Created);
				
				restResponse = new RestResponse<ResultStub>
				{
					ContentType = "application/json",
					ContentLength = 5,
					Content = "{\"status\":\"completed\"}",
					StatusCode = HttpStatusCode.Created,
					StatusDescription = HttpStatusCode.Created.ToString(),
					ContentDeserialized = new ResultStub()
				};
				
				restClient.Process<ResultStub>(Arg.Any<RestRequest>(), Arg.Any<HttpStatusCode>()).Returns(restResponse);
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
				}), Arg.Is<HttpStatusCode>(h => {
					return h == HttpStatusCode.Created;
				}));
			}
			
			[Test]
			public void Should_return_deserialized_object()
			{
				Assert.That(resultStub.Status, Is.EqualTo("completed"));
			}
		}
		
		public class When_executing_a_couch_command_without_a_message : ConcernFor<CouchProxy>
		{
			private IRestClient restClient;
			private ICouchCommand couchCommand;
			private ResultStub resultStub;
			private RestResponse<ResultStub> restResponse;
			
			protected override void Given()
			{
				restClient = Fake<IRestClient>();
				couchCommand = Fake<ICouchCommand>();
				couchCommand.Route.Returns("some/path");
				couchCommand.Operation.Returns(HttpMethod.Get);
				couchCommand.SuccessStatusCode.Returns(HttpStatusCode.OK);
				
				restResponse = new RestResponse<ResultStub>
				{
					ContentType = "application/json",
					ContentLength = 5,
					Content = "{\"status\":\"completed\"}",
					StatusCode = HttpStatusCode.OK,
					StatusDescription = HttpStatusCode.OK.ToString(),
					ContentDeserialized = new ResultStub()
				};
				
				restClient.Process<ResultStub>(Arg.Any<RestRequest>(), Arg.Any<HttpStatusCode>()).Returns(restResponse);
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
						   r.Method == HttpMethod.Get && 
						   r.Payload == null;
				}), Arg.Is<HttpStatusCode>(h => {
					return h == HttpStatusCode.OK;
				}));
			}
			
			[Test]
			public void Should_return_deserialized_object()
			{
				Assert.That(resultStub.Status, Is.EqualTo("completed"));
			}
		}
		
		[Ignore("Until we can figure out how to tell NSubstitute to throw an exception when Process is called")]
		public class When_executing_a_command_that_causes_an_unexpected_response_by_rest_client : ConcernFor<CouchProxy>
		{
			private IRestClient restClient;
			private ICouchCommand couchCommand;
			
			protected override void Given()
			{
				restClient = Fake<IRestClient>();
				
				couchCommand = Fake<ICouchCommand>();
				couchCommand.Route.Returns("some/path");
				couchCommand.Operation.Returns(HttpMethod.Get);
				couchCommand.SuccessStatusCode.Returns(HttpStatusCode.OK);
				
				// TODO:  How to tell restClient to throw exception when Process gets called.
			}
			
			public override CouchProxy CreateSystemUnderTest()
			{
				return new CouchProxy(restClient);
			}
			
			protected override void When()
			{
				Sut.Execute<ResultStub>(couchCommand);
			}
			
			[Test]
			public void Should_call_process_on_rest_client_with_generated_rest_request()
			{
				restClient.Received().Process<ResultStub>(Arg.Is<RestRequest>(r => {
					return r.Path == "some/path" && 
						   r.Method == HttpMethod.Get && 
						   r.Payload == null;
				}), Arg.Is<HttpStatusCode>(h => {
					return h == HttpStatusCode.OK;
				}));
			}
			
			[Test]
			public void Should_call_error_handler_on_couch_command()
			{
				// TODO: Make sure HandleError on CouchCommand is called
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
