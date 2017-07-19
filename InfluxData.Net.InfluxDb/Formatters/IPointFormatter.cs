using InfluxData.Net.Common.Constants;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.Formatters
{
    public interface IPointFormatter
    {
        string GetLineTemplate();

        string PointToString(Point point, string precision = TimeUnit.Milliseconds);

        Serie PointToSerie(Point point);
    }
}