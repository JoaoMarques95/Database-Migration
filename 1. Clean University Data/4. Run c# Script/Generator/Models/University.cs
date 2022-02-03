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
        public string ChangeAPINameFlag { get; set; }
        public string NotListedInstitutionFlag { get; set; }
        public string APIListName { get; set; }
        internal static University ParseFromCsv(string line)
        {
            var columns = line.Split(Properties.csvFileDelemitor);

            return new University
            {
                Id = columns[0],
                Name = columns[1],
                Deleted = columns[12],
                UpdateUniversity = columns[13],
                UpdateFaculty = columns[14],
                UpdateFlag = columns[15],
                DeleteUniversity = columns[16],
                NotListedInstitutionFlag = columns[17],
                DeleteFaculty = columns[18],
                ChangeAPINameFlag = columns[19],
                APIListName = columns[21]
            };
        }
    }
}