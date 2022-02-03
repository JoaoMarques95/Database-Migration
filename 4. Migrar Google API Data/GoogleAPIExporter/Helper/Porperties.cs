using Generator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Generator
{
    public class Porperties
    {
        public static string resultFilePath = @"C:\Users\joaom\Desktop\networkme\data\university_data_migration\4. Migrar Google API Data\GoogleAPIExporter\Result\script1.txt";
        public static string apiKey = "xxxx";
        public static string searchInputType = "textquery";
        public static string seachFields = "place_id,type";
        public static string detailsFields = "address_component,formatted_address,geometry,name,url,place_id,formatted_phone_number,international_phone_number,website";
        public static string invalidSQLCharacter = "'";
        public static string validSQLCharacter = "''";

        public static bool updateGoogleDataOnlyIfDoesNotExist  = false;
        public static bool ValidateCandidateTypes = true;
        public static string FilterUniveristyCountryCode = "all"; //"all" if you want to run for all universities.

    }
}
