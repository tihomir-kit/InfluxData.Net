namespace InfluxData.Net.InfluxDb.Formatters
{
    internal class InfluxDbFormatterV092 : InfluxDbFormatterV09x
    {
        protected override string ToInt(string result)
        {
            return result;
        }
    }
}