namespace sas.Scenario.Setup;

public delegate T Setup<T>(T current);

public static class SetupExtensions
{
    public static T Apply<T>(this Setup<T>? setup, T value)
    {
        if (setup is null)
        {
            return value;
        }

        return setup(value);
    }
}