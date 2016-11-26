using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public interface IUserClientModule
    {
        /// <summary>
        /// Gets all available users.
        /// </summary>
        /// <returns>A collection of all users.</returns>
        Task<IEnumerable<User>> GetUsersAsync();

        /// <summary>
        /// Gets a list of granted database privileges for a user.
        /// </summary>
        /// <param name="username">The name of the user to get granted privilges for.</param>
        Task<IEnumerable<Grant>> GetPrivilegesAsync(string username);
    }
}
