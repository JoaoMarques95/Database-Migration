using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PlaceCandidates
    {
        public List<SearchCadidates> candidates { get; set; }
        public string status { get; set; }
    }
}