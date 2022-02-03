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
            List<string> sqlComands = new List<string>();
            //List<University> universities = Helper.getUniversities("Data/UniversitiesSelected.csv");
            List<UniversityApiModel> apiUniversities = JsonConvert.DeserializeObject<List<UniversityApiModel>>(Helper.getJsonString("Data/universitiesApiList.json"));
            List<University> universities = JsonConvert.DeserializeObject<List<University>>(Helper.getJsonString("Data/universities.json"));

            //If university Name already exists -- UPDATE DATA
            foreach (var apiUniversity in apiUniversities)
            {
                var university =  universities.Where(u => u.Name == apiUniversity.name).FirstOrDefault();

                var newUniversityId = "'" + Guid.NewGuid() +"'";
                var nameAPI = apiUniversity.name != null ? "'" + apiUniversity.name + "'" : "NULL";
                var countryAPI = apiUniversity.country != null ? "'" + apiUniversity.country + "'" : "NULL";
                var countryCodeAPI = apiUniversity.alpha_two_code != null ? "'" + apiUniversity.alpha_two_code + "'" : "NULL";
                var domainAPI = apiUniversity.domains.Count() != 0 ? "'" + String.Join(",", apiUniversity.domains.ToArray())+ "'" : "NULL";
                var webPageAPI = apiUniversity.web_pages.Count() != 0 ? "'" + String.Join(",", apiUniversity.web_pages.ToArray())+ "'" : "NULL";

                if (university == null)
                {
                    //Insert Data University and Faculty.

                    sqlComands.Add($"INSERT INTO University(Id,Name,Country,CountryCode,Domain,WebPage,Deleted) VALUES({newUniversityId},{nameAPI},{countryAPI},{countryCodeAPI},{domainAPI},{webPageAPI},0);");
                    //sqlComands.Add($"INSERT Faculty SET Name={nameAPI} UniversityId='{newUniversityId}', Deleted='0';");
                }
                else
                {
                    sqlComands.Add($"UPDATE University SET Country={countryAPI}, CountryCode={countryCodeAPI}, Domain={domainAPI}, WebPage={webPageAPI} WHERE Id='{university.Id}';");
                }
            }

            Console.WriteLine("Writing Lines");
            Helper.insertSQLLine(sqlComands);
            Console.WriteLine("Finish");
        }
    }
}
