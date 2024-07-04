using System.ServiceModel;
using System.ServiceModel.Channels;
using HttpRequest.Spy;
using Microsoft.Extensions.DependencyInjection;
using sas.Scenario;
using sas.Simulators;
using sas.simulators.http.Http;
using sas.simulators.soap.Soap;
using sas.simulators.soap.Soap.Spy;

namespace sas.simulators.soap;

public abstract class BaseSoapClientSimulator<TChannel, TClient> : ISimulateBehaviour
    where TChannel: class
    where TClient : ClientBase<TChannel>, TChannel
{
    private static readonly Uri Uri = new($"http://endpoint-for-tests/{typeof(TChannel).Name}.asmx");
    
    private static EndpointAddress EndpointAddress => new(Uri);
    
    private readonly Binding _binding = new BasicHttpBinding();

    private class InternalHttpClient : IDeferHttpRequestHandling, IHandleSoapHttpRequest
    {
        public Task<HttpResponseMessage?> Get(string uri)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage?> Get(string uri, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage?> Post(string uri, HttpContent? content)
        {
            return HandlePost(uri, content);
        }

        public Task<HttpResponseMessage?> Post(string uri, HttpContent? content, CancellationToken _)
        {
            return HandlePost(uri, content);
        }

        public Task<HttpResponseMessage?> Put(string uri, HttpContent? content)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage?> Put(string uri, HttpContent? content, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage?> Patch(string uri, HttpContent? content)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage?> Patch(string uri, HttpContent? content, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage?> Delete(string uri)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage?> Delete(string uri, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage?> Delete(string uri, HttpContent content)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage?> Delete(string uri, HttpContent content, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage?> Send(HttpRequestMessage request)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage?> Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        
        private async Task<HttpResponseMessage?> HandlePost(string _, HttpContent? content)
        {
            if (content is null)
            {
                ArgumentNullException.ThrowIfNull(content);
            }
            
            foreach (var (requestType, handler) in _handlers)
            {
                if (content.IsSoapRequest(requestType))
                {
                    return await handler(content);
                }
            }
            
            return null;
        }

        private readonly Dictionary<Type, Func<HttpContent, Task<HttpResponseMessage?>>> _handlers = new();
        
        public IHandleSoapHttpRequest HandleSoapRequest<TRequest, TResponse>(Func<TRequest, TResponse> handle)
            where TRequest: class
        {
            _handlers[typeof(TRequest)] = content => OnSoapRequest(content, handle);
            
            return this;
        }
    
        private static async Task<HttpResponseMessage?> OnSoapRequest<TRequest, TResponse>(HttpContent content, Func<TRequest, TResponse> handle) 
        {
            var soapVersion = await content.GetSoapMessageVersion();

            var request = await content.
                ReadFromXmlSoapBodyAsync<TRequest>(soapVersion);
               
            var response = handle(request!);

            var httpContent = new SoapXmlContent(response, soapVersion);
            return new HttpResponseMessage
            {
                Content = httpContent,
            };
        }
    }
    
    private readonly IDeferHttpRequestHandling _httpClient = new InternalHttpClient();
    private HttpRequestSpy HttpRequestSpy { get; } = HttpRequestSpy.Create();

    protected IHandleSoapHttpRequest SoapClient => (IHandleSoapHttpRequest)_httpClient;

    private SoapRequestSpy? _soapRequestSpy;
    protected SoapRequestSpy SoapRequestSpy => _soapRequestSpy ??= new SoapRequestSpy(Uri, HttpRequestSpy);

    public void RegisterTo(IServiceCollection services, BaseScenario scenario)
    {
        var existing = services.SingleOrDefault(sd => sd.ServiceType == typeof(TChannel));

        if (existing is not null)
        {
            services.Remove(existing);
        }
        
        services.AddSingleton<TChannel>(_ => BuildStubbedSoapClient());
        
        Simulate(scenario);
    }

    private TClient BuildStubbedSoapClient()
    {
        var soapClient = (TClient?)Activator.CreateInstance(typeof(TClient), _binding, EndpointAddress);

        if (soapClient is null)
        {
            throw new SoapClientBuildException(typeof(TClient));
        }

        var httpMessageHandler = new SpyHttpMessageHandler(HttpRequestSpy,
            new HttpMessageInterceptionHandler(_httpClient, Uri, BuildUnexpectedSoapMessageError));

        soapClient.Endpoint.EndpointBehaviors.Add(new HttpMessageInterceptionEndpointBehaviour(httpMessageHandler));
        
        return soapClient;
    }

    private static string BuildUnexpectedSoapMessageError(HttpRequestMessage request)
    {
        if (request.Content is null || !request.Content.IsSoapRequest())
        {
            return $"This non-soap request, {request.Method.Method} to {request.RequestUri} was never stubbed";
        }

        var unexpectedRequestsType = request.Content.GetSoapRequestsTypes();
        
        return $"A Soap http request including {string.Join(",", unexpectedRequestsType)} was never stubbed.";
    }

    protected abstract void Simulate(BaseScenario scenario);
}