namespace InfluxData.Net.InfluxDb.QueryBuilders
{
    public interface IDiagnosticsQueryBuilder
    {
        string GetStats();

        string GetDiagnostics();
    }
}