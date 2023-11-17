using System.Collections.Generic;
using WeatherDB.Cli.Models;

namespace WeatherDB.Cli.DAO
{
    public interface IUserDao
    {
        /// <summary>Get all the users from the database.</summary>
        /// <returns>A List of user objects.</returns>
        IList<User> GetUsers();

        /// <summary>
        /// Retrieve the password and salt for a user with the given username.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>Dictionary of password and salt.</returns>
        Dictionary<string, string> GetPasswordAndSaltByUsername(string username);

        /// <summary>
        /// Retrieve the user with the given username.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>The retrieved user.</returns>
        User GetUserByUsername(string username);

        /// <summary>
        /// Save a new user to the database. The original password isn't passed in,
        /// it's salted and hashed before being passed. The original password is never
        /// stored in the system.
        /// </summary>
        /// <param name="username">The username to give the new user.</param>
        /// <param name="hashedPassword">The user's hashed password.</param>
        /// <param name="saltString">The salt of the user's hashed password.</param>
        /// <returns>The new user.</returns>
        User CreateUser(string username, string hashedPassword, string saltString);
    }
}
