using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.Owin;
using Owin;
using TollabAPI.Controllers;

[assembly: OwinStartup(typeof(TollabAPI.Startup))]

namespace TollabAPI
{
    public partial class Startup
    {
       // HangFireJobsController hangFireJobsController = new HangFireJobsController();
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            app.UseHangfireDashboard("/dashboard", new DashboardOptions
            {
                Authorization = new[] { new CustomAuthorizeFilter() }
            });

           // hangFireJobsController.SendSmsToStudentForRenewSubscription();
          //  hangFireJobsController.FixInvoices();
        }
    }
}
