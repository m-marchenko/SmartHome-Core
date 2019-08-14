using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHome.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using SmartHome.Models.Security;
using SmartHome.Model;

namespace SmartHome
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

            services.AddDefaultIdentity<SmartHomeUser>()
                    .AddUserStore<SmartHomeUserStore>()
                    .AddSignInManager<SignInManager<SmartHomeUser>>()
                    .AddDefaultUI();
            
            services.Configure<IdentityOptions>(options =>
            {
                // Set your identity Settings here (password length, etc.)                
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.Configure<List<SmartHomeUser>>(Configuration.GetSection("Users"));

            //var profiles = Configuration.GetSection("Profiles").Get<List<UserProfile>>();
            
            services.Configure<List<UserProfile>>(Configuration.GetSection("Profiles"));
            //services.PostConfigure<List<UserProfile>>(profiles => profiles.ForEach(profile => profile.Content.ForEach(c => c.UpdateParents())));

            
            //services.ConfigureApplicationCookie(options =>
            //{
            //    options.LoginPath = $"/account/login";
            //    options.LogoutPath = $"/account/logout";
            //    options.AccessDeniedPath = $"/account/access-denied";
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }


    
}
