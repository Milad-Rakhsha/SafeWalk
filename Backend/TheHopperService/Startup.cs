using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(TheHopperService.Startup))]

namespace TheHopperService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}