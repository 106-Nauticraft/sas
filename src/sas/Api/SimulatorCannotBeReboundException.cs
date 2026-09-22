using sas.Simulators;

namespace sas.Api;

/// <summary>
/// Raised when a host is asked to bind another scenario while one of its simulators does not implement <see cref="IBindScenario"/>.
/// </summary>
public class SimulatorCannotBeReboundException(IEnumerable<ISimulateBehaviour> simulators)
    : InvalidOperationException(BuildMessage(simulators))
{
    private static string BuildMessage(IEnumerable<ISimulateBehaviour> simulators)
    {
        var simulatorNames = string.Join(", ", simulators.Select(simulator => simulator.GetType().Name));

        return $"{simulatorNames} cannot be bound to another scenario because they do not implement {nameof(IBindScenario)}. " +
               $"Implement {nameof(IBindScenario)} on it - with an empty {nameof(IBindScenario.Bind)} if it reads nothing from the scenario or build one host per scenario.";
    }
}
