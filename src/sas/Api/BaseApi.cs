using System.Runtime.CompilerServices;
using sas.Configurations;
using sas.Scenario;
using sas.Scenario.Defaulter;
using sas.Simulators;

namespace sas.Api;


public abstract class BaseApi<TStartup> : IDisposable, IAsyncDisposable
    where TStartup : class
{
    protected HttpClient HttpClient { get; }

    private readonly BaseScenario _scenario;
    private readonly ApiHost<TStartup> _host;
    private readonly bool _ownsHost;

    protected BaseApi(BaseScenario scenario, ISimulateBehaviour[] simulators, IEnrichConfiguration[] configurations)
    {
        _scenario = scenario;
        _host = new ApiHost<TStartup>(scenario, simulators, configurations);
        _ownsHost = true;

        HttpClient = _host.CreateClient();
    }

    /// <summary>
    /// Runs on a host built elsewhere - a pool, typically - instead of building one.
    /// The host is bound to this scenario and left alive when this API is disposed, so it can serve the next one.
    /// </summary>
    protected BaseApi(BaseScenario scenario, ApiHost<TStartup> borrowedHost)
    {
        _scenario = scenario;
        _host = borrowedHost;
        _ownsHost = false;

        borrowedHost.Bind(scenario);
        HttpClient = borrowedHost.CreateClient();
    }

    public T GetRequiredService<T>() where T : notnull => _host.GetRequiredService<T>();

    public TSimulator GetSimulator<TSimulator>() where TSimulator : ISimulateBehaviour =>
        _host.GetSimulator<TSimulator>();

    protected ValueFromScenarioDefaulter<T> Defaulting<T>(T? value, [CallerArgumentExpression(nameof(value))] string? message = null)
    {
        return new ValueFromScenarioDefaulter<T>(value, _scenario, message);
    }

    public void Dispose()
    {
        HttpClient.Dispose();

        if (_ownsHost)
        {
            _host.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        HttpClient.Dispose();

        if (_ownsHost)
        {
            await _host.DisposeAsync();
        }
    }
}
