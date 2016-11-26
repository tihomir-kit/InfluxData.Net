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
    }
}
