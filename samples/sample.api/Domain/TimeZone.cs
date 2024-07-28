namespace sample.api.Domain;

public record TimeZone(string Name)
{
    public static readonly TimeZone Paris = new("Europe/Paris");
    public static readonly TimeZone London = new("Europe/London");
    public static readonly TimeZone NewYork = new("America/New_York");
}