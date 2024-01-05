using System.Net;
using NFluent;
using NSubstitute;
using sas.simulators.http.Http;

namespace sas.simulators.http.tests;

public class HttpMessageInterceptionHandlerShould
{
    [Fact]
    public async Task Propagate_Get_Request_to_stub_method_with_cancellation_token()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Get("path", Arg.Any<CancellationToken>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.GetAsync("/path");

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Propagate_Get_Request_to_stub_method_without_cancellation_token()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Get("path")
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.GetAsync("/path");

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Get_Request_to_stub_method_when_both_uri_and_stub_uri_contains_spaces()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Get("path?name=Jean Neige")
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.GetAsync("/path?name=Jean Neige");

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Get_Request_to_stub_method_when_both_uri_and_stub_uri_contains_encoded_spaces()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Get("path?name=Jean%20Neige")
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.GetAsync("/path?name=Jean%20Neige");

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Get_Request_to_stub_method_when_uri_contains_spaces_and_stub_uri_contains_encoded_spaces()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Get("path?name=Jean%20Neige")
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.GetAsync("/path?name=Jean Neige");

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Propagate_Get_Request_to_stub_method_when_uri_contains_encoded_spaces_and_stub_uri_contains_spaces()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Get("path?name=Jean Neige")
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.GetAsync("/path?name=Jean%20Neige");

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Propagate_Post_Request_to_stub_method_with_cancellation_token()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Post("path", Arg.Any<HttpContent>(), Arg.Any<CancellationToken>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.PostAsync("/path", new StringContent("content"));

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Post_Request_to_stub_method_without_cancellation_token()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Post("path", Arg.Any<HttpContent>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.PostAsync("/path", new StringContent("content"));

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Propagate_Post_Request_to_stub_method_when_both_uri_and_stub_uri_contains_spaces()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Post("path?name=Jean Neige", Arg.Any<HttpContent>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.PostAsync("/path?name=Jean Neige", new StringContent("content"));

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Post_Request_to_stub_method_when_both_uri_and_stub_uri_contains_encoded_spaces()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Post("path?name=Jean%20Neige", Arg.Any<HttpContent>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.PostAsync("/path?name=Jean%20Neige", new StringContent("content"));

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Post_Request_to_stub_method_when_uri_contains_spaces_and_stub_uri_contains_encoded_spaces()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Post("path?name=Jean%20Neige", Arg.Any<HttpContent>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.PostAsync("/path?name=Jean Neige", new StringContent("content"));

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Post_Request_to_stub_method_when_uri_contains_encoded_spaces_and_stub_uri_contains_spaces()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Post("path?name=Jean Neige", Arg.Any<HttpContent>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.PostAsync("/path?name=Jean%20Neige", new StringContent("content"));

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Propagate_Put_Request_to_stub_method_with_cancellation_token()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Put("path", Arg.Any<HttpContent>(), Arg.Any<CancellationToken>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.PutAsync("/path", new StringContent("content"));

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Put_Request_to_stub_method_without_cancellation_token()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Put("path", Arg.Any<HttpContent>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.PutAsync("/path", new StringContent("content"));

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Put_Request_to_stub_method_when_both_uri_and_stub_uri_contains_spaces()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Put("path?name=Jean Neige", Arg.Any<HttpContent>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.PutAsync("/path?name=Jean Neige", new StringContent("content"));

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Put_Request_to_stub_method_when_both_uri_and_stub_uri_contains_encoded_spaces()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Put("path?name=Jean%20Neige", Arg.Any<HttpContent>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.PutAsync("/path?name=Jean%20Neige", new StringContent("content"));

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Put_Request_to_stub_method_when_uri_contains_spaces_and_stub_uri_contains_encoded_spaces()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Put("path?name=Jean%20Neige", Arg.Any<HttpContent>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.PutAsync("/path?name=Jean Neige", new StringContent("content"));

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Put_Request_to_stub_method_when_uri_contains_encoded_spaces_and_stub_uri_contains_spaces()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Patch("path?name=Jean Neige", Arg.Any<HttpContent>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.PatchAsync("/path?name=Jean%20Neige", new StringContent("content"));

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Patch_Request_to_stub_method_with_cancellation_token()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Patch("path", Arg.Any<HttpContent>(), Arg.Any<CancellationToken>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.PatchAsync("/path", new StringContent("content"));

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Patch_Request_to_stub_method_without_cancellation_token()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Patch("path", Arg.Any<HttpContent>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.PatchAsync("/path", new StringContent("content"));

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Patch_Request_to_stub_method_when_both_uri_and_stub_uri_contains_spaces()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Patch("path?name=Jean Neige", Arg.Any<HttpContent>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.PatchAsync("/path?name=Jean Neige", new StringContent("content"));

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Patch_Request_to_stub_method_when_both_uri_and_stub_uri_contains_encoded_spaces()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Patch("path?name=Jean%20Neige", Arg.Any<HttpContent>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.PatchAsync("/path?name=Jean%20Neige", new StringContent("content"));

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Patch_Request_to_stub_method_when_uri_contains_spaces_and_stub_uri_contains_encoded_spaces()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Patch("path?name=Jean%20Neige", Arg.Any<HttpContent>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.PatchAsync("/path?name=Jean Neige", new StringContent("content"));

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Patch_Request_to_stub_method_when_uri_contains_encoded_spaces_and_stub_uri_contains_spaces()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Patch("path?name=Jean Neige", Arg.Any<HttpContent>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.PatchAsync("/path?name=Jean%20Neige", new StringContent("content"));

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Propagate_Delete_Request_to_stub_method_with_cancellation_token()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Delete("path", Arg.Any<CancellationToken>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.DeleteAsync("/path");

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Propagate_Delete_Request_to_stub_method_without_cancellation_token()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Delete("path")
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.DeleteAsync("/path");

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Delete_Request_to_stub_method_when_both_uri_and_stub_uri_contains_spaces()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Delete("path?name=Jean Neige")
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.DeleteAsync("/path?name=Jean Neige");

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Delete_Request_to_stub_method_when_both_uri_and_stub_uri_contains_encoded_spaces()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Delete("path?name=Jean%20Neige")
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.DeleteAsync("/path?name=Jean%20Neige");

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Delete_Request_to_stub_method_when_uri_contains_spaces_and_stub_uri_contains_encoded_spaces()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Delete("path?name=Jean%20Neige")
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.DeleteAsync("/path?name=Jean Neige");

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Propagate_Delete_Request_to_stub_method_when_uri_contains_encoded_spaces_and_stub_uri_contains_spaces()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Delete("path?name=Jean Neige")
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.DeleteAsync("/path?name=Jean%20Neige");

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }
        
    [Fact]
    public async Task Propagate_Send_Request_to_stub_method_with_cancellation_token()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();

        httpClientStub.Send(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
            
        var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Trace, "path"));

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Propagate_Send_Request_to_stub_method_without_cancellation_token()
    {
        var (httpClient,httpClientStub) = SetupHttpClient();
            
        httpClientStub.Send(Arg.Any<HttpRequestMessage>())
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
        var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Trace, "path"));

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Intercept_Delete_payload_when_it_is_provided()
    {
        var (httpClient, httpClientStub) = SetupHttpClient();

        var expectedPayload = "Je sais où tu te caches !";
        var payloadReadFromRequest = string.Empty;

        httpClientStub.Delete(Arg.Any<string>(), Arg.Any<HttpContent>())
            .Returns<Task<HttpResponseMessage?>>(async callInfo =>
            {
                var receivedPayload = callInfo.Arg<HttpContent>();
                payloadReadFromRequest = await receivedPayload.ReadAsStringAsync();
                return new HttpResponseMessage(HttpStatusCode.Accepted);
            });

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, "path")
        {
            Content = new StringContent(expectedPayload)
        };

        var response = await httpClient.SendAsync(httpRequestMessage);

        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Accepted);
        Check.That(payloadReadFromRequest).IsEqualTo(expectedPayload);
    }
        
    private static (HttpClient, IDeferHttpRequestHandling) SetupHttpClient()
    {
        var httpClientStub = Substitute.For<IDeferHttpRequestHandling>();

        var baseUri = new Uri("http://test/");
        var httpClient = new HttpClient(new HttpMessageInterceptionHandler(httpClientStub, baseUri, _ => ""))
        {
            BaseAddress = baseUri
        };
        return (httpClient, httpClientStub);
    }
}