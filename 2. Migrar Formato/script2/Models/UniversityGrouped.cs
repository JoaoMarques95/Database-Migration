using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator.Models
{
    public class UniversityGrouped
    {
        public string FirstId { get; set; }
        public string Name { get; set; }
        public List<University> Universities { get; set; }
    }
}