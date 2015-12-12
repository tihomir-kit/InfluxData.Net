using System;

namespace InfluxData.Net.Models.Responses
{
    public class Pong
    {
        public bool Success{ get; set; }

        public string Version { get; set; }

        public TimeSpan ResponseTime { get; set; }
    }
}