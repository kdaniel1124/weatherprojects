using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Markup;
using WeatherDB.Cli.Models;

namespace WeatherDb.Cli.DAO
{
    public class WeatherSqlDao : IWeatherDao
    {
        private readonly string connectionString;

        public WeatherSqlDao(string connString)
        {
            this.connectionString = connString;
        }
        public WeatherObject CreateWeather(int userId, WeatherObject weatherObject, int zipcode)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand command = conn.CreateCommand();

                command.CommandText = @"INSERT INTO weather (user_id, zipcode, main, description, temperature)
                                        VALUES (@user_id, @zipcode, @main, @description, @temperature)";
                command.Parameters.AddWithValue("@user_id", userId);
                command.Parameters.AddWithValue("@zipcode", zipcode);
                command.Parameters.AddWithValue("@main", weatherObject.weather[0].main);
                command.Parameters.AddWithValue("@description", weatherObject.weather[0].description);
                command.Parameters.AddWithValue("@temperature", weatherObject.main.temp);

                command.ExecuteNonQuery();
            }

            return GetWeatherByUserId(userId);
        }

        public WeatherObject GetWeatherByUserId(int userId)
        {
            WeatherObject weatherObject = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand command = conn.CreateCommand();

                command.CommandText = "SELECT user_id, zipcode, main, description, temperature \r\nFROM weather \r\nWHERE user_id = @user_id";
                command.Parameters.AddWithValue("@user_id", userId);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    weatherObject = MapRowToWeatherObject(reader);
                }
            }

            return weatherObject;
        }

        public WeatherObject MapRowToWeatherObject(SqlDataReader reader)
        {
            WeatherObject temp = new WeatherObject();
            temp.main = new Main();
            Weather weather = new Weather();
            Main mainObj = new Main();
            temp.weather = new List<Weather>();

            string main = Convert.ToString(reader["main"]);
            weather.main = main;
            string description = Convert.ToString(reader["description"]);
            float temperature = (float)Convert.ToDouble(reader["temperature"]);
            weather.description = description;
            temp.main.temp = temperature;
            temp.weather.Add(weather);
            return temp;
        }
    }
}
