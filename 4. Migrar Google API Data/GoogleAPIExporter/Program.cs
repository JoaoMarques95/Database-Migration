using Generator;
using Generator.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GoogleAPIExporter
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        static async Task Main()
        {
            int i = 0;
            List<string> sqlComands = new List<string>();
            List<University> universities = JsonSerializer.Deserialize<List<University>>(Helper.getJsonString("Data/universities.json"));
            List<Faculty> faculties = JsonSerializer.Deserialize<List<Faculty>>(Helper.getJsonString("Data/faculties.json"));

            
            var universitiesFilteredByContry = universities.Where(u => Porperties.FilterUniveristyCountryCode.Contains("all") ? true : Porperties.FilterUniveristyCountryCode.Contains(u.CountryCode)).ToList();


            foreach (var university in universitiesFilteredByContry)
            {
                if (Porperties.updateGoogleDataOnlyIfDoesNotExist)
                {
                    //If true then check if entity already has data.
                    if (!String.IsNullOrWhiteSpace(university.GoogleMapsPlaceId))
                    {
                        continue;
                    }
                }
                Console.WriteLine(i++);
                await setGoogleDetailsInSQL(sqlComands, university.Name, university.Id, false);
            }


            foreach (var faculty in faculties)
            {
                if (Porperties.updateGoogleDataOnlyIfDoesNotExist)
                {
                    //If true then check if entity already has data.
                    if (!String.IsNullOrWhiteSpace(faculty.GoogleMapsPlaceId))
                    {
                        continue;
                    }
                }
                Console.WriteLine(i++);
                var FacultyUniversity = universities.Where(u => u.Id == faculty.UniversityId).FirstOrDefault();

                if (FacultyUniversity != null)
                {
                    var searchName = FacultyUniversity.Name + " " + faculty.Name;
                    await setGoogleDetailsInSQL(sqlComands, searchName, faculty.Id, true);
                }
            }

            Console.WriteLine("Writing");
            Helper.insertSQLLine(sqlComands);
            Console.WriteLine("FInished");

        }

        private static async Task setGoogleDetailsInSQL(List<string> sqlComands, string name, string id, bool isFaculty)
        {
            //Call Google API -- Search Candidates
            PlaceCandidates candidates = await API.SearchCadidates(name, Porperties.seachFields, Porperties.searchInputType, Porperties.apiKey);

            //Get the google Details for the first university "candidate".
            foreach (var candidate in candidates.candidates)
            {
                if (Porperties.ValidateCandidateTypes)
                {
                    if (!(candidate.types.Contains("university") || candidate.types.Contains("school") || candidate.types.Contains("secondary_school")))
                    {
                        continue;
                    }
                }

                PlaceDetails PlaceDetails = await API.GetPlaceDetails(candidate.place_id, Porperties.detailsFields, Porperties.apiKey);
                if (PlaceDetails.result != null)
                {
                    Update(PlaceDetails, id, sqlComands, isFaculty);
                    break;
                }
            }
        }

        private static void Update(PlaceDetails placeDetails, string universityId, List<string> sqlComands, bool isFaculty)
        {
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

            //Map City & Postal Code & Contry & ContryCode
            if (placeDetails.result.address_components != null)
            {
                //check "administrative_area_level_1" if does not exist check "locality".
                var cityObject = placeDetails.result.address_components.Where(c => c.types != null ? c.types.Contains("locality") : false).FirstOrDefault();
                var cityComponent = cityObject != null ? cityObject.long_name : null;

                if (cityComponent == null)
                {
                    cityObject = placeDetails.result.address_components.Where(c => c.types != null ? c.types.Contains("administrative_area_level_1") : false).FirstOrDefault();
                    cityComponent = cityObject != null ? cityObject.long_name : null;
                }
                city = (cityComponent != null) ? cityComponent : null;

                //Map Postal Code
                var postalCodeObject = placeDetails.result.address_components.Where(c => c.types != null ? c.types.Contains("postal_code") : false).FirstOrDefault();
                var postalCodeComponent = postalCodeObject != null ? postalCodeObject.long_name : null;
                postalCode = (postalCodeComponent != null) ? postalCodeComponent : null;

                //Map Contry
                var ContryObject = placeDetails.result.address_components.Where(c => c.types != null ? c.types.Contains("country") : false).FirstOrDefault();
                var ContryComponent = ContryObject != null ? ContryObject.long_name : null;
                contry = (ContryComponent != null) ? ContryComponent : null;

                // Map Country Code
                var ContryCodeObject = placeDetails.result.address_components.Where(c => c.types != null ? c.types.Contains("country") : false).FirstOrDefault();
                var ContryCodeComponent = ContryCodeObject != null ? ContryCodeObject.short_name : null;
                countryCode = (ContryCodeComponent != null) ? ContryCodeComponent : null;
            }

            // Map laitude and longitude
            if (placeDetails.result.geometry != null)
            {
                if (placeDetails.result.geometry.location != null)
                {
                    latitude = placeDetails.result.geometry.location.lat;
                    longitude = placeDetails.result.geometry.location.lng;
                }
            }

            //Map Name && URL
            placeId = (placeDetails.result.place_id != null) ? placeDetails.result.place_id : null;
            googleMapsName = (placeDetails.result.name != null) ? placeDetails.result.name : null;
            url = (placeDetails.result.url != null) ? placeDetails.result.url : null;
            fullAdress = (placeDetails.result.formatted_address != null) ? placeDetails.result.formatted_address : null;
            website = (placeDetails.result.website != null) ? placeDetails.result.website : null;
            formatted_phone_number = (placeDetails.result.formatted_phone_number != null) ? placeDetails.result.formatted_phone_number : null;
            international_phone_number = (placeDetails.result.international_phone_number != null) ? placeDetails.result.international_phone_number : null;


            //Update SQL Statement
            List<String> UpdateEntries = new List<string>();

            string placeIdSQL = placeId != null ? $"GoogleMapsPlaceId='{placeId.Replace(Porperties.invalidSQLCharacter, Porperties.validSQLCharacter)}'" : null;
            UpdateEntries.Add(placeIdSQL);

            string citySQL = city != null ? $"City='{city.Replace(Porperties.invalidSQLCharacter, Porperties.validSQLCharacter)}'" : null;
            UpdateEntries.Add(citySQL);

            string postalCodeSQL = postalCode != null ? $"PostalCode='{postalCode.Replace(Porperties.invalidSQLCharacter, Porperties.validSQLCharacter)}'" : null;
            UpdateEntries.Add(postalCodeSQL);

            string googleMapsNameSQL = googleMapsName != null ? $"GoogleMapsName='{googleMapsName.Replace(Porperties.invalidSQLCharacter, Porperties.validSQLCharacter)}'" : null;
            UpdateEntries.Add(googleMapsNameSQL);

            string latitudeSQL = latitude != null ? $"Latitude='{latitude}'" : null;
            UpdateEntries.Add(latitudeSQL);

            string longitudeSQL = longitude != null ? $"Longitude='{longitude}'" : null;
            UpdateEntries.Add(longitudeSQL);

            string urlSQL = url != null ? $"GooleMapsUrl='{url}'" : null;
            UpdateEntries.Add(urlSQL);

            string fullAdressSQL = fullAdress != null ? $"Address='{fullAdress.Replace(Porperties.invalidSQLCharacter, Porperties.validSQLCharacter)}'" : null;
            UpdateEntries.Add(fullAdressSQL);

            string websiteSQL = website != null ? $"GoogleMapsWebPage='{website}'" : null;
            UpdateEntries.Add(websiteSQL);

            string formattedPhoneNumberSQL = formatted_phone_number != null ? $"FormatedPhoneNumber='{formatted_phone_number.Replace(Porperties.invalidSQLCharacter, Porperties.validSQLCharacter)}'" : null;
            UpdateEntries.Add(formattedPhoneNumberSQL);

            string internationalPhoneNumber = international_phone_number != null ? $"InternationalPhoneNumber='{international_phone_number.Replace(Porperties.invalidSQLCharacter, Porperties.validSQLCharacter)}'" : null;
            UpdateEntries.Add(internationalPhoneNumber);

            //Add Contry Code and Contry as well
            UpdateEntries.Add(countryCode != null ? $"CountryCode='{countryCode}'" : null);
            UpdateEntries.Add(contry != null ? $"Country='{contry.Replace(Porperties.invalidSQLCharacter, Porperties.validSQLCharacter)}'" : null);

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
            var entity = isFaculty ? "Faculty" : "University";
            sqlComands.Add($"UPDATE {entity} SET {rawUpdateEntries} WHERE Id='{universityId}';");
        }
    }
}
