namespace sample.api.Domain;

public interface CityLocator
{
    (Coordinates coordinates, TimeZone timeZone) Locate(string citySearchInput);
}