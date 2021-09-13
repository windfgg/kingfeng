using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KingFeng.Models
{
    public class EnvModel
    {
        [Required]
        public string name { get; set; }

        [Required]
        public string value { get; set; }

        public string remarks { get; set; }
    }

    public class EnvModel2
    {
        //public long created { get; set; }

        [Required]
        public string name { get; set; }

        //public int position { get; set; }

        public string remarks { get; set; }

        public bool status { get; set; }

        public string timestamp { get; set; }

        [Required]
        public string value { get; set; }


        public string _id { get; set; }
    }

    public class EnvUpdateModel
    {
        //public long created { get; set; }

        [Required]
        public string name { get; set; }

        //public int position { get; set; }

        public string remarks { get; set; }

        [Required]
        public string value { get; set; }


        public string _id { get; set; }
    }

    public class EnvsModel
    {
        public List<EnvModel> envs { get; set; }
    }
}
