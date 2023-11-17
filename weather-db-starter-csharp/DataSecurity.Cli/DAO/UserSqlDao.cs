using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using WeatherDB.Cli.Models;

namespace WeatherDB.Cli.DAO
{
    public class UserSqlDao : IUserDao
    {
        private readonly string connectionString;

        /// <summary>
        /// Create a new user DAO with the supplied data source.
        /// </summary>
        /// <param name="connString">Database connection string.</param>
        public UserSqlDao(string connString)
        {
            connectionString = connString;
        }

        public IList<User> GetUsers()
        {
            IList<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT id, username FROM users;";

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    User user = MapRowToUser(reader);
                    users.Add(user);
                }
            }

            return users;
        }

        public Dictionary<string, string> GetPasswordAndSaltByUsername(string username)
        {
            Dictionary<string, string> passwordAndSalt = new Dictionary<string, string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT password, salt FROM users WHERE username = @username;";
                command.Parameters.AddWithValue("@username", username);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    passwordAndSalt.Add("password", Convert.ToString(reader["password"]));
                    passwordAndSalt.Add("salt", Convert.ToString(reader["salt"]));
                }

                return passwordAndSalt;
            }
        }

        public User GetUserByUsername(string username)
        {
            User user = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT id, username FROM users WHERE username = @username;";
                command.Parameters.AddWithValue("@username", username);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    user = MapRowToUser(reader);
                }

                return user;
            }
        }

        public User CreateUser(string username, string hashedPassword, string saltString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO users (username, password, salt)
                                        VALUES (@username, @password, @salt);";

                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", hashedPassword);
                command.Parameters.AddWithValue("@salt", saltString);

                command.ExecuteNonQuery();

                User newUser = GetUserByUsername(username);

                return newUser;
            }
        }

        private User MapRowToUser(SqlDataReader reader)
        {
            User user = new User();
            user.Id = Convert.ToInt32(reader["id"]);
            user.Username = Convert.ToString(reader["username"]);
            return user;
        }
    }
}
