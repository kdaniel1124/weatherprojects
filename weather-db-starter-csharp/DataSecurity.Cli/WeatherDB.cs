using System;
using System.Collections.Generic;
using WeatherDb.Cli.DAO;
using WeatherDB.Cli.DAO;
using WeatherDB.Cli.Models;
using WeatherDB.Cli.Security;
using WeatherDB.Cli.Services;

namespace WeatherDB.Cli
{
    public class WeatherDB
    {
        private readonly IUserDao userDao;

        private readonly IWeatherDao weatherDao;

        private readonly IPasswordHasher passwordHasher;

        private readonly WeatherService service = new WeatherService();


        public WeatherDB(IUserDao userDao, IPasswordHasher passwordHasher, IWeatherDao weatherDao)
        {
            this.userDao = userDao;
            this.passwordHasher = passwordHasher;
            this.weatherDao = weatherDao;
        }

        private User LoggedInUser { get; set; }

        /// <summary>
        /// Add a new user to the system. Anyone can register a new account like
        /// this. It calls CreateUser() in the DAO to save it to the data store.
        /// </summary>
        private void AddNewUser()
        {
            Console.WriteLine("Enter the following information for a new user: ");
            Console.Write("Username: ");
            string username = Console.ReadLine();

            Console.Write("Password: ");
            string password = Console.ReadLine();

            // generate hashed password and salt
            byte[] salt = passwordHasher.GenerateRandomSalt();
            string hashedPassword = passwordHasher.ComputeHash(password, salt);
            string saltString = Convert.ToBase64String(salt);

            User user = userDao.CreateUser(username, hashedPassword, saltString);
            Console.WriteLine($"User {user.Username} added with id {user.Id}!");
            Console.WriteLine();
        }

