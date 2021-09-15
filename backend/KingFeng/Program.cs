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
using Serilog;
using Serilog.Events;

namespace KingFeng
{
    public class Program
    {
        public static string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"config", "config.yaml");
        public static string InitConfig()
        {
            var servers = new List<ConfigItemModel>();
            servers.Add(new ConfigItemModel()
            {
                QL_URL = "http://localhost:5700/",
                QL_Client_ID = "你的Client_ID",
                QL_Client_Secret = "你的Client_Secre",
                MaxCount = 100,
                QL_Name = "默认节点"
            });
            var config = new KingFeng.Models.ConfigModel()
            {
                SecretKey = Guid.NewGuid().ToString("N").ToUpper(),
                Servers = servers,
            }.Toyaml();

            return config;
        }
        public static int Main(string[] args)
        {
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config")))
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config"));
            }
            if (!File.Exists(ConfigPath))
            {
                var config = InitConfig();
                File.WriteAllText(Program.ConfigPath, config);
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
