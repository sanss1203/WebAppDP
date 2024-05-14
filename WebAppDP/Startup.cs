using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using System;
using Microsoft.AspNet.SignalR;


[assembly: OwinStartupAttribute(typeof(WebAppDP.Startup))]
namespace WebAppDP
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
