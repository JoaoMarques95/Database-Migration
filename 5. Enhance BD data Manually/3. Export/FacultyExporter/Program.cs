using Ganss.Excel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Helper;

namespace FacultyExplorer
{
    class Program
    {
        static async Task Main()
        {
            int i = 1;
            List<string> sqlComands = new List<string>();
            List<University> universitiesBD = JsonSerializer.Deserialize<List<University>>(Helper.getJsonString("Data/universities.json"));
            List<Faculty> facultiesBD = JsonSerializer.Deserialize<List<Faculty>>(Helper.getJsonString("Data/faculties.json"));
            List<Faculty> facultiesToUpsert = new ExcelMapper(Properties.excelUniverisitesFile).Fetch<Faculty>().ToList();


            var facultiesToUpsertFiltered = facultiesToUpsert.Where(f => f.Upsert == "x").ToList();
            foreach (var facultyToUpsert in facultiesToUpsertFiltered)
            {
                Console.WriteLine(i++);
                bool exists = false;
                string universityId = null;
                string facultyId = null;
               
                /* -------- Rules --------
                 If exists Faculty Id
                 Else:
                    If Faculty has UniveristyName that exist (match by name)
                       If: Univerisity already has the faculty (match by name)
                           Update Faculty
                       Else:
                           Create Faculty
                    Else:
                        Invalid Request
                 */

                if (!String.IsNullOrEmpty(facultyToUpsert.Id))
                {
                    exists = true;
                    universityId = facultyToUpsert.UniversityId;
                    facultyId = facultyToUpsert.Id;
                }
                else
                {
                    if (!String.IsNullOrEmpty(facultyToUpsert.UniversityName))
                    {
                        //University Exists?
                        University matchUniversity = universitiesBD.Where(u => u.Name == facultyToUpsert.UniversityName).FirstOrDefault();
                        if (matchUniversity != null)
                        {
                            //University already has the faculty? In university, has faculty name!
                            Faculty matchFaculty = facultiesBD.Where(f => f.UniversityId == matchUniversity.Id && f.Name == facultyToUpsert.Name).FirstOrDefault();
                            if (matchFaculty != null)
                            {
                                exists = true;
                                universityId = matchUniversity.Id;
                                facultyId = matchFaculty.Id;
                            }
                            else
                            {
                                exists = false;
                                universityId = matchUniversity.Id;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                //Update apenas aos campos novos que estão no excel (Passar FacultyId --  que deram match no nome).
                //Create criar apenas com os campos que estão no excel (Passar universityId -- que deu match pelo nome).
                if (exists)
                {
                    //Update Faculty
                    UpdateSQLStatement(facultyId, facultyToUpsert, sqlComands);
                }
                else
                {
                    //Create Faculty and complement with googleAPIData
                    await CreateSQLStatement(facultyToUpsert,universityId, sqlComands);
                }
            }
            Console.WriteLine("Writing");
            Helper.insertSQLLine(sqlComands, Properties.resultFilePath);
            Console.WriteLine("FInished");
        }

        private async static Task CreateSQLStatement(Faculty facultyToUpsert, string universityId, List<string> sqlComands)
        {
            //Get data from GoogleAPI
            PlaceDetails facultyGoogle = await Helper.GetGoogleDetails(facultyToUpsert.UniversityName + " " + facultyToUpsert.Name);

            if(facultyGoogle.result == null)
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

            Helper.mapGoogleDetails(facultyGoogle, ref placeId, ref city, ref countryCode, ref contry, ref postalCode, ref googleMapsName, ref latitude, ref longitude, ref url, ref fullAdress, ref website, ref formatted_phone_number, ref international_phone_number);


            //Update SQL Statement
            List<String> colunms = new List<string>();
            List<String> values = new List<string>();


            Helper.returnInsertSQLStringVariable(facultyToUpsert.Name, null, "Name", colunms, values);
            Helper.returnInsertSQLStringVariable(facultyToUpsert.Country, contry, "Country", colunms, values);
            Helper.returnInsertSQLStringVariable(facultyToUpsert.CountryCode, countryCode, "CountryCode", colunms, values);
            Helper.returnInsertSQLStringVariable(facultyToUpsert.Domain, null, "Domain", colunms, values);
            Helper.returnInsertSQLStringVariable(facultyToUpsert.WebPage, null, "WebPage", colunms, values);
            Helper.returnInsertSQLStringVariable(facultyToUpsert.Abbreviations, null, "Abbreviations", colunms, values);
            Helper.returnInsertSQLStringVariable(facultyToUpsert.City, city, "City", colunms, values);
            Helper.returnInsertSQLStringVariable(facultyToUpsert.PostalCode, postalCode, "PostalCode", colunms, values);
            Helper.returnInsertSQLStringVariable(facultyToUpsert.GoogleMapsName, googleMapsName, "GoogleMapsName", colunms, values);
            Helper.returnInsertSQLStringVariable(facultyToUpsert.GooleMapsUrl, url, "GooleMapsUrl", colunms, values);
            Helper.returnInsertSQLStringVariable(facultyToUpsert.GoogleMapsPlaceId, placeId, "GoogleMapsPlaceId", colunms, values);
            Helper.returnInsertSQLStringVariable(facultyToUpsert.Latitude, latitude != null ? latitude.ToString() : null, "Latitude", colunms, values);
            Helper.returnInsertSQLStringVariable(facultyToUpsert.Longitude, longitude != null ? longitude.ToString() : null, "Longitude", colunms, values);
            Helper.returnInsertSQLStringVariable(facultyToUpsert.Address, fullAdress, "Address", colunms, values);
            Helper.returnInsertSQLStringVariable(facultyToUpsert.Address, website, "GoogleMapsWebPage", colunms, values);
            Helper.returnInsertSQLStringVariable(facultyToUpsert.Address, formatted_phone_number, "FormatedPhoneNumber", colunms, values);
            Helper.returnInsertSQLStringVariable(facultyToUpsert.Address, international_phone_number, "InternationalPhoneNumber", colunms, values);
            Helper.returnInsertSQLStringVariable(universityId, null, "UniversityId", colunms, values);

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

            sqlComands.Add($"INSERT INTO Faculty ({colunmsEntries}) VALUES({valuesEntries}) ;");
        }

        private static void UpdateSQLStatement(string facultyId, Faculty facultyToUpsert, List<string> sqlComands)
        {
            //Update SQL Statement
            List<String> UpdateEntries = new List<string>();

            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(facultyToUpsert.Name, "Name"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(facultyToUpsert.Country, "Country"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(facultyToUpsert.CountryCode, "CountryCode"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(facultyToUpsert.Domain, "Domain"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(facultyToUpsert.WebPage, "WebPage"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(facultyToUpsert.Abbreviations, "Abbreviations"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(facultyToUpsert.City, "City"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(facultyToUpsert.PostalCode, "PostalCode"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(facultyToUpsert.GoogleMapsName, "GoogleMapsName"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(facultyToUpsert.GoogleMapsPlaceId, "GoogleMapsPlaceId"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(facultyToUpsert.GooleMapsUrl, "GooleMapsUrl"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(facultyToUpsert.Latitude, "Latitude"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(facultyToUpsert.Longitude, "Longitude"));
            UpdateEntries.Add(Helper.returnUpdateSQLStringVariable(facultyToUpsert.Address, "Address"));


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
            sqlComands.Add($"UPDATE Faculty SET {rawUpdateEntries} WHERE Id='{facultyId}';");
        }

    }
}
