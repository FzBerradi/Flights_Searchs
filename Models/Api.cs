using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Vol_Search_DotNet.Models
{
    public class meta
    {
        public meta() { }
    }
    public class Api
    {
        
        public Api() { }
        [JsonProperty("meta")]
        public meta meta { get; set; }
        [JsonProperty("data")]
        public List<Resultss> data { get; set; }
    }
}