using Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Domain.Helper
{
    public class Helper
    {
        public static void insertSQLLine(List<string> sqlCommands, string path)
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

        public static string returnUpdateSQLStringVariable(string variable, string fieldName)
        {
            return (!String.IsNullOrWhiteSpace(variable)) ? $"{fieldName}='{variable.Replace(GenericProperties.invalidSQLCharacter, GenericProperties.validSQLCharacter)}'" : null;
        }

        public static void returnInsertSQLStringVariable(string upsertVariable, string googleField, string entity, List<String> colunms, List<String> values)
        {

            upsertVariable = !String.IsNullOrWhiteSpace(upsertVariable) ? upsertVariable.Replace(GenericProperties.invalidSQLCharacter, GenericProperties.validSQLCharacter) : null;
            googleField = !String.IsNullOrWhiteSpace(googleField) ? googleField.Replace(GenericProperties.invalidSQLCharacter, GenericProperties.validSQLCharacter) : null;

            //If something exists
            if (upsertVariable != null || googleField != null)
            {
                colunms.Add(entity);
                var UpsertValue = upsertVariable != null ? upsertVariable : googleField;
                values.Add($"'{UpsertValue}'");
            }
        }

        public static async Task<PlaceDetails> GetGoogleDetails(string name)
        {
            PlaceDetails place = new PlaceDetails();
            PlaceCandidates candidates = await API.SearchCadidates(name);

            //Get the google Details for the first university "candidate".
            foreach (var candidate in candidates.candidates)
            {
                if (GenericProperties.ValidateCandidateTypes)
                {
                    if (!(candidate.types.Contains("university") || candidate.types.Contains("school") || candidate.types.Contains("secondary_school")))
                    {
                        continue;
                    }
                }

                PlaceDetails PlaceDetails = await API.GetPlaceDetails(candidate.place_id);
                if (PlaceDetails.result != null)
                {
                    place = PlaceDetails;
                    return place;
                }

            }
            return place;
        }

        public static void mapGoogleDetails(PlaceDetails placeDetails, ref string placeId, ref string city, ref string countryCode, ref string contry, ref string postalCode, ref string googleMapsName, ref decimal? latitude, ref decimal? longitude, ref string url, ref string fullAdress, ref string website, ref string formatted_phone_number, ref string international_phone_number)
        {
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
        }
    }
}
