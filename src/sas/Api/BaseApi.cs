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


public abstract class BaseApi<TStartup>
    where TStartup : class
{
    protected HttpClient HttpClient { get; }
    
    private readonly IServiceProvider _services;

    private readonly BaseScenario _scenario;
    private readonly ISimulateBehaviour[] _simulators;

    protected BaseApi(BaseScenario scenario, ISimulateBehaviour[] simulators, IEnrichConfiguration[] configurations)
    {
        var factory = new WebApplicationFactory<TStartup>();

        _scenario = scenario;
        _simulators = simulators;

        factory = factory.WithWebHostBuilder(webHost => webHost
            .ConfigureAppConfiguration(ConfigureAppConfiguration(configurations))
            .ConfigureTestServices(
            ConfigureTestServices(_simulators, scenario)
        ));
        HttpClient = factory.CreateClient();
        _services = factory.Services;
    }
    
    private static Action<IConfigurationBuilder>ConfigureAppConfiguration(IEnrichConfiguration[] additionalConfigurations) =>
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

    private static IConfigurationRoot BuildAdditionalConfiguration(IEnrichConfiguration[] additionalConfigurations)
    {
        var configurationBuilder = new ConfigurationBuilder();

        foreach (var configurationEnricher in additionalConfigurations)
        {
            configurationEnricher.Enrich(configurationBuilder);
        }

        return configurationBuilder.Build();
    }

    public T GetRequiredService<T>() where T : notnull
    {
        try
        {
            return _services.GetRequiredService<T>();
        }
        // For scoped services, an exception is thrown and the IServiceScopeFactory is needed.
        catch (InvalidOperationException)
        {
            using var scope = _services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            return scope.ServiceProvider.GetRequiredService<T>();
        }
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