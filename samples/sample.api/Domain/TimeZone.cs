namespace sample.api.Domain;

public class TimeZone
{
    private TimeZone(string name)
    {
        Name = name;
    }
    
    public static readonly TimeZone Paris = new("Europe/Paris");
    public static readonly TimeZone London = new("Europe/London");
    public static readonly TimeZone NewYork = new("America/New_York");
    
    
    public string Name { get; set; }
    
}