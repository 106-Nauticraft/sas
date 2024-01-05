using HttpRequestSpy;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using sas.Scenario;
using sas.Simulators;
using sas.simulators.http.Http;

namespace sas.simulators.http;

public abstract class HttpClientSimulator<THttpClient> : ISimulateBehaviour
    where THttpClient : class
{
    protected IDeferHttpRequestHandling HttpClient { get; } = Substitute.For<IDeferHttpRequestHandling>();

    protected HttpRequestSpy.HttpRequestSpy Spy { get; } = HttpRequestSpy.HttpRequestSpy.Create();

    private readonly Uri _baseUri = new($"https://{typeof(THttpClient).Name.ToLower()}-tests/");

    public void RegisterTo(IServiceCollection services, BaseScenario scenario)
    {
        if (scenario is NoScenario)
        {
            return;
        }

        // Using Transient here to avoid crashing when trying to inject a Singleton into a Scoped service.
        services.AddTransient(BuildHttpClient);
        Simulate(scenario);
    }

    private THttpClient BuildHttpClient(IServiceProvider provider)
    {
        var constructor = typeof(THttpClient).GetConstructors().OrderByDescending(c => c.GetParameters().Length)
            .Last();

        var parameters = constructor.GetParameters().Select(param =>
        {
            if (param.ParameterType == typeof(System.Net.Http.HttpClient))
            {
                return new System.Net.Http.HttpClient(new SpyHttpMessageHandler(Spy,
                    new HttpMessageInterceptionHandler(HttpClient, _baseUri, request => $"A {request.Method.Method} to {request.RequestUri} was never stubbed")))
                {
                    BaseAddress = _baseUri
                };
            }

            return provider.GetRequiredService(param.ParameterType);
        }).ToArray();

        return (THttpClient) constructor.Invoke(parameters);
    }

    protected abstract void Simulate(BaseScenario scenario);
}