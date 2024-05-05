using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedditConsole.ServiceRepository;
using RedditConsole.Services;
using Serilog;

namespace RedditConsole
{
    partial class Program
    {

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();
        }

        static async Task Main(string[] args)
        {

            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context,servcies)=>
                {
                    servcies.AddScoped<ISubRedditServiceRepo,SubRedditServiceRepo>();
                    servcies.AddScoped<ISubredditService, SubredditService>();

                })
                .UseSerilog()
                .Build();

            var svc = ActivatorUtilities.CreateInstance<SubredditService>(host.Services);
            Log.Logger.Information("Application started fetching data from Reddit");
            await svc.GetSubreddit();
        }
    }
}
