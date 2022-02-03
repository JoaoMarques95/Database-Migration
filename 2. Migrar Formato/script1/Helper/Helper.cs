using Generator.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Generator
{
    public class Helper
    {
        public static string separator = "|";
        public static string filePath = @"C:\Users\joaom\Desktop\BD refactoring\2. Migrar Formato\script1\Result\script1.txt";

        public static void insertSQLLine(List<string> sqlCommands)
        {

            File.WriteAllLines(filePath, sqlCommands);
        }

        public static string getJsonString(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

            using (var file = new StreamReader(fs, Encoding.UTF8))
            {
                return file.ReadToEnd();
            }
        }

    }
}
