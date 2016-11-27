using System.Collections.Generic;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.Common.Enums;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public interface IBatchHandler
    {
        void Start();

        void AddPoints(IEnumerable<Point> points);

        void Stop();
    }

    internal interface IBatchHandlerFactory : IBatchHandler
    {
        IBatchHandler CreateBatchHandler(string dbName, string retenionPolicy = "default", TimeUnit precision = TimeUnit.Milliseconds);
    }
}