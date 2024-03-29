using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TrackCaloriesBot;

public class Program
{
    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }
}