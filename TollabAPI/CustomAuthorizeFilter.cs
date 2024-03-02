using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(TollabAPI.Startup))]

namespace TollabAPI
{
    public class CustomAuthorizeFilter : IDashboardAuthorizationFilter
    {

        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}
