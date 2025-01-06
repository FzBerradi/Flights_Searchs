using System;
namespace Vol_Search_DotNet.Models
{
    public class ClientSecure
    {
        public ClientSecure()
        {
        }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string grant_type { get; set; }
    }
}
