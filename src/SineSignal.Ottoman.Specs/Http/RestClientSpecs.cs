using System;
using System.Net;

using NSubstitute;
using NUnit.Framework;
using SineSignal.Ottoman.Specs.Framework;

using SineSignal.Ottoman.Http;
using SineSignal.Ottoman.Serialization;

namespace SineSignal.Ottoman.Specs.Http
{
	public class RestClientSpecs
	{
		public class When_processing_a_post_request : ConcernFor<RestClient>
		{
			private Uri baseUri;
			private IHttpClient httpClient;
			private RestRequest restRequest;
			private Employee payload;
			private RestResponse<ResultStub> restResponse;
			private ISerializer serializer;
			private string requestContent;
			private string responseContent;
			private ResultStub contentDeserialized;
			private string jsonContentType;
			
			protected override void Given()
			{
				baseUri = new Uri("http://127.0.0.1:5984");
				payload = new Employee { Name = "Bob", Login = "boblogin" };
				restRequest = new RestRequest { Path = "some/path", Method = HttpMethod.Post, Payload = payload };
				requestContent = "{\"Id\":\"" + Guid.Empty + "\",\"Name\":\"\",\"Login\":\"\"}";
				responseContent = "{\"status\":\"completed\"}";
				jsonContentType = "application/json";
				
				serializer = Fake<ISerializer>();
				serializer.Serialize(Arg.Any<object>()).Returns(requestContent);
				contentDeserialized = new ResultStub();
				serializer.Deserialize<ResultStub>(Arg.Any<string>()).Returns(contentDeserialized);
				
				httpClient = Fake<IHttpClient>();
				httpClient.Send(Arg.Any<HttpRequest>()).Returns(new HttpResponse {
					ContentType = jsonContentType,
					ContentLength = responseContent.Length,
					Content = responseContent,
					StatusCode = HttpStatusCode.Created,
					StatusDescription = HttpStatusCode.Created.ToString()
				});
			}
			
			public override RestClient CreateSystemUnderTest()
			{
				return new RestClient(baseUri, httpClient, serializer);
			}

			protected override void When()
			{
				restResponse = Sut.Process<ResultStub>(restRequest);
			}
			
			[Test]
			public void Should_call_serialize_on_serializer_with_the_payload()
			{
				serializer.Received().Serialize(Arg.Is<Employee>(e => {
					return e.Id == Guid.Empty && 
						   e.Name == "Bob" && 
						   e.Login == "boblogin";
				}));
			}
			
			[Test]
			public void Should_call_send_on_http_client_with_generated_http_request()
			{
				httpClient.Received().Send(Arg.Is<HttpRequest>(h => {
					return h.Url == new Uri(baseUri.ToString() + restRequest.Path) && 
						   h.Accept == jsonContentType && 
						   h.Method == restRequest.Method && 
						   h.ContentType == jsonContentType && 
						   h.Content == requestContent && 
						   h.ContentLength == requestContent.Length;
				}));
			}
			
			[Test]
			public void Should_call_deserialize_on_serializer_with_content_of_response()
			{
				serializer.Received().Deserialize<ResultStub>(Arg.Is<string>(s => {
					return s == responseContent;
				}));
			}
			
			[Test]
			public void Should_return_rest_response_generated_from_http_response()
			{
				Assert.That(restResponse.RestRequest, Is.EqualTo(restRequest));
				Assert.That(restResponse.ContentType, Is.EqualTo(jsonContentType));
				Assert.That(restResponse.Content, Is.EqualTo(responseContent));
				Assert.That(restResponse.ContentLength, Is.EqualTo(responseContent.Length));
				Assert.That(restResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
				Assert.That(restResponse.StatusDescription, Is.EqualTo(HttpStatusCode.Created.ToString()));
				Assert.That(restResponse.ContentDeserialized, Is.EqualTo(contentDeserialized));
			}
		}
	}
}
