using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using sas.Scenario;

namespace sas.Simulators.Logger;

public class LoggerSimulator : ISimulateBehaviour
{
    public void RegisterTo(IServiceCollection services, BaseScenario _)
    {
        services.AddSingleton(Substitute.For<ILogger>());
        services.AddSingleton(_ =>
        {
            var factory = Substitute.For<ILoggerFactory>();
            factory.CreateLogger(Arg.Any<string>())
                .Returns(Substitute.For<ILogger>());
            return factory;
        });
    }
}

public class LoggerSimulator<TTarget> : ISimulateBehaviour, ISpyLogs
{
    private readonly LoggerSpy<TTarget> _spy = new();

    public void RegisterTo(IServiceCollection services, BaseScenario _)
    {
        services.AddSingleton<ILogger<TTarget>>(_spy);
    }

    public bool WasNeverCalled(LogLevel? specificLevelToCheck = null)
    {
        return _spy.WasNeverCalled(specificLevelToCheck);
    }

    public bool WasCalledWith(params (LogLevel level, string message)[] expected)
    {
        return _spy.WasCalledWith(expected);
    }

    public bool WasCalledOnlyWith(params (LogLevel level, string message)[] expected)
    {
        return _spy.WasCalledOnlyWith(expected);
    }
}