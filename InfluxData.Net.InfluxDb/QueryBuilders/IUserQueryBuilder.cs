using InfluxData.Net.InfluxDb.Enums;

namespace InfluxData.Net.InfluxDb.QueryBuilders
{
    public interface IUserQueryBuilder
    {
        /// <summary>
        /// Returns a list of all users.
        /// </summary>
        string GetUsers();

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

        /// <summary>
        /// Gets a list of database privilege grants for the given user.
        /// </summary>
        /// <param name="username">The user to get grants for.</param>
        string GetPrivileges(string username);

        /// <summary>
        /// Grants a user administrator privileges.
        /// </summary>
        /// <param name="username">The name of the user to grant privileges to.</param>
        string GrantAdministator(string username);

        /// <summary>
        /// Revokes administrator privileges from a user.
        /// </summary>
        /// <param name="username">The name of the user to revoke privileges from.</param>
        string RevokeAdministrator(string username);

        /// <summary>
        /// Grants a user a specific privilege on a specific database.
        /// </summary>
        /// <param name="username">The name of the user.</param>
        /// <param name="privilege">The privilege to grant.</param>
        /// <param name="dbName">The name of the database to grant privileges on.</param>
        string GrantPrivilege(string username, Privileges privilege, string dbName);

        /// <summary>
        /// Revokes a specific privilege from a user on a specific database.
        /// </summary>
        /// <param name="username">The name of the user.</param>
        /// <param name="privilege">The privilege to revoke.</param>
        /// <param name="dbName">The name of the database to revoke privileges from.</param>
        string RevokePrivilege(string username, Privileges privilege, string dbName);
    }
}
