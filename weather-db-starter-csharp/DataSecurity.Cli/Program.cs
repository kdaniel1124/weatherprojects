using WeatherDB.Cli.DAO;
using WeatherDB.Cli.Security;
using Microsoft.Extensions.Configuration;
using WeatherDb.Cli.DAO;

namespace WeatherDB.Cli
{
    public class Program
    {
        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = config.GetConnectionString("WeatherDBConnection");

            IUserDao userDao = new UserSqlDao(connectionString);
            IWeatherDao weatherDao = new WeatherSqlDao(connectionString);
            IPasswordHasher passwordHasher = new PasswordHasher();

            WeatherDB application = new WeatherDB(userDao, passwordHasher, weatherDao);

            application.Run();
        }
    }
}
