using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxData.Helpers;
using InfluxData.Net.InfluxDb.Helpers;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.RequestClients;
using InfluxData.Net.InfluxDb.ResponseParsers;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class BatchClientModule : IBatchClientModule
    {
        private IBatchHandler _batchHandler;

        public BatchClientModule(IBatchHandler batchHandler)
            : base()
        {
            _batchHandler = batchHandler;
        }

        public IBatchHandler CreateBatchHandler(string dbName, string retenionPolicy = "default", TimeUnit precision = TimeUnit.Milliseconds)
        {
            return ((IBatchHandlerFactory)_batchHandler).CreateBatchHandler(dbName, retenionPolicy, precision);
        }
    }
}
