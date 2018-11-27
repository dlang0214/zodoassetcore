using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Senparc.CO2NET;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.RegisterServices;
using Senparc.Weixin.Work;
using Senparc.Weixin.Work.Containers;
using System;
using System.IO;

namespace Zodo.Assets.Website
{
    public class Startup
    {
        public static ILoggerRepository LogResposition;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            LogResposition = LogManager.CreateRepository("NETCoreRepository");
            XmlConfigurator.Configure(LogResposition, new FileInfo("log4net.config"));
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMemoryCache(); //使用本地缓存必须添加
            services.AddDistributedRedisCache((options) =>
            {
                options.Configuration = "127.0.0.1:6379";
            });

            services.AddSession();

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opts =>
                {
                    opts.AccessDeniedPath = "/Error/Deny";
                    opts.LoginPath = "/Login";
                    opts.Cookie.Name = "aspnetcore_zodoassets";
                    opts.ExpireTimeSpan = TimeSpan.FromDays(2);
                    opts.SlidingExpiration = true;
                });

            services.AddSenparcGlobalServices(Configuration)//Senparc.CO2NET 全局注册
                    .AddSenparcWeixinServices(Configuration);//Senparc.Weixin 注册

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IOptions<SenparcSetting> senparcSetting, IOptions<SenparcWeixinSetting> senparcWeixinSetting)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            IRegisterService register = RegisterService
                .Start(env, senparcSetting.Value)
                .UseSenparcGlobal(false); // 启动 CO2NET 全局注册，必须！

            register.UseSenparcWeixin(senparcWeixinSetting.Value, null)
                .RegisterWorkAccount(senparcWeixinSetting.Value); //微信全局注册，必须！
        }
    }
}
