using InfluxData.Net.Common.Constants;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.Formatters
{
    public interface IPointFormatter
    {
        string PointToString(Point point, string precision = TimeUnit.Milliseconds);

        Serie PointToSerie(Point point);
    }
}