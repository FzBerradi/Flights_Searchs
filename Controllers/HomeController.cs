using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Linq;
using Vol_Search_DotNet.Models;

namespace Vol_Search_DotNet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]

        public IActionResult Privacy()
        {
            return View("Privacy");
        }
        public IActionResult About()
        {
            return View("About");
        }
		[HttpPost]
		public JsonResult Index(string Prefix)
        {
			StreamReader r = new StreamReader("wwwroot/js/airports.json");
			string jsonString = r.ReadToEnd();
			List<Vol> airports = JsonConvert.DeserializeObject<List<Vol>>(jsonString);
			var cities = (from N in airports
						  where N.city.StartsWith(Prefix)
						  select new { N.city });
			return Json(cities);
		}

			public static string getAirportFullName(string code)
        {

            StreamReader r = new StreamReader("wwwroot/js/airports.json");
            string jsonString = r.ReadToEnd();
            List<Airport> airports = JsonConvert.DeserializeObject<List<Airport>>(jsonString);

            foreach (var i in airports)
            {
                if (i.code == code)
                {
                    return i.name;
                }
            }
            return "";
        }
        public static string getAirportcode(string city)
        {

            StreamReader r = new StreamReader("wwwroot/js/airports.json");
            string jsonString = r.ReadToEnd();
            List<Airport> airports = JsonConvert.DeserializeObject<List<Airport>>(jsonString);

            foreach (var i in airports)
            {
                if (i.city == city)
                {
                    return i.code;
                }
            }
            return "";
        }
      public IActionResult Result(Vol vol)
        {
            

            Vol newVol = new Vol();
            newVol.departure = vol.departure;
            newVol.arrival = vol.arrival; newVol.adults = vol.adults;
            newVol.flight_date_d = vol.flight_date_d;
            newVol.flight_date_a = vol.flight_date_a;
            newVol.flight_type = vol.flight_type;
            newVol.travel_class = vol.travel_class;
            newVol.non_stop = vol.non_stop;
            if (newVol.flight_type == "One way")
            {
                
                    var date_dep = newVol.flight_date_d.ToString("yyyy-MM-dd");
                    var resp = getFlightsOneWay(newVol.departure, newVol.arrival, date_dep, newVol.adults,  newVol.travel_class, newVol.non_stop);
                if (resp is string)
                {
                    ModelState.AddModelError("", "Sorry, Please try to fill in the  required fields to have all correct information!!!");
					return View("index");

				}
				else
                {
                    ViewBag.Message = resp;
                    var mydataa = ViewBag.Message.data;

                    foreach (var obj in mydataa)
                    {
                        foreach (var it in obj.itineraries)
                        {
                            foreach (var seg in it.segments)
                            {
                                var mycodeAller = getAirportFullName(seg.departure.iataCode.ToString());
                                seg.departure.iataCode = mycodeAller;
                                var mycodeArrivER = getAirportFullName(seg.arrival.iataCode.ToString());
                                seg.arrival.iataCode = mycodeArrivER;

                            }

                        }
                    }
                    ViewBag.Vol = newVol;
                    return View("OneWay");

                }


            }
            else
            {
                
                var date_dep = newVol.flight_date_d.ToString("yyyy-MM-dd");
                var date_arr = newVol.flight_date_a.ToString("yyyy-MM-dd");
                System.Diagnostics.Debug.WriteLine(date_arr);
                System.Diagnostics.Debug.WriteLine(date_dep);


                var resp = getFlightsRoundtrip(newVol.departure, newVol.arrival, date_dep, date_arr, newVol.adults, newVol.travel_class, newVol.non_stop);
                System.Diagnostics.Debug.WriteLine("Resultat de reponse" + resp);
				if (resp is string)
				{
                    ModelState.AddModelError("", "Please try to fill in all required fields to see all correct information!!!");
                    return View("index");
                }
                else
                {
					ViewBag.Message = resp;

					var mydataa = ViewBag.Message.data;
					foreach (var obj in mydataa)
					{
						foreach (var it in obj.itineraries)
						{
							foreach (var seg in it.segments)
							{
								var mycodeAller = getAirportFullName(seg.departure.iataCode.ToString());
								seg.departure.iataCode = mycodeAller;
								var mycodeArrivER = getAirportFullName(seg.arrival.iataCode.ToString());
								seg.arrival.iataCode = mycodeArrivER;

							}

						}
					}
					ViewBag.Vol = newVol;
					return View("RoundTrip");
				}

					
            }

        }
        public Object getFlightsOneWay(string depart, string arriver, string d, int adults, string travel_class, string non_stop)
        {
            var codeAiroDepart = getAirportcode(depart);
            var codeAiroArrive = getAirportcode(arriver);
            string accessToken = getAccessToken();
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.BaseAddress = new Uri("https://test.api.amadeus.com/v2/shopping/");

            var obj = httpClient.GetAsync("flight-offers?originLocationCode=" + codeAiroDepart + "&destinationLocationCode=" + codeAiroArrive + "&departureDate=" + d + "&adults=" + adults + "&travelClass=" + travel_class + "&nonStop=" + non_stop).Result;
            System.Diagnostics.Debug.WriteLine("flight-offers?originLocationCode=" + codeAiroDepart + "&destinationLocationCode=" + codeAiroArrive + "&departureDate=" + d + "&adults=" + adults +  "&travelClass=" + travel_class + "&nonStop=" + non_stop);
            System.Diagnostics.Debug.WriteLine(obj);
            if (obj.IsSuccessStatusCode)
            {
                var response = obj.Content.ReadAsStringAsync().Result;

                Api list = JsonConvert.DeserializeObject<Api>(response);

                System.Diagnostics.Debug.WriteLine("resultat  : " + list);
                System.Diagnostics.Debug.WriteLine("resultat  : " + list.data);
                foreach (var item in list.data)
                {
                    System.Diagnostics.Debug.WriteLine("resultat  : " + item);
                }
                return list;
            }
            
            return "error ... ";
        }
        public Object getFlightsRoundtrip(string depart, string arriver, string date_dep, string date_arr, int adults, string travel_class, string non_stop)
        {
            var codeAiroDepart = getAirportcode(depart);
            var codeAiroArri = getAirportcode(arriver);
            string accessToken = getAccessToken();
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.BaseAddress = new Uri("https://test.api.amadeus.com/v2/shopping/");
            var obj = httpClient.GetAsync("flight-offers?originLocationCode=" + codeAiroDepart + "&destinationLocationCode=" + codeAiroArri + "&departureDate=" + date_dep + "&returnDate=" + date_arr + "&adults=" + adults + "&travelClass=" + travel_class + "&nonStop=" + non_stop).Result;
            System.Diagnostics.Debug.WriteLine("flight-offers?originLocationCode=" + codeAiroDepart + "&destinationLocationCode=" + codeAiroArri + "&departureDate=" + date_dep + "&returnDate=" + date_arr + "&adults=" + adults + "&children=" + "&travelClass=" + travel_class + "&nonStop=" + non_stop);
            if (obj.IsSuccessStatusCode)
            {
                var response = obj.Content.ReadAsStringAsync().Result;

                Api list = JsonConvert.DeserializeObject<Api>(response);

                System.Diagnostics.Debug.WriteLine("resultat  : " + list);
                System.Diagnostics.Debug.WriteLine("resultat  : " + list.data);
                foreach (var item in list.data)
                {
                    System.Diagnostics.Debug.WriteLine("resultat  : " + item);
                }
                return list;
            }


            return "error ... ";
        }
        
        public string getAccessToken()
        {
            string accessToken = "";
            //pour envoyer des requêtes HTTP
            HttpClient httpClient = new HttpClient();
            ClientSecure credentials = new ClientSecure();
            credentials.grant_type = "client_credentials";
            credentials.client_id = "ReI5I74kFNAfF3kBpAWGLt8P2QWfGL7C";
            credentials.client_secret = "3mxf2ChomMBFOCYA";
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("client_id", "ReI5I74kFNAfF3kBpAWGLt8P2QWfGL7C")); //api key
            nvc.Add(new KeyValuePair<string, string>("client_secret", "3mxf2ChomMBFOCYA")); //api secret _ amadeus developer website account
            nvc.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));

            var req = new HttpRequestMessage(HttpMethod.Post, "https://test.api.amadeus.com/v1/security/oauth2/token")
            {
                Content = new FormUrlEncodedContent(nvc)
            };

            using (HttpResponseMessage result = httpClient.SendAsync(req).Result)
            {
                string resultJson = result.Content.ReadAsStringAsync().Result;
                Jeton list = JsonConvert.DeserializeObject<Jeton>(resultJson);
                accessToken = list.access_token;
                System.Diagnostics.Debug.WriteLine("resultat  TOKEN: {} = " + list.access_token);
            }



            return accessToken;
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}  