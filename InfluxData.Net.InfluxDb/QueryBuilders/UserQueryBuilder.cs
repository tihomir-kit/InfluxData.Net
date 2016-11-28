using InfluxData.Net.InfluxDb.Constants;
using InfluxData.Net.InfluxDb.Enums;

namespace InfluxData.Net.InfluxDb.QueryBuilders
{
    public class UserQueryBuilder : IUserQueryBuilder
    {
        public virtual string GetUsers()
        {
            return QueryStatements.GetUsers;
        }

        public virtual string CreateUser(string username, string password, bool isAdmin)
        {
            return string.Format(QueryStatements.CreateUser, username, password, isAdmin ? " WITH ALL PRIVILEGES" : null);
        }

        public virtual string DropUser(string username)
        {
            return string.Format(QueryStatements.DropUser, username);
        }

        public virtual string SetPassword(string username, string password)
        {
            return string.Format(QueryStatements.SetPassword, username, password);
        }

        public virtual string GetPrivileges(string username)
        {
            return string.Format(QueryStatements.GetGrants, username);
        }

        public virtual string GrantAdministator(string username)
        {
            return string.Format(QueryStatements.GrantAdministrator, username);
        }

        public virtual string RevokeAdministrator(string username)
        {
            return string.Format(QueryStatements.RevokeAdministrator, username);
        }

        public virtual string GrantPrivilege(string username, Privileges privilege, string dbName)
        {
            return string.Format(QueryStatements.GrantPrivilege, privilege.ToString().ToUpper(), dbName, username);
        }

        public virtual string RevokePrivilege(string username, Privileges privilege, string dbName)
        {
            return string.Format(QueryStatements.RevokePrivilege, privilege.ToString().ToUpper(), dbName, username);
        }
    }
}
