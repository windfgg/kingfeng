using KingFeng.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KingFeng
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

            services.AddControllers();


            //配置Controllers全部由AutoFac创建
            //services.AddControllersWithViews().AddControllersAsServices()

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //AddSingleton的生命周期：项目启动 - 项目关闭   相当于静态类 只会有一个
            services.AddScoped<IConfigServices, ConfigServices>();

            services.AddSwaggerGen(c =>
            { 
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "KingFeng", Version = "v1" });
                /*
                c.AddSecurityDefinition("Jwt身份认证", new OpenApiSecurityScheme()
                {
                    Scheme = "bearer",
                    //Description = "Authorization:Bearer {your JWT token}<br/><b>授权地址:/Base_Manage/Home/SubmitLogin</b>",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http
                });
                */
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP ClientRequest pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
             
            }

            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "KingFeng v1");
                //c.RoutePrefix = string.Empty;
            });

            // 设置允许所有来源跨域
            app.UseCors(options =>
            {
                options.SetIsOriginAllowed(origin => true);
                options.AllowAnyHeader();
                options.AllowAnyMethod();
                options.AllowCredentials();
            });

            //启动Session
            //app.UseSession();

            //app.UseHttpsRedirection(); //强制使用Https协议

            //静态文件
            app.UseStaticFiles();
            /*
            app.UseStaticFiles(new StaticFileOptions
              {
                  ServeUnknownFileTypes = true,
                  DefaultContentType = "application/octet-stream"
              })
            */
 
            app.UseRouting();

            app.UseAuthorization();

            app.UseFileServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
