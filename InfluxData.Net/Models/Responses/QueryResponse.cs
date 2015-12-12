namespace InfluxData.Net.Models.Responses
{
    public class QueryResponse
    {
        public string Error { get; set; }

        public SeriesResult[] SeriesResult { get; set; }
    }

    public class SeriesResult
    {
        public string Error { get; set; }

        public Serie[] Series { get; set; }
    }
}