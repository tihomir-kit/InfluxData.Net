using InfluxData.Net.InfluxDb.Enums;

namespace InfluxData.Net.Kapacitor.Models
{
    /// <summary>
    /// Task definition object. Used for creating tasks in Kapacitor. Uses a tick script.
    /// </summary>
    public class DefineTaskParams : BaseTaskParams
    {
        /// <summary>
        /// Task type - stream, batch..
        /// </summary>
        public TaskType TaskType { get; set; }

        /// <summary>
        /// Tick script to save.
        /// </summary>
        public string TickScript { get; set; }
    }
}
