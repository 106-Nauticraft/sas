using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using sas.Configurations;
using sas.Scenario;
using sas.Scenario.Defaulter;
using sas.Simulators;

namespace sas.Api;

/// <summary>
/// This is an abstraction that is exposed to outside world. Meant to be used once per test ; 1 test = 1 LazyBaseApi.
/// <para>
/// Can either use a borrowed <see cref="ApiHost{TStartup}"/> or build and own its own.
/// In the latter case, the <see cref="ApiHost{TStartup}"/> will be disposed of when this <see cref="LazyBaseApi{TStartup}"/> is disposed.
/// </para>
/// </summary>
/// <typeparam name="TStartup">Entry point (e.g. Program)</typeparam>
public abstract class LazyBaseApi<TStartup> : IDisposable, IAsyncDisposable
    where TStartup : class
{
    private HttpClient? _httpClient;
    private readonly BaseScenario _scenario;
    private readonly ISimulateBehaviour[] _simulators;
    private readonly IEnrichConfiguration[] _additionalConfigurations;
    private ApiHost<TStartup>? _host;

    protected LazyBaseApi(BaseScenario scenario,
        ISimulateBehaviour[] simulators,
        IEnrichConfiguration[] additionalConfigurations)
    {
        _scenario = scenario;
        _simulators = simulators;
        _additionalConfigurations = additionalConfigurations;
    }

    protected HttpClient BuildHttpClient(Action<IServiceCollection> postSetup)
    {
        if (_httpClient is not null)
        {
            return _httpClient;
        }

        _host = new ApiHost<TStartup>(_scenario, _simulators, _additionalConfigurations, postSetup);

        return _httpClient = _host.CreateClient();
    }

    // Must reimplement the logic found in ApiHost.GetSimulator because _host might still be null.
    public TSimulator GetSimulator<TSimulator>() where TSimulator : ISimulateBehaviour
    {
        var foundSimulator = _simulators.SingleOrDefault(simulator => simulator is TSimulator);

        if (foundSimulator is null)
        {
            throw new InvalidOperationException(
                $"No Simulator of type {typeof(TSimulator).Name} found. Make sure it is provided in the constructor");
        }

        return (TSimulator) foundSimulator;
    }

    protected ValueFromScenarioDefaulter<T> Defaulting<T>(T? value, [CallerArgumentExpression(nameof(value))] string? message = null)
    {
        return new ValueFromScenarioDefaulter<T>(value, _scenario, message);
    }

    public T GetRequiredService<T>() where T : notnull
    {
        if (_host == null)
        {
            throw new NullReferenceException($"API was not built ; please use {nameof(BuildHttpClient)} before calling this method.");
        }

        return _host.GetRequiredService<T>();
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
        _host?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        _httpClient?.Dispose();

        if (_host != null)
        {
            await _host.DisposeAsync();
        }
    }
}
