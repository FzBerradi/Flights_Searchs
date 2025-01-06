using System;
using System.Collections.Generic;
namespace Vol_Search_DotNet.Models
{
    public class segments
    {
        public departure departure { get; set; }
        public arrival arrival { get; set; }
        public string carrierCode { get; set; }
        public int number { get; set; }
        public string duration { get; set; }
        public int id { get; set; }
        public int numberOfStops { get; set; }
        public Boolean blacklistedInEU { get; set; }
    }
}
