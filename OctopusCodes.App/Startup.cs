using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OctopusCodes.App.Startup))]
namespace OctopusCodes.App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}