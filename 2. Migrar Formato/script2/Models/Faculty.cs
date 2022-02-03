using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator.Models
{
    public class Faculty
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UniversityId { get; set; }
        public string Deleted { get; set; }
        public string OriginalUniversityId { get; set; }
    }
}