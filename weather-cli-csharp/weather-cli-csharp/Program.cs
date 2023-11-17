using weather_cli_csharp.Models;
using weather_cli_csharp.Services;

public class Program
{
    private static void Main(string[] args)
    {
        WeatherService service = new WeatherService();

        Console.Write("Enter your zip code: ");
        string zipcode = Console.ReadLine();
        LatLon latLon = service.GetLatLon(zipcode);

        Console.WriteLine($"You are in {latLon.Name}");
        Console.WriteLine($"Latitude: {latLon.Lat}, Longitude: {latLon.Lon}");

        WeatherObject weatherObject = service.GetWeather(latLon);

        Console.WriteLine($"Temperature: {weatherObject.main.temp} Feels like: {weatherObject.main.feels_like}");
        Console.WriteLine($"Today's weather: {weatherObject.weather[0].description}");
        double feelsLike = weatherObject.main.feels_like;

        if(feelsLike < 50)
        {
            Console.WriteLine("Don't forget a jacket");
        }

    }
}