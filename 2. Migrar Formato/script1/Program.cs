using Generator;
using Generator.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace script1
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Writing...");

            List<string> sqlComands = new List<string>();
            List<University> universities = JsonConvert.DeserializeObject<List<University>>(Helper.getJsonString("Data/universities.json"));

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
            


            // Prepare data to create SQL commands (First universityId ; List <Faculty Name; Original faculty Name(separated by comma if multiple)>)
            List<DataToInsertUniversityFaculties> dataToInsertFaculties = new List<DataToInsertUniversityFaculties>();
            foreach (var item in groupUniversities)
            {
                var universitiesWithSameName = item.Value;
                var dataToInsertFaculty = new DataToInsertUniversityFaculties()
                {
                    firstUniversityId = universitiesWithSameName.First().Id,
                    facultiesData = new List<FacultyData>()
                };
                
                //Get all faculty data.
                var facultiesData = new List<FacultyData>();
                foreach (var universityWithSameName in universitiesWithSameName)
                {
                    var FacultyName = universityWithSameName.Name.Substring(universityWithSameName.Name.IndexOf(Helper.separator) + 1);
                    var OriginalUniversityId = universityWithSameName.Id;

                    if(FacultyName != universityWithSameName.Name)
                    {
                        facultiesData.Add(new FacultyData()
                        {
                            FacultyName = FacultyName,
                            FacultyFlag = universityWithSameName.Deleted,
                            OriginalUniversityId = OriginalUniversityId
                        });
                    }
                }

                
                //Get only distint faculties with orginalUniversityId Merged.                  
                var facultiesDataFilteredList = new List<FacultyData>();
                for (int i = facultiesData.Count - 1; i >= 0; i--)
                {
                    if(facultiesData.Count == 0)
                    {
                        break;
                    }

                    if (i >= facultiesData.Count)
                    {
                        i = facultiesData.Count - 1;
                    }

                    // some method that removes multiple elements here
                    var facultyActualObject = new FacultyData();
                    facultyActualObject = facultiesData[i];
                    var otherSameFaculties = facultiesData.Where(f => f.FacultyName == facultiesData[i].FacultyName).ToList();

                    if (otherSameFaculties.Count > 1) //If has more than 1 faculty with the same name.
                    {
                        otherSameFaculties.Remove(facultyActualObject);
                        facultiesData.Remove(facultyActualObject); //Irelevant

                        //Remove all, append to new object the new one.
                        foreach (var otherSameFaculty in otherSameFaculties)
                        {
                            facultyActualObject.OriginalUniversityId += "|" + otherSameFaculty.OriginalUniversityId;
                            facultiesData.Remove(otherSameFaculty);
                        }
                        facultiesDataFilteredList.Add(facultyActualObject);
                    }
                    else
                    {
                        //unique faculty
                        facultiesDataFilteredList.Add(facultyActualObject);
                    }
                }

                //Append data to dataToInsertFaculties
                foreach (var facultyData in facultiesDataFilteredList)
                {
                    dataToInsertFaculty.facultiesData.Add(facultyData);
                }
                dataToInsertFaculties.Add(dataToInsertFaculty);
            }

            //Create SQL script, for each university create an SQL that
            foreach (var dataToInsertFaculty in dataToInsertFaculties)
            {
                foreach (var facultyData in dataToInsertFaculty.facultiesData)
                {
                    //Insert faculty
                    insertFacultyDB(facultyData.FacultyName.Trim(),facultyData.FacultyFlag,facultyData.OriginalUniversityId, dataToInsertFaculty.firstUniversityId, sqlComands);
                }
            }

            Helper.insertSQLLine(sqlComands);
            Console.WriteLine("Finish");
        }

        private static void insertFacultyDB(string facultyName, string flag,  string originalUniversityId, string universityId, List<string> sqlComands)
        {
            sqlComands.Add($"INSERT INTO Faculty ( Name, UniversityId,Deleted, OriginalUniversityId) VALUES ('{facultyName}', '{universityId}', '{flag}', '{originalUniversityId}');");   
        }
    }
}
