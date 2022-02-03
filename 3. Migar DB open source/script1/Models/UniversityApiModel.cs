using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator.Models
{
    public class UniversityApiModel
    {
        public string name { get; set; }
        public string alpha_two_code { get; set; }
        public string country { get; set; }
        public List<string> domains { get; set; }
        public List<string> web_pages { get; set; }

    }
}