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

                //_logger.LogInformation(Model.ToJson());
            }
            catch(Exception ex)
            {
                _logger.LogError("请检查配置文件格式是否正确");
                _logger.LogError(ex.Message);
            }
        }

        public void UpdateConfig(ConfigModel config)
        {
            File.WriteAllText(Program.ConfigPath, config.Toyaml());
        }
    }
}
