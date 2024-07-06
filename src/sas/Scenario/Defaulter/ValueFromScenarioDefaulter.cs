namespace sas.Scenario.Defaulter;

public class ValueFromScenarioDefaulter<T>(T? value, BaseScenario scenario, string? valueName)
{
    public T From<TScenario>(Func<TScenario, T?> defaultWith)
        where TScenario:BaseScenario
    {
        if (value is not null)
        {
            return value;
        }

        if (scenario is not TScenario castedScenario)
        {
            throw new InvalidOperationException($"Unexpected scenario type {scenario.GetType().Name}");
        }
        var defaultedValue = defaultWith(castedScenario);
        if (defaultedValue is not null)
        {
            return defaultedValue;
        }

        throw new InvalidOperationException( $"No values provided for {valueName}");
    }
}