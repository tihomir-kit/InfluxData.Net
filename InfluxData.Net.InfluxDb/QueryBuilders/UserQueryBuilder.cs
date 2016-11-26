using InfluxData.Net.InfluxDb.Constants;

namespace InfluxData.Net.InfluxDb.QueryBuilders
{
    public class UserQueryBuilder : IUserQueryBuilder
    {
        public virtual string GetUsers()
        {
            return QueryStatements.GetUsers;
        }

        public virtual string GetPrivileges(string username)
        {
            return string.Format(QueryStatements.GetGrants, username);
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
    }
}
