namespace sas.tests.api;

public class Program
{
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webHost => webHost.UseStartup<Startup>())
            .Build();

        host.Run();
    }
}
