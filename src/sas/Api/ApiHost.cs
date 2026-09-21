using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using sas.Configurations;
using sas.Scenario;
using sas.Simulators;

namespace sas.Api;

/// <summary>
/// The web application an API talks to: built once from a set of simulators, then re-bindable to another scenario.
/// <para>
/// A <see cref="BaseApi{TStartup}"/> built the usual way owns its host and disposes it. A host built on its own can
/// instead be lent to successive APIs. Makes pooling possible: building the host is the expensive part, binding a scenario to it is not.
/// </para>
/// </summary>
public sealed class ApiHost<TStartup> : IDisposable, IAsyncDisposable
    where TStartup : class
{
    private readonly ISimulateBehaviour[] _simulators;
    private readonly WebApplicationFactory<TStartup> _factory;

    public ApiHost(BaseScenario scenario, ISimulateBehaviour[] simulators, IEnrichConfiguration[] configurations)
        : this(scenario, simulators, configurations, _ => { }) { }

    public ApiHost(BaseScenario scenario, ISimulateBehaviour[] simulators, IEnrichConfiguration[] configurations, Action<IServiceCollection> postSetup)
    {
        _simulators = simulators;

        _factory = new WebApplicationFactory<TStartup>()
            .WithWebHostBuilder(webHost => webHost
                .ConfigureAppConfiguration(ConfigureAppConfiguration(configurations))
                .ConfigureTestServices(services =>
                {
                    RegisterSimulators(services, scenario);
                    postSetup(services);
                }));
    }

    public IServiceProvider Services => _factory.Services;

    public HttpClient CreateClient() => _factory.CreateClient();

    /// <summary>
    /// Replays every simulator against another scenario.
    /// </summary>
    /// <exception cref="SimulatorCannotBeReboundException">
    /// When a simulator does not implement <see cref="IBindScenario"/>. Such a simulator only ever sees the scenario
    /// the host was built with, so binding would silently leave it behind.
    /// </exception>
    public void Bind(BaseScenario scenario)
    {
        var notRebindable = _simulators.Where(simulator => simulator is not IBindScenario).ToArray();

        if (notRebindable.Length > 0)
        {
            throw new SimulatorCannotBeReboundException(notRebindable);
        }

        // The host builds lazily and RegisterTo binds the construction scenario and will apply the former scenario.
        // Forcing the build now makes sure this Bind is the last one applied.
        _ = _factory.Services;

        foreach (var simulator in _simulators.OfType<IBindScenario>())
        {
            simulator.Bind(scenario);
        }
    }

    public T GetRequiredService<T>() where T : notnull
    {
        try
        {
            return Services.GetRequiredService<T>();
        }
        // For scoped services, an exception is thrown and the IServiceScopeFactory is needed.
        catch (InvalidOperationException)
        {
            using var scope = Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            return scope.ServiceProvider.GetRequiredService<T>();
        }
    }

    public TSimulator GetSimulator<TSimulator>() where TSimulator : ISimulateBehaviour
    {
        var foundSimulator = _simulators.SingleOrDefault(simulator => simulator is TSimulator);

        if (foundSimulator is null)
        {
            throw new InvalidOperationException($"No Simulator of type {typeof(TSimulator).Name} found. Make sure it is provided in the constructor");
        }

        return (TSimulator) foundSimulator;
    }

    private void RegisterSimulators(IServiceCollection services, BaseScenario scenario)
    {
        foreach (var simulator in _simulators)
        {
            simulator.RegisterTo(services, scenario);
        }
    }

    private static Action<IConfigurationBuilder> ConfigureAppConfiguration(IEnrichConfiguration[] additionalConfigurations) =>
        configurationBuilder =>
        {
            configurationBuilder.Sources.Clear();
            configurationBuilder.Sources.Add(new JsonConfigurationSource
            {
                Path = "appsettings.json",
                Optional = false,
            });
            var additionalConfiguration = BuildAdditionalConfiguration(additionalConfigurations);
            configurationBuilder.AddConfiguration(additionalConfiguration);
        };

    private static IConfigurationRoot BuildAdditionalConfiguration(IEnumerable<IEnrichConfiguration> additionalConfigurations)
    {
        var configurationBuilder = new ConfigurationBuilder();

        foreach (var configurationEnricher in additionalConfigurations)
        {
            configurationEnricher.Enrich(configurationBuilder);
        }

        return configurationBuilder.Build();
    }

    public void Dispose()
    {
        _factory.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _factory.DisposeAsync();
    }
}