        /// <summary>
        /// Take a username and password from the user and check it against
        /// the DAO via the IsUsernameAndPasswordValid() method.
        /// If the method returns false it means the username/password aren't valid.
        /// You don't know what's specifically wrong about the login, just that the combined
        /// username & password aren't valid. You don't want to give an attacker any information about
        /// what they got right or what they got wrong when trying this. Information
        /// like that is gold to an attacker because then they know what they're
        /// getting right and what they're getting wrong.
        /// </summary>
        private void LoginUser()
        {
            Console.WriteLine("Log into the system");
            Console.Write("Username: ");

            string username = Console.ReadLine();
            Console.Write("Password: ");

            string password = Console.ReadLine();

            if (IsUsernameAndPasswordValid(username, password))
            {
                LoggedInUser = new User();
                LoggedInUser.Username = username;
                User user = userDao.GetUserByUsername(username);
                LoggedInUser.Id = user.Id;
                Console.WriteLine($"Welcome {username}!");
            }
            else
            {
                Console.WriteLine("That log in is not valid, please try again.");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Check the username and password are valid.
        /// </summary>
        /// <param name="username">The supplied username to validate.</param>
        /// <param name="password">The supplied password to validate.</param>
        /// <returns></returns>
        private bool IsUsernameAndPasswordValid(string username, string password)
        {
            Dictionary<string, string> passwordAndSalt = userDao.GetPasswordAndSaltByUsername(username);
            if(passwordAndSalt.Count == 0)
            {
                return false;
            }
            string storedPassword = passwordAndSalt["password"];
            string storedSalt = passwordAndSalt["salt"];
            string computedHash = passwordHasher.ComputeHash(password, Convert.FromBase64String(storedSalt));

            return computedHash.Equals(storedPassword);
        }

        /// <summary>
        /// Show all the users that are in the data store. You can't show passwords
        /// because you don't have them! Passwords in the data store are hashed and
        /// you can see that by opening up a SQL client like SQL Server Management Studio
        /// and looking at what's stored in the `users` table.
        /// 
        /// Only allow access to this to logged-in users.If a user isn't logged
        /// in, give them a message and leave. Having an `if` statement like this
        /// at the top of the method is a common way of handling authorization checks.
        /// </summary>
        private void ShowUsers()
        {
            if (LoggedInUser == null)
            {
                Console.WriteLine("Sorry. Only logged in users can see other users.");
                Console.WriteLine("Hit enter to continue...");

                Console.Read();
                return;
            }

            IList<User> users = userDao.GetUsers();
            Console.WriteLine("Users currently in the system are: ");
            foreach (User user in users)
            {
                Console.WriteLine($"{user.Id}. {user.Username}");
            }

            Console.WriteLine();
            Console.WriteLine("Hit enter to continue...");
            Console.Read();
            Console.WriteLine();
        }

        private void PrintGreeting()
        {
            Console.WriteLine("Welcome to the User Manager Application!");
            Console.WriteLine();
        }

        private void PrintMenu()
        {
            Console.WriteLine("(A)dd a new User");
            Console.WriteLine("(S)how all users");
            Console.WriteLine("(L)og in");
            Console.WriteLine("(Q)uit");
            Console.WriteLine();
        }

        private void WeatherMenu()
        {
            Console.WriteLine("(A)dd a zipcode");
            Console.WriteLine("(S)how weather");
            Console.WriteLine("(Q)uit");
            Console.WriteLine();
        }

        private void ShowWeatherMenu()
        {
            int zipcode = 0;
            while (true)
            {
                WeatherMenu();
                string option = AskPrompt().ToLower();
                if (option == "a")
                {
                    zipcode = AddZipcode();
                }
                else if (option == "s")
                {
                    ShowWeather(zipcode);
                }
                else if (option == "q")
                {
                    Console.WriteLine("Thanks for checking the weather!");
                    break;
                }
                else
                {
                    Console.WriteLine($"{option} is not a valid option. Try again!");
                }
            }
        
        }

        private void ShowWeather(int zipcode)
        {
            if (zipcode == 0)
            {
                Console.WriteLine("Invalid zipcode or zipcode not entered!");
            } else
            {
                Console.WriteLine("user is " + LoggedInUser.Username + " and zipcode is" + zipcode);
                LatLon latlon = service.GetLatLon(zipcode.ToString());

                WeatherObject weather = service.getWeather(latlon);

                WeatherObject weatherFromDB = weatherDao.CreateWeather(LoggedInUser.Id, weather, zipcode);

                Console.WriteLine(weather.weather[0].description);
                Console.WriteLine(weather.weather[0].main);
                Console.WriteLine(weather.main.temp);

            }
        }

        private int AddZipcode()
        {
            bool isValid = false;
            int zipcode = 0;

            while (!isValid)
            {
                Console.Write("Enter your zipcode: ");
                string zipString = Console.ReadLine();
                try
                {
                    zipcode = int.Parse(zipString);
                    isValid = true;
                } catch (FormatException e)
                {
                    Console.WriteLine("Invalid entry - please try again");
                }
            }
            return zipcode;
        }

        /// <summary>
        /// The main run loop.
        /// </summary>
        public void Run()
        {
            PrintGreeting();

            while (true)
            {
                PrintMenu();
                string option = AskPrompt().ToLower();

                if (option == "a")
                {
                    AddNewUser();
                }
                else if (option == "s")
                {
                    ShowUsers();
                }
                else if (option == "l")
                {
                    LoginUser();
                    if (LoggedInUser != null)
                    {
                        ShowWeatherMenu();
                    }
                }
                else if (option == "q")
                {
                    Console.WriteLine("Thanks for using the Weather App!");
                    break;
                }
                else
                {
                    Console.WriteLine($"{option} is not a valid option. Try again!");
                }
            }
        }

        private string AskPrompt()
        {
            // Get the username for the logged in user unless LoggedInUser
            // is null, or the Username is null
            string name = LoggedInUser?.Username ??
                "Unauthenticated User";

            Console.WriteLine($"Welcome {name}!");
            Console.Write("What would you like to do today? ");

            return Console.ReadLine();
        }
    }
}
