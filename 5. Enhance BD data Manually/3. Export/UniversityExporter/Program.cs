using Ganss.Excel;
using Domain.Models;
using Domain.Helper;
using System;
using System.Linq;
using UnversityExplorer.Properties;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace UniversityExporter
{
    class Program
    {
        static async Task Main()
        {
            int i = 1;  
            List<string> sqlComands = new List<string>();
            List<University> universitiesBD = JsonSerializer.Deserialize<List<University>>(Helper.getJsonString("Data/universities.json"));
            List<University> universitiesToUpsert = new ExcelMapper(Properties.excelUniverisitesFile).Fetch<University>().ToList();

            var universitiesToUpsertFiltered = universitiesToUpsert.Where(u => u.Upsert =="x").ToList();

            foreach (var universityToUpsert in universitiesToUpsertFiltered)
            {
                Console.WriteLine(i++);
                bool exists = false;
                string universityId = null;


                //Check if university exists
                /*
                 1. If University has Id, It is to upsert
                 2. If University does not have Id, then Name must mach with some already existing university.
                 3. If not then create new university.
                 */

                if (!String.IsNullOrWhiteSpace(universityToUpsert.Id))
                {
                    exists = true;
                    universityId = universityToUpsert.Id;
                }
                else
                {
                    var universityBD = universitiesBD.Where(ubd => ubd.Name == universityToUpsert.Name).FirstOrDefault();

                    if (universityBD != null)
                    {
                        exists = true;
                        universityId = universityBD.Id;
                    }
                    else
                    {
                        exists = false;
                    }
                }


                //Map university Data
                if (exists)
                {
                    //Update existingData
                    UpdateSQLStatement(universityId, universityToUpsert, sqlComands);
                }
                else
                {
                    //Complement with googleAPIData
                    await CreateSQLStatement(universityToUpsert, sqlComands);
                }

                
            }
            Console.WriteLine("Writing");
            Helper.insertSQLLine(sqlComands,Properties.resultFilePath);
            Console.WriteLine("FInished");
        }

        private async static Task CreateSQLStatement(University universityToUpsert, List<string> sqlComands)
        {
            //Get data from GoogleAPI
            PlaceDetails universityGoogle = await Helper.GetGoogleDetails(universityToUpsert.Name);
            if (universityGoogle.result == null)
            {
                return;
            }
            //Map gogle Maps fields
            string placeId = null;
            string city = null;
            string countryCode = null;
            string contry = null;
            string postalCode = null;
            string googleMapsName = null;
            decimal? latitude = null;
            decimal? longitude = null;
            string url = null;
            string fullAdress = null;
            string website = null;
            string formatted_phone_number = null;
            string international_phone_number = null;

            Helper.mapGoogleDetails(universityGoogle, ref placeId, ref city, ref countryCode, ref contry, ref postalCode, ref googleMapsName, ref latitude, ref longitude, ref url, ref fullAdress, ref website, ref formatted_phone_number, ref international_phone_number);


            //Update SQL Statement
            List<String> colunms = new List<string>();
            List<String> values = new List<string>();


            Helper.returnInsertSQLStringVariable(universityToUpsert.Name, null, "Name", colunms, values);
            Helper.returnInsertSQLStringVariable(universityToUpsert.Country, contry, "Country", colunms, values);
            Helper.returnInsertSQLStringVariable(universityToUpsert.CountryCode, countryCode, "CountryCode", colunms, values);
            Helper.returnInsertSQLStringVariable(universityToUpsert.Domain, null, "Domain", colunms, values);
            Helper.returnInsertSQLStringVariable(universityToUpsert.WebPage, null, "WebPage", colunms, values);
            Helper.returnInsertSQLStringVariable(universityToUpsert.Abbreviations, null, "Abbreviations", colunms, values);
            Helper.returnInsertSQLStringVariable(universityToUpsert.City, city, "City", colunms, values);
            Helper.returnInsertSQLStringVariable(universityToUpsert.PostalCode, postalCode, "PostalCode", colunms, values);
            Helper.returnInsertSQLStringVariable(universityToUpsert.GoogleMapsName, googleMapsName, "GoogleMapsName", colunms, values);
            Helper.returnInsertSQLStringVariable(universityToUpsert.GooleMapsUrl, url, "GooleMapsUrl", colunms, values);
            Helper.returnInsertSQLStringVariable(universityToUpsert.GoogleMapsPlaceId, placeId, "GoogleMapsPlaceId", colunms, values);
            Helper.returnInsertSQLStringVariable(universityToUpsert.Latitude, latitude !=null? latitude.ToString():null, "Latitude", colunms, values);
            Helper.returnInsertSQLStringVariable(universityToUpsert.Longitude, longitude != null ? longitude.ToString() : null, "Longitude", colunms, values);
            Helper.returnInsertSQLStringVariable(universityToUpsert.Address, fullAdress, "Address", colunms, values);
            Helper.returnInsertSQLStringVariable(universityToUpsert.Address, website, "GoogleMapsWebPage", colunms, values);
            Helper.returnInsertSQLStringVariable(universityToUpsert.Address, formatted_phone_number, "FormatedPhoneNumber", colunms, values);
            Helper.returnInsertSQLStringVariable(universityToUpsert.Address, international_phone_number, "InternationalPhoneNumber", colunms, values);

            //Remove null values
            for (int i = 0; i < colunms.Count;)
            {
                if (colunms[i] == null)
                {
                    colunms.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }

            //Remove null values
            for (int i = 0; i < values.Count;)
            {
                if (values[i] == null)
                {
                    values.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }

            var colunmsEntries = String.Join(",", colunms);
            var valuesEntries = String.Join(",", values);

            sqlComands.Add($"INSERT INTO University ({colunmsEntries}) VALUES({valuesEntries}) ;");
        }

        private static void UpdateSQLStatement(string universityId, University universityToUpsert, List<string> sqlComands)
        {
            //Update SQL Statement
            List<String> UpdateEntries = new List<string>();

            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(universityToUpsert.Name, "Name"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(universityToUpsert.Country, "Country"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(universityToUpsert.CountryCode, "CountryCode"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(universityToUpsert.Domain, "Domain"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(universityToUpsert.WebPage, "WebPage"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(universityToUpsert.Abbreviations, "Abbreviations"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(universityToUpsert.City, "City"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(universityToUpsert.PostalCode, "PostalCode"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(universityToUpsert.GoogleMapsName, "GoogleMapsName"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(universityToUpsert.GoogleMapsPlaceId, "GoogleMapsPlaceId"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(universityToUpsert.GooleMapsUrl, "GooleMapsUrl"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(universityToUpsert.Latitude, "Latitude"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(universityToUpsert.Longitude, "Longitude"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(universityToUpsert.Address, "Address"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(universityToUpsert.GoogleMapsWebPage, "GoogleMapsWebPage"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(universityToUpsert.InternationalPhoneNumber, "InternationalPhoneNumber"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(universityToUpsert.FormatedPhoneNumber, "FormatedPhoneNumber"));

            //Remove null values
            for (int i = 0; i < UpdateEntries.Count;)
            {
                if (UpdateEntries[i] == null)
                {
                    UpdateEntries.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }

            var rawUpdateEntries = String.Join(",", UpdateEntries);
            sqlComands.Add($"UPDATE University SET {rawUpdateEntries} WHERE Id='{universityId}';");
        }

    }
}
