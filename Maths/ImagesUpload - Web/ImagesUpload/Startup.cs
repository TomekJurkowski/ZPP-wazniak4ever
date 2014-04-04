using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ImagesUpload.Startup))]
namespace ImagesUpload
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
