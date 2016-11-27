namespace InfluxData.Net.InfluxDb.Models.Responses
{
    /// <summary>
    /// Represents <see cref="{RetentionPolicy}"/>.
    /// </summary>
    public class RetentionPolicy
    {
        /// <summary>
        /// RP name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// RP duration.
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// RP shard group duration.
        /// </summary>
        public string ShardGroupDuration { get; set; }

        /// <summary>
        /// RP replication copies.
        /// </summary>
        public int ReplicationCopies { get; set; }

        /// <summary>
        /// RP replication copies.
        /// </summary>
        public bool Default { get; set; }
    }
}
