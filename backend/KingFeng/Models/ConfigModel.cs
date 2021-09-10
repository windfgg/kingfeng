using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KingFeng.Models
{
    public class ConfigModel
    {
        public string QL_URL { get; set; }

        public string QL_Client_ID { get; set; }

        public string QL_Client_Secret { get; set; }

        public string SecretKey { get; set; }

        public string WsKeyTaskFullName { get; set; }

        public string Course { get; set; }

        public string PushImageUrl { get; set; }
    }
}
