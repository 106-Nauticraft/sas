
using Microsoft.Extensions.Configuration;

namespace sas.Configurations;

public class InMemoryConfiguration : IEnrichConfiguration
{
    public static IEnrichConfiguration Single(string key, string? value) => new InMemoryConfiguration(
        new Dictionary<string, string?>
        {
            [key] = value
        });
    
    private readonly Dictionary<string, string?> _configuration;

    public InMemoryConfiguration(Dictionary<string, string?> configuration)
    {
        _configuration = configuration;
    }
    
    public void Enrich(ConfigurationBuilder builder)
    {
        builder.AddInMemoryCollection(_configuration);
    }
}