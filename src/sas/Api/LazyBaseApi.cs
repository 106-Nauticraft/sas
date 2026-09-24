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
/// Either way, the scenario applies when the client is built, not when this API is created.
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
    private readonly ApiHost<TStartup>? _borrowedHost;
    private ApiHost<TStartup>? _host;

    /// <summary>
    /// Builds its own <see cref="ApiHost{TStartup}"/> on the first call to <see cref="BuildHttpClient()"/>. Will dispose it when this instance is disposed.
    /// </summary>
    protected LazyBaseApi(BaseScenario scenario,
        ISimulateBehaviour[] simulators,
        IEnrichConfiguration[] additionalConfigurations)
    {
        _scenario = scenario;
        _simulators = simulators;
        _additionalConfigurations = additionalConfigurations;
    }

    /// <summary>
    /// Runs on a host built elsewhere instead of building one.
    /// The host is bound to this scenario on the first call to <see cref="BuildHttpClient()"/>, and left alive when this API is disposed, so it can serve the next one.
    /// </summary>
    protected LazyBaseApi(BaseScenario scenario, ApiHost<TStartup> borrowedHost)
    {
        _scenario = scenario;
        _simulators = [];
        _additionalConfigurations = [];
        _borrowedHost = borrowedHost;
    }

    private bool OwnsHost => _borrowedHost is null;

    protected HttpClient BuildHttpClient() => Build(postSetup: null);

    /// <param name="postSetup">Last say on the services. Only applies to a host this API builds.</param>
    /// <exception cref="InvalidOperationException">When this API runs on a borrowed host, which is already built.</exception>
    protected HttpClient BuildHttpClient(Action<IServiceCollection> postSetup) => Build(postSetup);

    private HttpClient Build(Action<IServiceCollection>? postSetup)
    {
        if (_borrowedHost is not null && postSetup is not null)
        {
            // Ignoring it would stub services the application can never resolve, silently.
            throw new InvalidOperationException(
                "A borrowed host is already built, so its services cannot be set up anymore. " +
                $"Give the post setup to the {nameof(ApiHost<TStartup>)} constructor instead, or call {nameof(BuildHttpClient)}() without it.");
        }

        if (_httpClient is not null)
        {
            return _httpClient;
        }

        if (_borrowedHost is not null)
        {
            _borrowedHost.Bind(_scenario);
            _host = _borrowedHost;
        }
        else
        {
            _host = new ApiHost<TStartup>(_scenario, _simulators, _additionalConfigurations, postSetup ?? (_ => { }));
        }

        return _httpClient = _host.CreateClient();
    }

    // An owned host does not exist before the client is built, so its simulators are looked up here.
    public TSimulator GetSimulator<TSimulator>() where TSimulator : ISimulateBehaviour
    {
        if (_borrowedHost is not null)
        {
            return _borrowedHost.GetSimulator<TSimulator>();
        }

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

        if (OwnsHost)
        {
            _host?.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        _httpClient?.Dispose();

        if (OwnsHost && _host != null)
        {
            await _host.DisposeAsync();
        }
    }
}
