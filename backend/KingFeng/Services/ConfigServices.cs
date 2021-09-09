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
        string QL_URL { get;}

        string QL_Client_ID { get; }

        string QL_Client_Secret { get;}

        public string SecretKey { get; set; }

        public string WsKeyTaskFullName { get; set; }

        public string Course { get; set; }

        void getConfig();
    }

    public class ConfigServices: IConfigServices
    {
        public readonly ILogger<ConfigServices> _logger;

        public ConfigServices(ILogger<ConfigServices> logger)
        {
            _logger = logger;

            getConfig();
        }

        public string QL_URL { get; set; }

        public string QL_Client_ID { get ; set; }

        public string QL_Client_Secret { get ; set; }

        public string SecretKey { get; set; }

        public string Course { get; set; }

        public string WsKeyTaskFullName { get; set; }

        public void getConfig()
        {
            try
            {
                var Model = File.ReadAllText(Program.ConfigPath).YamlTo<ConfigModel>();

                QL_URL = Model.QL_URL;
                QL_Client_ID = Model.QL_Client_ID;
                QL_Client_Secret = Model.QL_Client_Secret;
                SecretKey=Model.SecretKey;
                WsKeyTaskFullName = Model.WsKeyTaskFullName;
                Course = Model.Course; 

                //_logger.LogInformation(Model.ToJson());
            }
            catch(Exception ex)
            {
                _logger.LogError("请检查配置文件格式是否正确");
                _logger.LogError(ex.Message);
            }
        }
    }
}
