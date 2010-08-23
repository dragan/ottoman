using System;
using System.Net;

using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SineSignal.Ottoman.Specs.Framework;

using SineSignal.Ottoman.Exceptions;
using SineSignal.Ottoman.Http;
using SineSignal.Ottoman.Serialization;

namespace SineSignal.Ottoman.Specs.Http
{
	public class RestClientSpecs
	{
		public class When_receiving_an_expected_status_code : ConcernFor<RestClient>
		{
			protected const string jsonType = "application/json";
			
			protected Uri baseUri;
			protected IHttpClient httpClient;
			protected ISerializer serializer;
			protected string acceptType;
			protected string responseContentType;
			protected string responseContent;
			protected ResultStub contentDeserialized;
			protected HttpStatusCode successStatusCode;
			protected RestResponse<ResultStub> restResponse;
			
			protected override void Given()
			{
				baseUri = new Uri("http://127.0.0.1:5984");
				httpClient = Fake<IHttpClient>();
				serializer = Fake<ISerializer>();
				acceptType = jsonType;
				responseContentType = jsonType;
				responseContent = "{\"status\":\"completed\"}";
				contentDeserialized = new ResultStub();
				
				serializer.Deserialize<ResultStub>(Arg.Any<string>()).Returns(contentDeserialized);
			}
			
			public override RestClient CreateSystemUnderTest()
			{
				return new RestClient(baseUri, httpClient, serializer);
			}
		}
		
		public class When_processing_a_post_request : When_receiving_an_expected_status_code
		{
			private Employee payload;
			private RestRequest restRequest;
			private string requestContentType;
			private string requestContent;
			
			protected override void Given()
			{
				base.Given();
				
				payload = new Employee { Name = "Bob", Login = "boblogin" };
				restRequest = new RestRequest { Path = "some/path", Method = HttpMethod.Post, Payload = payload };
				requestContentType = jsonType;
				requestContent = "{\"Id\":\"" + Guid.Empty + "\",\"Name\":\"Bob\",\"Login\":\"boblogin\"}";
				
				serializer.Serialize(Arg.Any<object>()).Returns(requestContent);
				
				successStatusCode = HttpStatusCode.Created;
				
				httpClient.Send(Arg.Any<HttpRequest>()).Returns(new HttpResponse {
					ContentType = responseContentType,
					ContentLength = responseContent.Length,
					Content = responseContent,
					StatusCode = successStatusCode,
					StatusDescription = successStatusCode.ToString()
				});
			}
			
			protected override void When()
			{
				restResponse = Sut.Process<ResultStub>(restRequest, successStatusCode);
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
						   h.Accept == acceptType && 
						   h.Method == restRequest.Method && 
						   h.ContentType == requestContentType && 
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
				Assert.That(restResponse.ContentType, Is.EqualTo(responseContentType));
				Assert.That(restResponse.Content, Is.EqualTo(responseContent));
				Assert.That(restResponse.ContentLength, Is.EqualTo(responseContent.Length));
				Assert.That(restResponse.StatusCode, Is.EqualTo(successStatusCode));
				Assert.That(restResponse.StatusDescription, Is.EqualTo(successStatusCode.ToString()));
				Assert.That(restResponse.ContentDeserialized, Is.EqualTo(contentDeserialized));
			}
		}
		
		public class When_processing_a_get_request : When_receiving_an_expected_status_code
		{
			private RestRequest restRequest;
			private string requestContent;
			private string requestContentType;
			
			protected override void Given()
			{
				base.Given();
				
				restRequest = new RestRequest { Path = "some/path", Method = HttpMethod.Get, Payload = null };
				requestContent = "";
				requestContentType = "";
				
				successStatusCode = HttpStatusCode.OK;
				
				httpClient.Send(Arg.Any<HttpRequest>()).Returns(new HttpResponse {
					ContentType = responseContentType,
					ContentLength = responseContent.Length,
					Content = responseContent,
					StatusCode = successStatusCode,
					StatusDescription = successStatusCode.ToString()
				});
			}
			
			protected override void When()
			{
				restResponse = Sut.Process<ResultStub>(restRequest, successStatusCode);
			}
			
			[Test]
			public void Should_not_call_serialize_on_serializer_with_the_payload()
			{
				serializer.DidNotReceive().Serialize(null);
			}
			
