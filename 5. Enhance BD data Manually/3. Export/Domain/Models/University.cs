using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class University
    {
        public string Upsert { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
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