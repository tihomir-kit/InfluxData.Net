using System.IO;
using Microsoft.Extensions.Configuration;

namespace InfluxData.Net.Tests.Infrastructure.AppSettings
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
