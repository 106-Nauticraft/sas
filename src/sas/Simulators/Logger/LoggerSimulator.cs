using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using sas.Scenario;

namespace sas.Simulators.Logger;

public class LoggerSimulator : ISimulateBehaviour, ISpyLogs
{
    private readonly LoggerSpy _spy = new();

    public void RegisterTo(IServiceCollection services, BaseScenario _)
    {
        services.AddSingleton<ILogger>(_spy);
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