
using Microsoft.Extensions.Configuration;

namespace sas.Configurations;

public interface IEnrichConfiguration
{
    void Enrich(ConfigurationBuilder builder);
}