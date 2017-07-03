using System.Collections.Generic;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.ResponseParsers
{
    public interface IUserResponseParser
    {
        IEnumerable<User> GetUsers(IEnumerable<Serie> series);

        IEnumerable<Grant> GetPrivileges(IEnumerable<Serie> series);
    }
}
