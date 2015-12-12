namespace InfluxData.Net.Models.Responses
{
    public class QueryResponse
    {
        public string Error { get; set; }

        // NOTE: do not rename this property (used by convention to deserialize query response)
        public SeriesResult[] Results { get; set; }
    }

    public class SeriesResult
    {
        public string Error { get; set; }

        public Serie[] Series { get; set; }
    }
}