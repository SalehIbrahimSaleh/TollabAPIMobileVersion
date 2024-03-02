using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using TollabAPI.Providers;
using TollabAPI.Models;
using Hangfire;
using Hangfire.Dashboard;

namespace TollabAPI
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {

            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            GlobalConfiguration.Configuration
    .UseSqlServerStorage("Data Source=tollabserver.database.windows.net;Initial Catalog=TollabDB;User Id=sadmin;Password=TollabAdminPa$$word;");
            //hangfire
            app.UseHangfireServer();

            //var options = new DashboardOptions
            //{
            //    Authorization = new List<IDashboardAuthorizationFilter>
            //         {
            //            new AuthorizationFilter { Users = "admin, superuser", Roles = "advanced" },
            //            new ClaimsBasedAuthorizationFilter("name", "value")
            //         }
            //};
            app.UseHangfireDashboard("/hangfire");
            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(365),
                // In production mode set AllowInsecureHttp = false
                AllowInsecureHttp = true
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            //app.UseFacebookAuthentication(
            //    appId: "",
            //    appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
            //  CreateUser();
            CreateUserg();
        }
        public async void CreateUser()
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbContext));

            ApplicationUser applicationUser = new ApplicationUser
            {
                Email = "admin@xapps.com",
                PhoneNumber = "+20100000000",
                UserName = "admin@xapps.com"
            };
            try
            {
                await _userManager.CreateAsync(applicationUser, "123456");
                await _userManager.AddToRoleAsync(applicationUser.Id, "Admin");
            }
            catch (Exception ex)
            {

            }
        }

        public async void CreateUserg()
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            var _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbContext));

            ApplicationUser applicationUser = new ApplicationUser
            {
                Email = "Guest@Tollab.com",
                PhoneNumber = "010000000000",
                UserName = "Guest@Tollab.com"
            };
            try
            {
                await _userManager.CreateAsync(applicationUser, "Guest123456");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
