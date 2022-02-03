using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator.Models
{
    public class University
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Deleted { get; set; }
        public string UpdateUniversity { get; set; }
        public string UpdateFaculty { get; set; }
        public string UpdateFlag { get; set; }
        public string DeleteUniversity { get; set; }
        public string DeleteFaculty { get; set; }
    }
}