using System;
namespace Vol_Search_DotNet.Models
{
    public class Vol
    {
        public string departure { get; set; }
        public string arrival { get; set; }
		public DateTime flight_date_d { get; set; }
		public DateTime flight_date_a { get; set; }
		public string flight_type { get; set; }
        public int adults { get; set; }
        public string travel_class { get; set; }
        public string non_stop { get; set; } = "false";
		public string city { get; set; }
	}

}
