using sample.api.Domain;
using TimeZone = sample.api.Domain.TimeZone;

namespace sample.api.Infra;

public class CityLocatorImplementation : CityLocator 
{
    private static readonly Coordinates Paris = new(48.8534, 2.3488);
    private static readonly Coordinates London = new(51.5074, 0.1278);
    private static readonly Coordinates NewYork = new(40.7128, -74.0060);
    
    public (Coordinates, TimeZone) Locate(string citySearchInput)
    {
        return citySearchInput.ToLowerInvariant() switch
        {
            "paris" => (Paris, TimeZone.Paris),
            "london" => (London, TimeZone.London),
            "newyork" => (NewYork, TimeZone.NewYork),
            // todo : implement a database somehow with either EFCore or Dapper or something like that
            _ => throw new ArgumentException($"Unknown town: {citySearchInput}")
        };
    }
}