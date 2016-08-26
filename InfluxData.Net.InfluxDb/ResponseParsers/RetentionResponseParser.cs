using System;
using System.Collections.Generic;
using System.Linq;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.ResponseParsers
{
    internal class RetentionResponseParser : IRetentionResponseParser
    {
        public IEnumerable<RetentionPolicy> GetRetentionPolicies(string dbName, IEnumerable<Serie> series)
        {
            var rps = new List<RetentionPolicy>();

            if (series == null)
                return rps;

            var serie = series.FirstOrDefault();
            if (serie == null || serie.Values == null)
                return rps;

            var columns = serie.Columns.ToArray();
            var indexOfName = Array.IndexOf(columns, "name");
            var indexOfDuration = Array.IndexOf(columns, "duration");
            var indexOfShardGroupDuration = Array.IndexOf(columns, "shardGroupDuration");
            var indexOfReplica = Array.IndexOf(columns, "replicaN");
            var indexOfDefault = Array.IndexOf(columns, "default");

            rps.AddRange(serie.Values.Select(p => new RetentionPolicy()
            {
                Name = (string)p[indexOfName],
                Duration = (string)p[indexOfDuration],
                ShardGroupduration = (string)p[indexOfShardGroupDuration],
                ReplicationCopies = (int)(long)p[indexOfReplica],
                Default = (bool)p[indexOfDefault]
            }));

            return rps;
        }
    }
}
