namespace InfluxData.Net.InfluxDb.Formatters
{
    public class PointFormatter_v_0_9_2 : PointFormatter_v_1_0_0
    {
        protected override string ToInt(string result)
        {
            return result;
        }
    }
}