using HttpRequest.Spy;
using Microsoft.Extensions.DependencyInjection;
using sas.Scenario;
using sas.Simulators;
using sas.simulators.http.Http;

namespace sas.simulators.http;

public abstract class AbstractHttpClientSimulator<THttpClient> : ISimulateBehaviour, IBindScenario
    where THttpClient : class
{
    protected abstract IDeferHttpRequestHandling HttpClient { get; }

    protected HttpRequestSpy Spy { get; private set; } = HttpRequestSpy.Create();

    private readonly Uri _baseUri = new($"https://{typeof(THttpClient).Name.ToLower()}-tests/");

    public void RegisterTo(IServiceCollection services, BaseScenario scenario)
    {
        // Using Transient here to avoid crashing when trying to inject a Singleton into a Scoped service.
        services.AddTransient(BuildHttpClient);
        Bind(scenario);
    }

    private THttpClient BuildHttpClient(IServiceProvider provider)
    {
        var constructor = typeof(THttpClient).GetConstructors().OrderByDescending(c => c.GetParameters().Length)
            .Last();

        var parameters = constructor.GetParameters().Select(param =>
        {
            if (param.ParameterType == typeof(HttpClient))
            {
                return new HttpClient(new SpyHttpMessageHandler(Spy,
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
    public void Bind(BaseScenario scenario)
    {
        Reset();
        if (scenario is not NoScenario)
            Simulate(scenario);
    }

    protected virtual void Reset()
    {
        Spy = HttpRequestSpy.Create();
        ResetHttpClient();
    }

    protected virtual void ResetHttpClient() { }
}