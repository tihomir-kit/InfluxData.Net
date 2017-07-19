using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluxData.Net.InfluxDb.Models.Responses
{
    public class User
    {
        /// <summary>
        /// User name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Whether or not the user is an administrator.
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
