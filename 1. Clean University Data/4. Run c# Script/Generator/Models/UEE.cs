using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator.Models
{
    public class UEE
    {
        public string Id { get; set; }
        public string ProfileId { get; set; }
        public string UniversityId { get; set; }
        public string FacultyId { get; set; }
        public string NotListedInstitution { get; set; }
        public string Course { get; set; }
        public string DegreeLevelId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string CreationDate { get; set; }
        public string UpdateDate { get; set; }
        public string Deleted { get; set; }
        internal static UEE ParseFromCsv(string line)
        {
            var columns = line.Split(Properties.csvFileDelemitor);

            return new UEE
            {
                Id = columns[0],
                ProfileId = columns[1],
                UniversityId = columns[2],
                FacultyId = columns[3],
                NotListedInstitution = columns[4],
                Course = columns[5],
                DegreeLevelId = columns[6],
                StartDate = columns[7],
                EndDate = columns[8],
                CreationDate = columns[9],
                UpdateDate = columns[10],
                Deleted = columns[11]
            };
        }
    }
}
