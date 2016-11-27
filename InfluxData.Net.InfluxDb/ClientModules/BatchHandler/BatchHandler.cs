using System.Collections.Generic;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb.Models;
using System.Threading.Tasks;
using System.Collections;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class BatchHandler : IBatchHandlerFactory
    {
        private IBasicClientModule _basicClientModule;
        private string _dbName;
        private string _retentionPolicy;
        private TimeUnit _precision;

        /// <summary>
        /// Concurrent readings queue.
        /// <see cref="http://www.codethinked.com/blockingcollection-and-iproducerconsumercollection"/>
        /// </summary>
        public BlockingCollection<Point> PointCollection { get; set; }

        internal BatchHandler(IBasicClientModule basicClientModule)
        {
            _basicClientModule = basicClientModule;
        }

        internal BatchHandler(IBasicClientModule basicClientModule, string dbName, string retenionPolicy = "default", TimeUnit precision = TimeUnit.Milliseconds)
        {
            _basicClientModule = basicClientModule;
            _dbName = dbName;
            _retentionPolicy = retenionPolicy;
            _precision = precision;
        }

        public virtual IBatchHandler CreateBatchHandler(string dbName, string retenionPolicy = "default", TimeUnit precision = TimeUnit.Milliseconds)
        {
            return new BatchHandler(_basicClientModule, dbName, retenionPolicy, precision);
        }

        public virtual void Start()
        {
            this.RegisterReadingsQueueHandler();
        }

        public virtual void AddPoint(Point point)
        {

        }

        public virtual void AddPoints(IEnumerable<Point> points)
        {

        }

        public virtual void Stop()
        {

        }

        public async Task RegisterReadingsQueueHandler()
        {
            await TaskEx.Delay(1000);
            //HandleReadingsQueue();
            //RegisterReadingsQueueHandler();
        }

        //private async Task HandleReadingsQueue()
        //{
        //    var readingsCount = ReadingsCollection.Count;
        //    IList<ReadingMessageDTO> readings = new List<ReadingMessageDTO>();

        //    for (var i = 0; i < readingsCount; i++)
        //    {
        //        ReadingMessageDTO reading;
        //        var dequeueSuccess = ReadingsCollection.TryTake(out reading);

        //        if (dequeueSuccess)
        //        {
        //            readings.Add(reading);
        //        }
        //        else
        //        {
        //            throw new Exception("Could not dequeue the collection");
        //        }
        //    }

        //    if (readings.Count > 0)
        //    {
        //        await _influxDbProvider.SaveReadings(readings);
        //    }
        //}
    }
}
