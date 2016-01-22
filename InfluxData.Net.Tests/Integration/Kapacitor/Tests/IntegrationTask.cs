using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using System.Threading.Tasks;
using InfluxData.Net.Common.Enums;
using Xunit;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Enums;
using InfluxData.Net.Kapacitor.Models;

namespace InfluxData.Net.Integration.Kapacitor.Tests
{
    [Collection("Kapacitor Integration")]
    [Trait("Kapacitor Integration", "Task")]
    public class IntegrationTask : IDisposable
    {
        private readonly IntegrationFixture _fixture;

        public IntegrationTask(IntegrationFixture fixture)
        {
            _fixture = fixture;
            _fixture.TestSetup();
        }

        public void Dispose()
        {
            _fixture.TestTearDown();
        }

        [Fact]
        public async Task Define_OnValidArguments_ShouldDefineSuccessfully()
        {
            var influxDbClient = new InfluxDbClient(
                ConfigurationManager.AppSettings.Get("influxDbEndpointUri"),
                ConfigurationManager.AppSettings.Get("influxDbUsername"),
                ConfigurationManager.AppSettings.Get("influxDbPassword"),
                InfluxDbVersion.Latest);

            var dbName = "KapacitorDb";
            var dropDbResponse = await influxDbClient.Database.DropDatabaseAsync(dbName);
            dropDbResponse.Success.Should().BeTrue();
            var createDbResponse = await influxDbClient.Database.CreateDatabaseAsync(dbName);
            createDbResponse.Success.Should().BeTrue();

            var defineTaskParams = new DefineTaskParams()
            {
                TaskName = "IntegrationTestTask",
                TaskType = TaskType.Stream,
                DbrpsParams = new DbrpsParams()
                {
                    DbName = dbName,
                    RetentionPolicy = "default"
                },
                TickScript = "stream\r\n" +
                             "    .from().measurement('reading')\r\n" +
                             "    .alert()\r\n" +
                             "        .crit(lambda: \"Humidity\" < 36)\r\n" +
                             "        .log('/tmp/alerts.log')\r\n"
            };

            var response = await _fixture.Sut.Task.DefineTask(defineTaskParams);
            response.Success.Should().BeTrue();
        }
    }
}
