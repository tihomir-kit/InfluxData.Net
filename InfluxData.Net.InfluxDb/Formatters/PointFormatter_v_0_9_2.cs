namespace InfluxData.Net.InfluxDb.Formatters
{
    public class PointFormatter_v_0_9_2 : PointFormatter
    {
        protected override string ToInt(string result)
        {
            return result;
        }
    }
}