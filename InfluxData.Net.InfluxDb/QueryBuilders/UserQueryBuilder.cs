using InfluxData.Net.InfluxDb.Constants;
using InfluxData.Net.InfluxDb.Enums;
using System;

namespace InfluxData.Net.InfluxDb.QueryBuilders
{
    public class UserQueryBuilder : IUserQueryBuilder
    {
        public virtual string GetUsers()
        {
            return QueryStatements.GetUsers;
        }

        public virtual string CreateUser(string username, string password, bool isAdmin = false)
        {
            return String.Format(QueryStatements.CreateUser, username, password, isAdmin ? QueryStatements.WithAllPrivileges : null);
        }

        public virtual string DropUser(string username)
        {
            return String.Format(QueryStatements.DropUser, username);
        }

        public virtual string SetPassword(string username, string password)
        {
            return String.Format(QueryStatements.SetPassword, username, password);
        }

        public virtual string GetPrivileges(string username)
        {
            return String.Format(QueryStatements.GetGrants, username);
        }

        public virtual string GrantAdministator(string username)
        {
            return String.Format(QueryStatements.GrantAdministrator, username);
        }

        public virtual string RevokeAdministrator(string username)
        {
            return String.Format(QueryStatements.RevokeAdministrator, username);
        }

        public virtual string GrantPrivilege(string username, Privileges privilege, string dbName)
        {
            return String.Format(QueryStatements.GrantPrivilege, privilege.ToString().ToUpper(), dbName, username);
        }

        public virtual string RevokePrivilege(string username, Privileges privilege, string dbName)
        {
            return String.Format(QueryStatements.RevokePrivilege, privilege.ToString().ToUpper(), dbName, username);
        }
    }
}
