using Generator.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Generator
{
    class Program
    {
        static void Main()
        {
            List<string> sqlComands = new List<string>();
            List<UniversityApiModel> apiUniversities = JsonConvert.DeserializeObject<List<UniversityApiModel>>(Helper.getJsonString(Properties.FilePathAPIListInput));
            List<University> universities = Helper.getUniversities(Properties.csvPathUniversityInput);
            List<UEE> experiences = Helper.getUEE(Properties.csvPathUEEInput);
            int i = 0;

            foreach (var university in universities){
                Console.WriteLine(i++);
                //Simple validation if data is valid
                if (university.Id == "")
                {
                    continue;
                }

                // change name from API LIST if necessary
                changeAPIListName(university, apiUniversities);


                //Build data obect
                var updateData = new Dictionary<string, string>() {
                    {"UpdateUniversity", university.UpdateUniversity == ""? null: university.UpdateUniversity},
                    {"UpdateFaculty", university.UpdateFaculty == ""? null: university.UpdateFaculty},
                    {"UpdateFlag", university.UpdateFlag == ""? null: university.UpdateFlag},
                };

                //Delete University
                if (university.DeleteUniversity == "x" || university.NotListedInstitutionFlag == "x")
                {
                    deleteUniversity(university, experiences, sqlComands, updateData);
                    continue;
                }

                //Delete faculty and if nedded, update university and flag
                if (university.DeleteFaculty == "x")
                {
                    updateDBEntry(updateData, sqlComands, university, true);
                    continue;
                }
                updateDBEntry(updateData, sqlComands, university, false);
            }
            Console.WriteLine("Writing..");
            Helper.insertSQLLine(Properties.FilePathSQLOutput,sqlComands);
            Helper.writeJsonString(Properties.FilePathAPIListOutput, apiUniversities);
            Console.WriteLine("Finish");
        }

        private static void changeAPIListName(University university, List<UniversityApiModel> apiUniversities)
        {
            if(university.ChangeAPINameFlag !="" && university.APIListName != "")
            {
                var nameBDList = university.Name.Split(Properties.UniverisyFacultyseparator);
                var univeristyName = nameBDList[0].Trim(' ');

                var nameToUpdate = university.UpdateUniversity != "" ? university.UpdateUniversity : univeristyName;

                try
                {
                    //Search the apiobject List and change it's name
                    var objectToChange = apiUniversities.Where(ul => ul.name == university.APIListName).FirstOrDefault();
                    if(objectToChange != null)
                    {
                        objectToChange.name = nameToUpdate;
                    }
                }
                catch (Exception)
                {
                    //Did not found any university.
                }
            }

        }

        private static void deleteUniversity(University university, List<UEE> experiences, List<string> sqlComands, Dictionary<string, string> updateData)
        {
            var experiencesToDelete = experiences.Where(experience => experience.UniversityId == university.Id).ToList();

            if (university.NotListedInstitutionFlag == "x")
            {
                foreach (var experienceToDelete in experiencesToDelete)
                {
                    var univeristyName = "";
                    if (updateData["UpdateUniversity"] == null)
                    {
                        var nameBDList = university.Name.Split(Properties.UniverisyFacultyseparator);
                        univeristyName = nameBDList[0].Trim(' ');
                    }
                    else
                    {
                        univeristyName = updateData["UpdateUniversity"];
                    }


                    sqlComands.Add($"UPDATE UserEducationExperience SET UniversityId=NULL, NotListedInstitution='{univeristyName}' WHERE Id='{experienceToDelete.Id}';");
                }
            }
            else if(university.DeleteUniversity == "x")
            {
                foreach (var experienceToDelete in experiencesToDelete)
                {
                    sqlComands.Add($"DELETE FROM UserEducationExperience WHERE Id='{experienceToDelete.Id};'");
                }
            }

            sqlComands.Add($"DELETE FROM University WHERE Id='{university.Id};'");
        }

        private static void updateDBEntry(Dictionary<string, string> updateData, List<string> sqlComands, University university, bool changeFaculty)
        {
            //Get Data
            char[] charsToTrim = {' '};
            string deletedFlagDB = updateData["UpdateFlag"];
            string UpdateUniversity = updateData["UpdateUniversity"] != null? updateData["UpdateUniversity"].Trim(charsToTrim): null;
            string UpdateFaculty = updateData["UpdateFaculty"] != null? updateData["UpdateFaculty"].Trim(charsToTrim): null;

            var nameBDList = university.Name.Split(Properties.UniverisyFacultyseparator);
            var univeristyName = nameBDList[0].Trim(charsToTrim);

            //Build BD name collum
            string newDBname = null;
            if (changeFaculty) //Delete Faculty
            {
                if (nameBDList.Length != 1)//Faculty exist -- Mantain only university Name
                {
                    if (UpdateUniversity != null) //If we have change university -- change university 
                    {
                        newDBname = UpdateUniversity;
                    }
                    else //Else mantain same university
                    {
                        newDBname = univeristyName;
                    }
                }
                else //Faculty does not exist
                {
                    if (UpdateUniversity != null) //If we have change university -- change university 
                    {
                        newDBname = UpdateUniversity;
                    }
                    else //Else mantain same university
                    {
                        newDBname = univeristyName;
                    }
                }
            }
            else if (UpdateUniversity != null && UpdateFaculty != null) //change university and faculty.
            {
                newDBname = UpdateUniversity + "|" + UpdateFaculty;
            }
            else if (UpdateFaculty != null && UpdateUniversity == null) //change only faculty!
            {
                newDBname = univeristyName + "|" + UpdateFaculty;
            }
            else if (UpdateFaculty == null && UpdateUniversity != null) //change only university!
            {
                if (nameBDList.Length == 1)//Faculty does not exist -- Mantain only university Name
                {
                    newDBname = UpdateUniversity;
                }
                else //Faculty does exist -- Maintain Faculty name as well
                {
                    //faculty = actual faculty
                    var ActualFacultyName = university.Name.Substring(university.Name.IndexOf(Properties.UniverisyFacultyseparator) + 1).Trim();
                    newDBname = UpdateUniversity + "|" + ActualFacultyName;

                }
            }
            else if (UpdateFaculty == null && UpdateUniversity == null) //does not want to update anything --Put valid Data
            {
                if (nameBDList.Length == 1)//Faculty does not exist -- Trim UniversityName
                {
                    newDBname = univeristyName;
                }
                else //Faculty does exist -- Trim booth and join.
                {
                    var ActualFacultyName = university.Name.Substring(university.Name.IndexOf(Properties.UniverisyFacultyseparator) + 1).Trim();
                    newDBname = univeristyName + "|" + ActualFacultyName;

                }
            }
                //Build DB deleted collum
                string newDBflag = null;
            switch (deletedFlagDB)
            {
                case "V":
                    newDBflag = "1";
                    break;
                case "F":
                    newDBflag = "0";
                    break;
                default:
                    break;
            }


            //Build sql command.
            if (newDBname == null && newDBflag == null)
            {
              //Do nothing
            }
            else if (newDBname != null && newDBflag == null)
            {
                sqlComands.Add($"UPDATE University SET Name='{newDBname}' WHERE Id='{university.Id}';");
            }
            else if (newDBname == null && newDBflag != null)
            {
                sqlComands.Add($"UPDATE University SET Deleted='{newDBflag}' WHERE Id='{university.Id}';");
            }
            else if (newDBname != null && newDBflag != null) 
            {
                sqlComands.Add($"UPDATE University SET Name='{newDBname}', Deleted='{newDBflag}' WHERE Id='{university.Id}';");
            };
        }
    }
}
