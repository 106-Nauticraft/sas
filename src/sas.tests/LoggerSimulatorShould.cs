using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NFluent;
using sas.Scenario;
using sas.Simulators.Logger;

namespace sas.tests;

public class LoggerSimulatorShould
{
    [Fact]
    public void Record_the_logs_written_through_the_registered_logger()
    {
        var simulator = new LoggerSimulator();
        var services = new ServiceCollection();

        simulator.RegisterTo(services, new BaseScenario());

        services.BuildServiceProvider().GetRequiredService<ILogger>().LogWarning("first scenario");

        Check.That(simulator.WasCalledOnlyWith((LogLevel.Warning, "first scenario"))).IsTrue();
    }

    [Fact]
    public void Forget_the_logs_recorded_before_it_was_bound_again()
    {
        var simulator = new LoggerSimulator();
        var services = new ServiceCollection();

        simulator.RegisterTo(services, new BaseScenario());
        var logger = services.BuildServiceProvider().GetRequiredService<ILogger>();
        logger.LogWarning("first scenario");

        simulator.Bind(new BaseScenario());
        logger.LogWarning("second scenario");

        Check.That(simulator.WasCalledOnlyWith((LogLevel.Warning, "second scenario"))).IsTrue();
        Check.That(simulator.WasCalledWith((LogLevel.Warning, "first scenario"))).IsFalse();
    }

    [Fact]
    public void Record_the_logs_written_through_the_registered_typed_logger()
    {
        var simulator = new LoggerSimulator<LoggerSimulatorShould>();
        var services = new ServiceCollection();

        simulator.RegisterTo(services, new BaseScenario());

        services.BuildServiceProvider()
            .GetRequiredService<ILogger<LoggerSimulatorShould>>()
            .LogWarning("first scenario");

        Check.That(simulator.WasCalledOnlyWith((LogLevel.Warning, "first scenario"))).IsTrue();
    }

    [Fact]
    public void Forget_the_logs_recorded_by_the_typed_logger_before_it_was_bound_again()
    {
        var simulator = new LoggerSimulator<LoggerSimulatorShould>();
        var services = new ServiceCollection();

        simulator.RegisterTo(services, new BaseScenario());
        var logger = services.BuildServiceProvider().GetRequiredService<ILogger<LoggerSimulatorShould>>();
        logger.LogWarning("first scenario");

        simulator.Bind(new BaseScenario());
        logger.LogWarning("second scenario");

        Check.That(simulator.WasCalledOnlyWith((LogLevel.Warning, "second scenario"))).IsTrue();
        Check.That(simulator.WasCalledWith((LogLevel.Warning, "first scenario"))).IsFalse();
    }
}