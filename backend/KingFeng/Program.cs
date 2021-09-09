using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KingFeng.Models;
using YamlDotNet.Serialization;
using Autofac.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace KingFeng
{
    public class Program
    {
        public static string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.yaml");
        public static int Main(string[] args)
        {
            if (!File.Exists(ConfigPath))
            {
                var config = new KingFeng.Models.ConfigModel()
                {
                    SecretKey = Guid.NewGuid().ToString("N").ToUpper(),
                    Course = "www.baidu.com"
                }.Toyaml();
                File.AppendAllText(ConfigPath, config);
            }

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }


            //CreateHostBuilder(args).Build().Run(); 
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //.UseSerilog((context, services, configuration) => configuration
                //    .ReadFrom.Configuration(context.Configuration)
                //    .ReadFrom.Services(services)
                //    .Enrich.FromLogContext()
                //    .WriteTo.Console())
                .UseSerilog() // <-- Add this line
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    //webBuilder.UseUrls($"http://[::]:5000");
                });
        //.UseServiceProviderFactory(new AutofacServiceProviderFactory()); //替换依赖注入管理器为AutoFac
    }
}
