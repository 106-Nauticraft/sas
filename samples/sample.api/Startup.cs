using sample.api.Domain;
using sample.api.HttpClients;
using sample.api.Infra;
using sample.api.Infra.HttpClients;

namespace sample.api;

public class Startup
{
    public void Configure(IApplicationBuilder app,
        IWebHostEnvironment env)
    {
        app.UseHttpsRedirection();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen();

        
        services.AddHttpClient<OpenMeteoHttpClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.open-meteo.com/v1/");
        });


        services.AddTransient<WeatherForecastsProvider>(x => x.GetRequiredService<OpenMeteoHttpClient>());
        
        services.AddTransient<CityLocator, CityLocatorImplementation>();
        
        services.AddTransient<WeatherForecasts>();

    }
}