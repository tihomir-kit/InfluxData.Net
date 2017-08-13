using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace InfluxData.Net.Tests.Common.AppSettings
{
    public static class ConfigurationManager
    {
        private static IConfigurationRoot Configuration { get; set; }

        public static string Get(string key)
        {
            if (Configuration == null)
            {
                BuildConfiguration();   
            }

            return Configuration[key];
        }

        private static void BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }
    }
}
