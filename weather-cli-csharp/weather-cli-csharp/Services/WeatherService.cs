using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using weather_cli_csharp.Models;

namespace weather_cli_csharp.Services
{
    public class WeatherService
    {
        //Properties
        const string API_KEY = "2009e83d829e4f5409a60b82b518ecd2";

        private static RestClient client = null;

        const string API_URL = "http://api.openweathermap.org/";

        //Constructor
        public WeatherService()
        {
            if (client == null)
            {
                client = new RestClient(API_URL);
            }
        }

        //Methods

        public LatLon GetLatLon(string zip)
        {
            RestRequest request = new RestRequest($"{API_URL}geo/1.0/zip?zip={zip}&appid={API_KEY}");
            IRestResponse<LatLon> response = client.Get<LatLon>(request);
            CheckForError(response);
            return response.Data;
        }

        public WeatherObject GetWeather(LatLon latLon)
        {
            RestRequest request = new RestRequest($"{API_URL}data/2.5/weather?lat={latLon.Lat}&lon={latLon.Lon}&units=imperial&appid={API_KEY}");
            IRestResponse<WeatherObject> response = client.Get<WeatherObject>(request);
            CheckForError(response);
            return response.Data;
        }

        private void CheckForError(IRestResponse response)
        {
            if (!response.IsSuccessful)
            {
                // TODO: Write a log message for future reference

                throw new HttpRequestException($"There was an error in the call to the server");
            }
        }
    }
}
