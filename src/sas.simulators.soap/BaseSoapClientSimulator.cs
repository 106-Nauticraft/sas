using System.ServiceModel;
using System.ServiceModel.Channels;
using HttpRequest.Spy;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
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

    private readonly IDeferHttpRequestHandling _httpClient;
    private readonly HttpRequestSpy _httpRequestSpy;
    
    protected SoapClientMessageHandler SoapClient { get; }
    
    protected SoapRequestSpy SoapRequestSpy { get; }

    protected BaseSoapClientSimulator()
    {
        _httpClient = Substitute.For<IDeferHttpRequestHandling>();
        _httpRequestSpy = HttpRequestSpy.Create();
        SoapClient = new SoapClientMessageHandler(_httpClient);
        SoapRequestSpy = new SoapRequestSpy(Uri, _httpRequestSpy);

    }
    
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

        var httpMessageHandler = new SpyHttpMessageHandler(_httpRequestSpy,
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