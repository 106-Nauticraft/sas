namespace cities.soap;

[DataContract]
public record GpsCoordinates([property:DataMember]decimal Latitude, [property:DataMember]decimal Longitude);

[DataContract]
public record City([property:DataMember]string Name, [property:DataMember]GpsCoordinates Coordinates);
    
[ServiceContract]

public interface IProvideCities
{
    [OperationContract]
    Task<City?> FindCity(string searchString);
}

[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
public class ProvideCities : IProvideCities
{
    public async Task<City?> FindCity(string searchString)
    {
        return searchString switch
        {
            "Berlin" => new City("Berlin", new GpsCoordinates(52.5200m, 13.4050m)),
            "Paris" => new City("Paris", new GpsCoordinates(48.8566m, 2.3522m)),
            "NewYork" => new City("New York", new GpsCoordinates(40.7128m, 74.0060m)),
            _ => null
        };
    }
}