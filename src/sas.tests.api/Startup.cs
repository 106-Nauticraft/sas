using Microsoft.Extensions.Options;

namespace sas.tests.api;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions();
        services.AddRouting();

        services.AddSingleton<IGreet, Greeter>();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/greet/{name}", (IGreet greeter, string name) => greeter.Greet(name));
            endpoints.MapGet("/prefix", (IOptions<GreetingOptions> options) => options.Value.Prefix);
        });
    }
}

public interface IGreet
{
    string Greet(string name);
}

public class Greeter : IGreet
{
    public const string RealImplementation = "the real greeter";

    public string Greet(string name) => $"{RealImplementation} greets {name}";
}

public class GreetingOptions
{
    public const string NotConfigured = "not configured";

    public string Prefix { get; set; } = NotConfigured;
}
