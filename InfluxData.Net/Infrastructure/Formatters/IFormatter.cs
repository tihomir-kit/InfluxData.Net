using InfluxData.Net.Models;

namespace InfluxData.Net.Infrastructure.Formatters
{
    public interface IFormatter
    {
        string GetLineTemplate();

        string PointToString(Point point);

        Serie PointToSerie(Point point);
    }
}