using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KingFeng.Models
{
    public class ConfigModel
    {
        public List<ConfigItemModel> Servers {  get; set; }

        public string SecretKey { get; set; }

        public string WsKeyTaskFullName { get; set; }

        public string Notice { get; set; }

        public string Course { get; set; }

        public string PushImageUrl { get; set; }
    }

    public class ConfigItemModel
    {
        public string QL_Name { get; set; }

        public string QL_URL { get; set; }

        public string QL_Client_ID { get; set; }

        public string QL_Client_Secret { get; set; }

        public int MaxCount { get; set; }
    }

    public class ConfigItemModel1
    {
        public string QL_Name { get; set; }

        public string QL_URL { get; set; }

        public int MaxCount { get; set; }

        public int? CurrentCount { get; set; }
    }
}
