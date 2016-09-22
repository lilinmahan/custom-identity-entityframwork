using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebApplication10.Startup))]
namespace WebApplication10
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
