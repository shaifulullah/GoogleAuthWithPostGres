using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using GoogleAuthWithPostGres.Data;
using GoogleAuthWithPostGres.Properties;
using GoogleAuthWithPostGres.Repository;
using GoogleAuthWithPostGres.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GoogleAuthWithPostGres
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<GoogleAuthWithPostGresContext>(opt => opt.UseNpgsql(Configuration.GetConnectionString("GoogleAuthWithPostGres")));

            //Session
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                options.Cookie.HttpOnly = true;
                //make the cookie essential
                options.Cookie.IsEssential = true;
            });
            //services.AddAuthentication()
            //    .AddGoogle(opts =>
            //    {
            //        opts.ClientId = "1046314261939-76g2b1cigtc3cb4bau2tf197cvmlocju.apps.googleusercontent.com";
            //        opts.ClientSecret = "WzyJCTHRTQuctsfw7swNlbqv";
            //        opts.SignInScheme = IdentityConstants.ExternalScheme;
            //    });

            services.AddTransient<IUser, UserRepository>();

            services.AddControllersWithViews();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 3;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;

            });
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            });
            var cookieExpireTimeSpan = TimeSpan.FromHours(12);
            //services.AddNodeServices();
            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
        .AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = Resources.googleClientId;
            googleOptions.ClientSecret = Resources.googleClientSecret;
            googleOptions.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
            googleOptions.ClaimActions.Clear();
            googleOptions.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
            googleOptions.CorrelationCookie.Expiration = cookieExpireTimeSpan;
        })
        .AddCookie();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action="SignIn"});
            });
            app.UseCookiePolicy();
        }
    }
}