			[Test]
			public void Should_call_send_on_http_client_with_generated_http_request()
			{
				httpClient.Received().Send(Arg.Is<HttpRequest>(h => {
					return h.Url == new Uri(baseUri.ToString() + restRequest.Path) && 
						   h.Accept == responseContentType && 
						   h.Method == restRequest.Method && 
						   h.ContentType == requestContentType && 
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
				Assert.That(restResponse.ContentType, Is.EqualTo(responseContentType));
				Assert.That(restResponse.Content, Is.EqualTo(responseContent));
				Assert.That(restResponse.ContentLength, Is.EqualTo(responseContent.Length));
				Assert.That(restResponse.StatusCode, Is.EqualTo(successStatusCode));
				Assert.That(restResponse.StatusDescription, Is.EqualTo(successStatusCode.ToString()));
				Assert.That(restResponse.ContentDeserialized, Is.EqualTo(contentDeserialized));
			}
		}
		
		public class When_processing_a_put_request_with_no_payload : When_receiving_an_expected_status_code
		{
			private RestRequest restRequest;
			private string requestContent;
			private string requestContentType;
			
			protected override void Given()
			{
				base.Given();
				
				restRequest = new RestRequest { Path = "some/path", Method = HttpMethod.Put, Payload = null };
				requestContent = "";
				requestContentType = "";
				
				successStatusCode = HttpStatusCode.Created;
				
				httpClient.Send(Arg.Any<HttpRequest>()).Returns(new HttpResponse {
					ContentType = responseContentType,
					ContentLength = responseContent.Length,
					Content = responseContent,
					StatusCode = successStatusCode,
					StatusDescription = successStatusCode.ToString()
				});
			}
			
			protected override void When()
			{
				restResponse = Sut.Process<ResultStub>(restRequest, successStatusCode);
			}
			
			[Test]
			public void Should_not_call_serialize_on_serializer_with_the_payload()
			{
				serializer.DidNotReceive().Serialize(null);
			}
			
			[Test]
			public void Should_call_send_on_http_client_with_generated_http_request()
			{
				httpClient.Received().Send(Arg.Is<HttpRequest>(h => {
					return h.Url == new Uri(baseUri.ToString() + restRequest.Path) && 
						   h.Accept == responseContentType && 
						   h.Method == restRequest.Method && 
						   h.ContentType == requestContentType && 
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
				Assert.That(restResponse.ContentType, Is.EqualTo(responseContentType));
				Assert.That(restResponse.Content, Is.EqualTo(responseContent));
				Assert.That(restResponse.ContentLength, Is.EqualTo(responseContent.Length));
				Assert.That(restResponse.StatusCode, Is.EqualTo(successStatusCode));
				Assert.That(restResponse.StatusDescription, Is.EqualTo(successStatusCode.ToString()));
				Assert.That(restResponse.ContentDeserialized, Is.EqualTo(contentDeserialized));
			}
		}
		
		public class When_processing_a_delete_request : When_receiving_an_expected_status_code
		{
			private RestRequest restRequest;
			private string requestContent;
			private string requestContentType;
			
			protected override void Given()
			{
				base.Given();
				
				restRequest = new RestRequest { Path = "some/path", Method = HttpMethod.Delete, Payload = null };
				requestContent = "";
				requestContentType = "";
				
				successStatusCode = HttpStatusCode.OK;
				
				httpClient.Send(Arg.Any<HttpRequest>()).Returns(new HttpResponse {
					ContentType = responseContentType,
					ContentLength = responseContent.Length,
					Content = responseContent,
					StatusCode = successStatusCode,
					StatusDescription = successStatusCode.ToString()
				});
			}
			
			protected override void When()
			{
				restResponse = Sut.Process<ResultStub>(restRequest, successStatusCode);
			}
			
			[Test]
			public void Should_not_call_serialize_on_serializer_with_the_payload()
			{
				serializer.DidNotReceive().Serialize(null);
			}
			
			[Test]
			public void Should_call_send_on_http_client_with_generated_http_request()
			{
				httpClient.Received().Send(Arg.Is<HttpRequest>(h => {
					return h.Url == new Uri(baseUri.ToString() + restRequest.Path) && 
						   h.Accept == responseContentType && 
						   h.Method == restRequest.Method && 
						   h.ContentType == requestContentType && 
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
				Assert.That(restResponse.ContentType, Is.EqualTo(responseContentType));
				Assert.That(restResponse.Content, Is.EqualTo(responseContent));
				Assert.That(restResponse.ContentLength, Is.EqualTo(responseContent.Length));
				Assert.That(restResponse.StatusCode, Is.EqualTo(successStatusCode));
				Assert.That(restResponse.StatusDescription, Is.EqualTo(successStatusCode.ToString()));
				Assert.That(restResponse.ContentDeserialized, Is.EqualTo(contentDeserialized));
			}
		}
	}
	
	public class When_receiving_an_unexpected_status_code : ConcernFor<RestClient>
	{
		private const string jsonType = "application/json";
		
		private Uri baseUri;
		private IHttpClient httpClient;
		private ISerializer serializer;
		private RestRequest restRequest;
		private string requestContent;
		private string requestContentType;
		protected string responseContentType;
		protected string responseContent;
		private HttpStatusCode successStatusCode;
		private HttpStatusCode unexpectedStatusCode;
		private UnexpectedHttpResponseException unexpectedResponseException;
		
		protected override void Given()
		{
			baseUri = new Uri("http://127.0.0.1:5984");
			httpClient = Fake<IHttpClient>();
			serializer = Fake<ISerializer>();
			
			restRequest = new RestRequest { Path = "some/path", Method = HttpMethod.Get, Payload = null };
			requestContent = "";
			requestContentType = "";
			responseContentType = jsonType;
			responseContent = "{\"error\":\"not_found\",\"reason\":\"no_db_file\"}";
			
			successStatusCode = HttpStatusCode.OK;
			unexpectedStatusCode = HttpStatusCode.NotFound;
				
			httpClient.Send(Arg.Any<HttpRequest>()).Returns(new HttpResponse {
				ContentType = responseContentType,
				ContentLength = responseContent.Length,
				Content = responseContent,
				StatusCode = unexpectedStatusCode,
				StatusDescription = unexpectedStatusCode.ToString()
			});
		}
		
		public override RestClient CreateSystemUnderTest ()
		{
			return new RestClient(baseUri, httpClient, serializer);
		}
		
		protected override void When()
		{
			try
			{
				Sut.Process<ResultStub>(restRequest, successStatusCode);
			}
			catch (UnexpectedHttpResponseException e)
			{
				unexpectedResponseException = e;
			}
		}
		
		[Test]
		public void Should_not_call_serialize_on_serializer_with_the_payload()
		{
			serializer.DidNotReceive().Serialize(null);
		}
		
		[Test]
		public void Should_call_send_on_http_client_with_generated_http_request()
		{
			httpClient.Received().Send(Arg.Is<HttpRequest>(h => {
				return h.Url == new Uri(baseUri.ToString() + restRequest.Path) && 
					   h.Accept == responseContentType && 
					   h.Method == restRequest.Method && 
					   h.ContentType == requestContentType && 
					   h.Content == requestContent && 
					   h.ContentLength == requestContent.Length;
			}));
		}
		
		[Test]
		public void Should_not_call_deserialize_on_serializer_with_content_of_response()
		{
			serializer.DidNotReceive().Deserialize<ResultStub>(Arg.Is<string>(s => {
				return s == responseContent;
			}));
		}
		
		[Test]
		public void Should_throw_unexpected_http_response_exception()
		{
			Assert.That(unexpectedResponseException, Is.Not.Null);
		}
		
		[Test]
		public void Should_populate_raw_response_on_unexpected_http_response_exception_with_http_response_returned_by_send()
		{
			Assert.That(unexpectedResponseException.RawResponse.ContentType, Is.EqualTo(responseContentType));
			Assert.That(unexpectedResponseException.RawResponse.Content, Is.EqualTo(responseContent));
			Assert.That(unexpectedResponseException.RawResponse.ContentLength, Is.EqualTo(responseContent.Length));
			Assert.That(unexpectedResponseException.RawResponse.StatusCode, Is.EqualTo(unexpectedStatusCode));
			Assert.That(unexpectedResponseException.RawResponse.StatusDescription, Is.EqualTo(unexpectedStatusCode.ToString()));
		}
	}
}
