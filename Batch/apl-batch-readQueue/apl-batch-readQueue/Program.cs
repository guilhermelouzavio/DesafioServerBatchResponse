using apl_batch_readQueue.Configuration;
using apl_batch_readQueue.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Channels;

class Program {
    public static IConfiguration _configuration { get; set; }

    public static async Task Main(string[] args)
    {
        _configuration = new ConfigurationBuilder().Build();

        var hostBuilder = new HostBuilder().ConfigureServices(ConfigureServices);

        await hostBuilder.RunConsoleAsync();
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.CreateHostBuilder(_configuration);

        services.AddSingleton(_configuration);

        services.AddScoped<IMessageService, MessageService>();
    }
       

}




