using System.Collections.Generic;

namespace Domain.Models
{
    public class SearchCadidates
    {
        public string name { get; set; }
        public string place_id { get; set; }
        public List<string> types { get; set; }

    }
}