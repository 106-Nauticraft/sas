namespace sas.Scenario.Defaulter;

public class ValueFromScenarioDefaulter<T>
{
    private readonly T? _value;
    private readonly BaseScenario _scenario;
    private readonly string? _valueName;

    public ValueFromScenarioDefaulter(T? value, BaseScenario scenario, string? valueName)
    {
        _value = value;
        _scenario = scenario;
        _valueName = valueName;
    }

    public T From<TScenario>(Func<TScenario, T?> defaultWith)
        where TScenario:BaseScenario
    {
        if (_value is not null)
        {
            return _value;
        }

        if (_scenario is not TScenario castedScenario)
        {
            throw new InvalidOperationException($"Unexpected scenario type {_scenario.GetType().Name}");
        }
        var defaultedValue = defaultWith(castedScenario);
        if (defaultedValue is not null)
        {
            return defaultedValue;
        }

        throw new InvalidOperationException( $"No values provided for {_valueName}");
    }
}