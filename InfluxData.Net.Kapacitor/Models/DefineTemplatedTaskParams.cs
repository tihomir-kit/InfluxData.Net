namespace InfluxData.Net.Kapacitor.Models
{
    /// <summary>
    /// Task definition object. Used for creating tasks in Kapacitor. Uses a task template.
    /// </summary>
    public class DefineTemplatedTaskParams : BaseTaskParams
    {
        /// <summary>
        /// The Id of the template to use for creating this task.
        /// </summary>
        public string TemplateId { get; set; }
    }
}