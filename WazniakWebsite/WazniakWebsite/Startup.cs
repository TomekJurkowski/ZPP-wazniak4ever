using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WazniakWebsite.Startup))]
namespace WazniakWebsite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
