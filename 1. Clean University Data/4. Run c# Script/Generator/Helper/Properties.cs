using System;
using System.Collections.Generic;
using System.Text;

namespace Generator
{
    public static class Properties
    {
        public static string csvFileDelemitor { get; set; } = ";";

        public static string UniverisyFacultyseparator { get; set; } = "-";

        public static string csvPathUEEInput { get; set; } = "Data/UEE.csv";
        public static string csvPathUniversityInput { get; set; } = "Data/UniversitiesSelected.csv";
        public static string FilePathSQLOutput { get; set; } = @"C:\Users\joaom\Desktop\BD refactoring\1. Clean University Data\4. Run c# Script\Generator\Result\script1.txt";
        public static string FilePathAPIListInput { get; set; } = "Data/universitiesApiList.json";
        public static string FilePathAPIListOutput { get; set; } = @"C:\Users\joaom\Desktop\BD refactoring\1. Clean University Data\4. Run c# Script\Generator\Data\universitiesApiListCorrected.json";
    }
}
