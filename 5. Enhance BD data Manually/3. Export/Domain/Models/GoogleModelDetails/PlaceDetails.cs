using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PlaceDetails
    {
        public Result result { get; set; }
    }

    public class Result
    {
        public List<AdressComponent> address_components { get; set; }
        public Geometry geometry { get; set; }
        public string formatted_address { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string place_id { get; set; }
        public string international_phone_number { get; set; }
        public string formatted_phone_number { get; set; }
        public string website { get; set; }

    }

    public class AdressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }

    }

    public class Location
    {
        public decimal lat { get; set; }
        public decimal lng { get; set; }
    }
}