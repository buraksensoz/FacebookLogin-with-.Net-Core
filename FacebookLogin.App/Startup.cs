using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookLogin.App
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

            #region Facebook Cookie Configuration

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = new PathString("/Home/index/");
                })
                .AddFacebook(FacebookDefaults.AuthenticationScheme, options =>
                {
                    options.AppId = Configuration.GetSection("Auth_Facebook").GetSection("apikey").Value;
                    options.AppSecret = Configuration.GetSection("Auth_Facebook").GetSection("secret").Value;
                    options.Scope.Add("email");
                    options.Fields.Add("birthday");
                    options.Fields.Add("picture");
                    options.Fields.Add("name");
                    options.Fields.Add("email");
                    options.ClaimActions.MapCustomJson("pictureUrl",
                    json => json.GetProperty("picture").GetProperty("data").GetProperty("url").GetString());

                });


            #endregion

            services.AddHttpContextAccessor(); //You Need This For Get Facebook Details From Cookie 
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
            
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.UseAuthentication(); //You Need For the Auth Facebook

            //For this Policy access from Facebook.
            app.UseCookiePolicy(new CookiePolicyOptions() 
            {
                MinimumSameSitePolicy = SameSiteMode.Lax
            });



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
