using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.Formatters
{
    public interface IPointFormatter
    {
        string GetLineTemplate();

        string PointToString(Point point);

        Serie PointToSerie(Point point);
    }
}