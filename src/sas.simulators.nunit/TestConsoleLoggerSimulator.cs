using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using sas.Scenario;
using sas.Simulators;

namespace sas.simulators.nunit;

/// <summary>
/// Redirects logs to test output by using the TestConsoleLogger capabilities. Can facilitate investigations when an exception is raised in the tested code.
/// </summary>
public class TestConsoleLoggerSimulator : ISimulateBehaviour
{
    private class TestLoggerFactory : ILoggerFactory
    {
        public void Dispose() { }

        public void AddProvider(ILoggerProvider provider) { }

        public ILogger CreateLogger(string categoryName)
        {
            return new TestConsoleLogger();
        }
    }
    
    public void RegisterTo(IServiceCollection services, BaseScenario scenario)
    {
        var loggerInstance = new TestConsoleLogger(maxLevel: LogLevel.Information);

        services.AddSingleton<ILogger>(loggerInstance);
        services.AddSingleton<ILoggerFactory, TestLoggerFactory>();
    }
}