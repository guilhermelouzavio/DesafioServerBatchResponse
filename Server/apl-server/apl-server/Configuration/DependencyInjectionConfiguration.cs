using apl_server.Client;
using apl_server.Service;

namespace apl_server.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDepedencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IMessageService,MessageService>();
        }
    }
}
