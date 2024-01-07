namespace sample.api.HttpClients.Tools;

public class UrlBuilder
{
    private readonly string _url;
    private readonly Dictionary<string, string?> _namedValues = new();

    private UrlBuilder(string url)
    {
        _url = url;
    }

    public static UrlBuilder From(string url) => new(url);

    public UrlBuilder With(string name, DateTime value, Predicate<DateTime>? condition = null)
    {
        if (condition != null && !condition(value))
        {
            return this;
        }

        _namedValues.Add(name, $"{value:yyyy-MM-dd}");
        return this;
    }

    public UrlBuilder With<T>(string name, T[] value, Predicate<T[]>? condition = null)
    {
        if (condition != null && !condition(value))
        {
            return this;
        }

        _namedValues.Add(name, string.Join(",", value));
        return this;
    }

    public UrlBuilder With<T>(string name, T value, Predicate<T>? condition = null)
    {
        if (condition != null && !condition(value))
        {
            return this;
        }

        _namedValues.Add(name, $"{value}");
        return this;
    }

    public string Build()
    {
        var queryString = QueryString.Create(_namedValues);
        return $"{_url}{queryString}";
    }
}