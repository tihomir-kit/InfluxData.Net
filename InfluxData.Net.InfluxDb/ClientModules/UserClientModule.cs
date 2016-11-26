using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb.Constants;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.QueryBuilders;
using InfluxData.Net.InfluxDb.RequestClients;
using InfluxData.Net.InfluxDb.ResponseParsers;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class UserClientModule : ClientModuleBase, IUserClientModule
    {
        private readonly IUserQueryBuilder _userQueryBuilder;
        private readonly IUserResponseParser _userResponseParser;

        public UserClientModule(IInfluxDbRequestClient requestClient, IUserQueryBuilder userQueryBuilder, IUserResponseParser userResponseParser)
            : base(requestClient)
        {
            _userQueryBuilder = userQueryBuilder;
            _userResponseParser = userResponseParser;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var query = _userQueryBuilder.GetUsers();
            var series = await base.ResolveSingleGetSeriesResultAsync(query).ConfigureAwait(false);
            var users = _userResponseParser.GetUsers(series);
            
            return users;
        }

        public async Task<IEnumerable<Grant>> GetPrivilegesAsync(string username)
        {
            var query = _userQueryBuilder.GetPrivileges(username);
            var series = await base.ResolveSingleGetSeriesResultAsync(query).ConfigureAwait(false);
            var grants = _userResponseParser.GetPrivileges(series);

            return grants;
        }
    }
}
