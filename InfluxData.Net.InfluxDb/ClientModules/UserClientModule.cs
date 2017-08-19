using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb.Enums;
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

        public async Task<IInfluxDataApiResponse> CreateUserAsync(string username, string password, bool isAdmin = false)
        {
            var query = _userQueryBuilder.CreateUser(username, password, isAdmin);
            var response = await base.PostAndValidateQueryAsync(query).ConfigureAwait(false);
            return response;
        }

        public async Task<IInfluxDataApiResponse> DropUserAsync(string username)
        {
            var query = _userQueryBuilder.DropUser(username);
            var response = await base.PostAndValidateQueryAsync(query).ConfigureAwait(false);
            return response;
        }

        public async Task<IInfluxDataApiResponse> SetPasswordAsync(string username, string password)
        {
            var query = _userQueryBuilder.SetPassword(username, password);
            var response = await base.PostAndValidateQueryAsync(query).ConfigureAwait(false);
            return response;
        }

        public async Task<IEnumerable<Grant>> GetPrivilegesAsync(string username)
        {
            var query = _userQueryBuilder.GetPrivileges(username);
            var series = await base.ResolveSingleGetSeriesResultAsync(query).ConfigureAwait(false);
            var grants = _userResponseParser.GetPrivileges(series);
            return grants;
        }

        public async Task<IInfluxDataApiResponse> GrantAdministratorAsync(string username)
        {
            var query = _userQueryBuilder.GrantAdministator(username);
            var response = await base.PostAndValidateQueryAsync(query).ConfigureAwait(false);
            return response;
        }

        public async Task<IInfluxDataApiResponse> RevokeAdministratorAsync(string username)
        {
            var query = _userQueryBuilder.RevokeAdministrator(username);
            var response = await base.PostAndValidateQueryAsync(query).ConfigureAwait(false);
            return response;
        }

        public async Task<IInfluxDataApiResponse> GrantPrivilegeAsync(string username, Privileges privilege, string dbName)
        {
            var query = _userQueryBuilder.GrantPrivilege(username, privilege, dbName);
            var response = await base.PostAndValidateQueryAsync(query).ConfigureAwait(false);
            return response;
        }

        public async Task<IInfluxDataApiResponse> RevokePrivilegeAsync(string username, Privileges privilege, string dbName)
        {
            var query = _userQueryBuilder.RevokePrivilege(username, privilege, dbName);
            var response = await base.PostAndValidateQueryAsync(query).ConfigureAwait(false);
            return response;
        }
    }
}
