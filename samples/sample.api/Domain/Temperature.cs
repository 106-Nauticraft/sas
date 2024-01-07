namespace sample.api.Domain;

public record Temperature(decimal Value)
{
    public Temperature ToFahrenheit()
    {
        return new Temperature(32 + Value / 0.5556m);
    }
}