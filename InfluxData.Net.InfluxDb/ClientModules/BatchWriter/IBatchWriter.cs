using System.Collections.Generic;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.Common.Enums;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public interface IBatchWriter
    {
        void Start();

        void AddPoint(Point point);

        void AddPoints(IEnumerable<Point> points);

        void Stop();
    }

    internal interface IBatchWriterFactory : IBatchWriter
    {
        IBatchWriter CreateBatchWriter(string dbName, int interval = 1000, string retenionPolicy = "default", TimeUnit precision = TimeUnit.Milliseconds);
    }
}