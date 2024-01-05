using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using sas.Scenario;
using sas.Simulators;

namespace sas.simulators.nunit;

/// <summary>
/// Redirects logs to test output by using the TestConsoleLogger capabilities. Can facilitate investigations when an exception is raised in the tested code.
/// </summary>
public class TestConsoleLoggerSimulator : ISimulateBehaviour
{
    public void RegisterTo(IServiceCollection services, BaseScenario scenario)
    {
        var loggerInstance = new TestConsoleLogger(maxLevel: LogLevel.Information);

        services.AddSingleton<ILogger>(loggerInstance);
        services.AddSingleton(_ =>
        {
            var loggerFactory = Substitute.For<ILoggerFactory>();
            loggerFactory.CreateLogger(Arg.Any<string>())
                .Returns(loggerInstance);
            return loggerFactory;
        });
    }
}