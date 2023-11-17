using System;
using System.Collections.Generic;
using System.Text;
using WeatherDB.Cli.Models;

namespace WeatherDb.Cli.DAO
{
    public interface IWeatherDao
    {
        WeatherObject CreateWeather(int userId, WeatherObject eatherObject, int zipcode);

        WeatherObject GetWeatherByUserId(int userId);
    }
}
