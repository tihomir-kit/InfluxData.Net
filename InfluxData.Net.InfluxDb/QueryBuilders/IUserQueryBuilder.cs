using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluxData.Net.InfluxDb.QueryBuilders
{
    public interface IUserQueryBuilder
    {
        /// <summary>
        /// Returns a list of all users.
        /// </summary>
        string GetUsers();

        /// <summary>
        /// Gets a list of database privilege grants for the given user.
        /// </summary>
        /// <param name="username">The user to get grants for.</param>
        string GetPrivileges(string username);

        /// <summary>
        /// Creates a user with the given username and privilege.
        /// </summary>
        /// <param name="username">The user's name.</param>
        /// <param name="password">The user's password.</param>
        /// <param name="isAdmin">Whether or not the user is a database administrator.</param>
        string CreateUser(string username, string password, bool isAdmin);

        /// <summary>
        /// Drops an existing user.
        /// </summary>
        /// <param name="username">The user's name.</param>
        string DropUser(string username);

        /// <summary>
        /// Sets a user's password.
        /// </summary>
        /// <param name="username">The user's name.</param>
        /// <param name="password">The password to set.</param>
        string SetPassword(string username, string password);
    }
}
