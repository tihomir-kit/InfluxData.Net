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
    }
}
