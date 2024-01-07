

using sample.api;

public class Program
{
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webHost => webHost.UseStartup<Startup>())
            .Build();

        host.Run();
    }
}