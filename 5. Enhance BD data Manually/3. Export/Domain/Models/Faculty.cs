using Ganss.Excel;

namespace Domain.Models
{
    public class Faculty
    {
        public string Upsert { get; set; }
        [FormulaResult]
        public string UniversityName { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string UniversityId { get; set; }
        public string Deleted { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Domain { get; set; }
        public string WebPage { get; set; }
        public string Abbreviations { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string GoogleMapsName { get; set; }
        public string GoogleMapsPlaceId { get; set; }
        public string GooleMapsUrl { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
        public string GoogleMapsWebPage { get; set; }
        public string InternationalPhoneNumber { get; set; }
        public string FormatedPhoneNumber { get; set; }

    }
}