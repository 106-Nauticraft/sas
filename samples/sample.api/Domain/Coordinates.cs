namespace sample.api.Domain;

public record Coordinates(double Latitude, double Longitude)
{
    public static readonly Coordinates Paris = new(48.8534, 2.3488);
    public static readonly Coordinates London = new(51.5074, 0.1278);
    public static readonly Coordinates NewYork = new(40.7128, -74.0060);
}