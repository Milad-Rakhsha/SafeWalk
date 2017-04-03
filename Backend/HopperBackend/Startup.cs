using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(HopperBackend.Startup))]

namespace HopperBackend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}