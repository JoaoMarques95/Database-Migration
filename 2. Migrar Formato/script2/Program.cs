using Generator;
using Generator.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace script2
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> sqlComands = new List<string>();

            List<University> universities = JsonConvert.DeserializeObject<List<University>>(Helper.getJsonString("Data/universities.json"));
            List<Faculty> faculties = JsonConvert.DeserializeObject<List<Faculty>>(Helper.getJsonString("Data/faculties.json"));
            List<UEE> experiences = JsonConvert.DeserializeObject<List<UEE>>(Helper.getJsonString("Data/UEE.json"));

            // Colocar faculty id na user education experince com base no originalUniversityId na tabela das faculdades.
            foreach (var experience in experiences)
            {
                if (experience.UniversityId != null)
                {
                    var faculty = faculties.Where(faculty => faculty.OriginalUniversityId != null ? faculty.OriginalUniversityId.Contains(experience.UniversityId) : false).FirstOrDefault();

                    if (faculty != null)
                    {
                        sqlComands.Add($"UPDATE UserEducationExperience SET FacultyId='{faculty.Id}' WHERE UniversityId='{experience.UniversityId}';");
                    }
                }
            }

            //Colocar universityId na user education experince.
            List<UniversityGrouped> universitiesGrouped = GetUniquefirstUniversities(universities);
            foreach (var experience in experiences)
            {
                foreach (var universityGrouped in universitiesGrouped)
                {
                    var existsUniversity = universityGrouped.Universities.Exists(university => university.Id == experience.UniversityId);

                    if(existsUniversity)
                    {
                        sqlComands.Add($"UPDATE UserEducationExperience SET UniversityId='{universityGrouped.FirstId}' WHERE UniversityId='{experience.UniversityId}';");
                        continue;
                    }
                }
            }

            /*Eliminar universidades repetidas
            OriginalUniversityId - UniversityId == universidades a eliminar
             */

            var UniversityIdList = (from universityGrouped in universitiesGrouped
                                    select universityGrouped.FirstId).ToList();

            var OriginalUniversityIdList = (from university in universities
                                    select university.Id).ToList();

            var universitiesToDelete = OriginalUniversityIdList.Except(UniversityIdList).ToList();

            foreach (var universityToDelete in universitiesToDelete)
            {
                sqlComands.Add($"DELETE FROM University WHERE Id='{universityToDelete}';");
            }

            //Change name of universities (to only university)
            foreach (var UniversityId in UniversityIdList)
            {
                var university = universities.Where(u => u.Id == UniversityId).FirstOrDefault();

                if(university != null)
                {
                    //Get university name
                    var nameBDList = university.Name.Split(Helper.separator);
                    var univeristyName = nameBDList[0].Trim();

                    sqlComands.Add($"UPDATE University SET Name='{univeristyName}' WHERE Id='{university.Id}';");
                }
            }

            Console.WriteLine("Writing Output...");
            Helper.insertSQLLine(sqlComands);
            Console.WriteLine("Finish");
        }

        private static List<UniversityGrouped> GetUniquefirstUniversities(List<University> universities)
        {
            // Get univeristy unique names.
            List<string> universityNames = new List<string>();
            foreach (var university in universities)
            {
                var nameBDList = university.Name.Split(Helper.separator);
                universityNames.Add(nameBDList[0]);
            }
            var uniqueUniversityNames = universityNames.Distinct().ToList();

            // Search for each name and append to a List (UniversityName ; List<University>)
            IDictionary<string, List<University>> groupUniversities = new Dictionary<string, List<University>>();
            foreach (var uniqueUniversityName in uniqueUniversityNames)
            {
                var listOfUniversities = universities.Where(u =>
                {
                    var uName = u.Name.Split(Helper.separator);
                    return uName[0] == uniqueUniversityName;
                }).ToList();
                groupUniversities.Add(uniqueUniversityName, listOfUniversities.ToList());
            }

            var universitiesGrouped = new List<UniversityGrouped>();

            foreach (var item in groupUniversities)
            {
                var universitiesWithSameName = item.Value;
                var universityGrouped = new UniversityGrouped()
                {
                    FirstId = universitiesWithSameName.First().Id,
                    Name = item.Key,
                    Universities = item.Value
                };
                universitiesGrouped.Add(universityGrouped);
            }

            return universitiesGrouped;

        }
    }
}
