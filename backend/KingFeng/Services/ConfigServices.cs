using KingFeng.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KingFeng.Services
{
    public interface IConfigServices
    {
        public ConfigModel config { get; set; }
        void ReadConfig();
        void UpdateConfig(ConfigModel config);
    }

    public class ConfigServices: IConfigServices
    {
        public readonly ILogger<ConfigServices> _logger;

        public ConfigServices(ILogger<ConfigServices> logger)
        {
            _logger = logger;

            ReadConfig();
        }

        public ConfigModel config { get; set; }

        public void ReadConfig()
        {
            try
            {
                config = File.ReadAllText(Program.ConfigPath).YamlTo<ConfigModel>();
                if (string.IsNullOrWhiteSpace(config.SecretKey))
                {
                    config.SecretKey = Guid.NewGuid().ToString("N").ToUpper();
                    UpdateConfig(config);

                    config = File.ReadAllText(Program.ConfigPath).YamlTo<ConfigModel>();
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("配置文件格式错误,已经为您初始化配置文件 请重新编辑配置文件");
                var config = InitConfig();

                File.WriteAllText(Program.ConfigPath, config);
                this.config = File.ReadAllText(Program.ConfigPath).YamlTo<ConfigModel>();
                _logger.LogError(ex.Message);
            }
        }

        public string InitConfig()
        {
            var servers = new List<ConfigItemModel>();
            servers.Add(new ConfigItemModel()
            {
                QL_URL = "http://localhost:5700/",
                QL_Client_ID = "123",
                QL_Client_Secret = "123",
                MaxCount = 100,
                QL_Name = "广州节点"
            });
            var config = new KingFeng.Models.ConfigModel()
            {
                PushImageUrl = "https://i0.hdslb.com/bfs/album/2c6255c33adf1952c068cbb081d006997a6f506e.png",
                Notice = "你好,这里可以自定义公告",
                SecretKey = Guid.NewGuid().ToString("N").ToUpper(),
                Servers = servers,
                UserName = "KingFeng作者"
            }.Toyaml();

            return config;
        }

        public void UpdateConfig(ConfigModel config)
        {
            File.WriteAllText(Program.ConfigPath, config.Toyaml());        
        }
    }
}
