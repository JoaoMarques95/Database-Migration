using Generator.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Generator
{
    public class Helper
    {
        private static string[] returnLines(string path)
        {
            var lines = new List<string>();
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            using (var sr = new StreamReader(fs, Encoding.GetEncoding("iso-8859-1")))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
            return lines.ToArray();
        }

       public static List<University> getUniversities(string path)
        {
            return returnLines(path)
                .Skip(1)
                .Where(line => line.Length > 1)
                .Select(University.ParseFromCsv)
                .ToList();
        }

        public static List<UEE> getUEE(string path)
        {
            return returnLines(path)
                .Skip(1)
                .Where(line => line.Length > 1)
                .Select(UEE.ParseFromCsv)
                .ToList();
        }
        
        public static void insertSQLLine(string path ,List<string> sqlCommands)
        {
            File.WriteAllLines(path, sqlCommands);
        }

        public static string getJsonString(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

            using (var file = new StreamReader(fs, Encoding.UTF8))
            {
                return file.ReadToEnd();
            }
        }
        public static void writeJsonString(string path, List<UniversityApiModel> newAPIList)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(newAPIList));
        }
    }
}
