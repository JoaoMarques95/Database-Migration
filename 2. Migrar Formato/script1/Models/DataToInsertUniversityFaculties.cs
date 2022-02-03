using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator.Models
{


    public class DataToInsertUniversityFaculties
    {
        public string firstUniversityId { get; set; }
        public List<FacultyData> facultiesData { get; set; }
       
    }
}