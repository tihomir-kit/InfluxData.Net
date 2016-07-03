using InfluxData.Net.InfluxDb.QueryBuilders;
using InfluxData.Net.InfluxDb.RequestClients;
using InfluxData.Net.InfluxDb.ResponseParsers;

namespace InfluxData.Net.InfluxDb.Infrastructure
{
    /// <summary>
    /// Container class which holds resolved InfluxDbClient dependencies based on 'InfluxDbVersion'.
    /// </summary>
    internal class InfluxDbClientDependencies
    {
        public IInfluxDbRequestClient RequestClient { get; set; }

        public ICqQueryBuilder CqQueryBuilder { get; set; }

        public ISerieResponseParser SerieResponseParser { get; set; }
    }
}
