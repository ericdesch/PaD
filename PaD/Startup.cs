using Microsoft.Owin;
using Owin;
using System.Data.Entity;
using Hangfire;

[assembly: OwinStartupAttribute(typeof(PaD.Startup))]
namespace PaD
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            // For hangfire
            GlobalConfiguration.Configuration.UseSqlServerStorage("HangfireConnection");

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}
