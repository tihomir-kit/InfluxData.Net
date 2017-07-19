using System.Collections.Generic;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.ResponseParsers
﻿{
    public interface IRetentionResponseParser
    {
        IEnumerable<RetentionPolicy> GetRetentionPolicies(string dbName, IEnumerable<Serie> series);
    }
}
