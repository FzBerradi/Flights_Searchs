using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Vol_Search_DotNet.Models
{
    public class ApiFlightOffer
    {
        [JsonProperty("data")]
        public List<FlightOffer> Data { get; set; }
    }

    public class FlightOffer
    {
        [JsonProperty("itineraries")]
        public List<Itinerary> Itineraries { get; set; }
    }

    public class Itinerary
    {
        [JsonProperty("segments")]
        public List<Segment> Segments { get; set; }
    }

    public class Segment
    {
        [JsonProperty("departure")]
        public FlightSegment Departure { get; set; }
        [JsonProperty("arrival")]
        public FlightSegment Arrival { get; set; }
    }

    public class FlightSegment
    {
        [JsonProperty("iataCode")]
        public string IataCode { get; set; }
    }
}