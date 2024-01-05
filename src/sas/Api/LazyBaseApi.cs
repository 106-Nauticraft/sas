using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using sas.Configurations;
using sas.Scenario;
using sas.Scenario.Defaulter;
using sas.Simulators;

namespace sas.Api;

public abstract class LazyBaseApi<TStartup> where TStartup : class
{
    private HttpClient? _httpClient;
    private readonly BaseScenario _scenario;
    private readonly ISimulateBehaviour[] _simulators;
    private readonly IEnrichConfiguration[] _additionalConfigurations;

    protected LazyBaseApi(BaseScenario scenario,
        ISimulateBehaviour[] simulators,
        IEnrichConfiguration[] additionalConfigurations)
    {
        _scenario = scenario;
        _simulators = simulators;
        _additionalConfigurations = additionalConfigurations;
    }

    private void InitWebApplication(Action<IServiceCollection> postSetup)
    {
        var factory = new WebApplicationFactory<TStartup>();

        factory = factory.WithWebHostBuilder(webHost => webHost
            .ConfigureAppConfiguration(ConfigureAppConfiguration(_additionalConfigurations))
            .ConfigureTestServices(services =>
                {
                    ConfigureTestServices(_simulators, _scenario)(services);
                    postSetup(services);
                }
            ));
        
        _httpClient = factory.CreateClient();
    }
    
    private static Action<IConfigurationBuilder>ConfigureAppConfiguration(IEnrichConfiguration[] additionalConfigurations) =>
        configurationBuilder =>
        {
            configurationBuilder.Sources.Clear();
            configurationBuilder.Sources.Add(new JsonConfigurationSource
            {
                Path = "appSettings.json",
                Optional = false
            });

            var additionalConfiguration = BuildAdditionalConfiguration(additionalConfigurations);
            configurationBuilder.AddConfiguration(additionalConfiguration);
        };

    private static IConfigurationRoot BuildAdditionalConfiguration(IEnumerable<IEnrichConfiguration> configurationEnrichers)
    {
        var configurationBuilder = new ConfigurationBuilder();

        foreach (var configurationEnricher in configurationEnrichers)
        {
            configurationEnricher.Enrich(configurationBuilder);
        }

        return configurationBuilder.Build();
    }
    
    protected HttpClient BuildHttpClient(Action<IServiceCollection> postSetup) 
    {
        if (_httpClient is not null)
        {
            return _httpClient;
        }

        InitWebApplication(postSetup);
            
        return _httpClient!;
        
    }

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

    private static Action<IServiceCollection> ConfigureTestServices(IEnumerable<ISimulateBehaviour> simulators,
        BaseScenario scenario) =>
        services =>
        {
            foreach (var simulator in simulators)
            {
                simulator.RegisterTo(services, scenario);
            }
        };
    
    protected ValueFromScenarioDefaulter<T> Defaulting<T>(T? value, [CallerArgumentExpression(nameof(value))] string? message = null)
    {
        return new ValueFromScenarioDefaulter<T>(value, _scenario, message);
    }
}