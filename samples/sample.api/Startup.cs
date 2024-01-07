using sample.api.Domain;
using sample.api.HttpClients;

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
    }
}