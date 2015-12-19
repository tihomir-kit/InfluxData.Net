namespace InfluxData.Net.InfluxDb.Constants
{
    internal static class QueryStatements
    {
        // NOTE: formatted time for WHERE clause needs to go in single quotes whereas downsampled serie name needs to go into double quotes

        internal const string CreateDatabase = "CREATE DATABASE \"{0}\"";
        internal const string GetDatabases = "SHOW DATABASES";
        internal const string DropDatabase = "DROP DATABASE \"{0}\"";
        internal const string DropSeries = "DROP SERIES FROM \"{0}\"";

        internal const string AlterRetentionPolicy = "ALTER RETENTION POLICY {0} ON {1} DURATION {2} REPLICATION {3}";

        internal const string CreateContinuousQuery = "CREATE CONTINUOUS QUERY {0} ON {1} BEGIN {2} END;";
        internal const string CreateContinuousQuerySubQuery = "SELECT {0} INTO \"{1}\" FROM {2} GROUP BY time({3}) {4} {5}";
        internal const string GetContinuousQueries = "SHOW CONTINUOUS QUERIES";
        internal const string DropContinuousQuery = "DROP CONTINUOUS QUERY {0} ON {1}";
        internal const string Backfill = "SELECT {0} INTO \"{1}\" FROM {2} WHERE {3} time >= '{4}' AND time < '{5}' GROUP BY time({6}) {7} {8}";
        internal const string Fill = "fill({0})";

        

        //internal const string CreateRetentionPolicy = "CREATE RETENTION POLICY \"{0}\" ON {1} {2} {3} {4} {5}";
        //internal const string CreateUser = "CREATE USER {0} WITH PASSWORD {1} {2}";
        //internal const string DropMeasurement = "DROP MEASUREMENT \"{0}\"";
        //internal const string DropRetentionPolicy = "DROP RETENTION POLICY \"{0}\" ON {1}";
        //internal const string DropUser = "DROP USER {0}";
        //internal const string Grant = "GRANT {0} ON {1} to {2}";
        //internal const string GrantAll = "GRANT ALL TO {0}";
        //internal const string Revoke = "REVOKE {0} ON {1} FROM {2}";
        //internal const string RevokeAll = "REVOKE ALL PRIVILAGES FROM {0}";
        //internal const string ShowFieldKeys = "SHOW FIELD KEYS {0} {1}";
        //internal const string ShowMeasurements = "SHOW MEASUREMENTS";
        //internal const string ShowRetentionPolicies = "SHOW RETENTION POLICIES {0}";
        //internal const string ShowSeries = "SHOW SERIES";
        //internal const string ShowTagKeys = "SHOW TAG KEYS";
        //internal const string ShowTagValues = "SHOW TAG VALUES";
        //internal const string ShowUsers = "SHOW USERS";
    }
}