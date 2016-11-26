using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WeFix.WebUI.Startup))]
namespace WeFix.WebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
