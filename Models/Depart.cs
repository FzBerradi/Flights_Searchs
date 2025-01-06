using System;
namespace Vol_Search_DotNet.Models
{
    public class departure
    {
        public departure() { }
        public string iataCode { get; set; }
        public string terminal { get; set; }
        public DateTime at { get; set; }
    }
}
