using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb.Enums;
using InfluxData.Net.Kapacitor;
using InfluxData.Net.Kapacitor.Models;
using InfluxData.Net.Tests.Common.AppSettings;

namespace InfluxData.Net.Integration.Kapacitor
{
    public abstract class IntegrationFixtureBase : IntegrationFixtureFactory, IIntegrationFixture
    {
        public static readonly string _fakeTaskPrefix = "FakeTask";

        public IKapacitorClient Sut { get; set; }

        protected IntegrationFixtureBase(
            string influxDbEndpointUriKey,
            InfluxDbVersion influxDbVersion,
            string kapacitorEndpointUriKey,
            KapacitorVersion kapacitorVersion)
            : base("FakeKapacitorDb", influxDbEndpointUriKey, influxDbVersion, false)
        {
            this.Sut = new KapacitorClient(
                ConfigurationManager.Get(kapacitorEndpointUriKey),
                kapacitorVersion);

            this.Sut.Should().NotBeNull();
        }

        public void Dispose()
        {
            Task.Run(() => this.PurgeFakeTasks()).Wait();
        }

        public string CreateRandomTaskId()
        {
            return base.CreateRandomName(_fakeTaskPrefix);
        }

        private async Task PurgeFakeTasks()
        {
            var tasks = await this.Sut.Task.GetTasksAsync();

            foreach (var task in tasks)
            {
                if (task.Id.StartsWith(_fakeTaskPrefix))
                    await this.Sut.Task.DeleteTaskAsync(task.Id);
            }
        }

        #region Data Mocks

        public async Task<DefineTaskParams> MockAndSaveTask()
        {
            var task = MockDefineTaskParams();

            var defineResponse = await this.Sut.Task.DefineTaskAsync(task);
            defineResponse.Success.Should().BeTrue();

            return task;
        }

        public virtual DefineTaskParams MockDefineTaskParams()
        {
            return new DefineTaskParams()
            {
                TaskId = CreateRandomTaskId(),
                TaskType = TaskType.Stream,
                DBRPsParams = new DBRPsParams()
                {
                    DbName = this.DbName,
                    RetentionPolicy = "default"
                },
                TickScript = "stream\r\n" +
                             "    |from().measurement('reading')\r\n" +
                             "    |alert()\r\n" +
                             "        .crit(lambda: \"Humidity\" < 36)\r\n" +
                             "        .log('/tmp/alerts.log')\r\n"
            };
        }

        public virtual DefineTemplatedTaskParams MockTemplateTaskParams()
        {
            return new DefineTemplatedTaskParams()
            {
                TaskId = CreateRandomTaskId(),
                DBRPsParams = new DBRPsParams()
                {
                    DbName = this.DbName,
                    RetentionPolicy = "default"
                },
                TemplateId = "TestTemplate",
                TaskVars = new Dictionary<string, TaskVar>()
                {
                    {"measurement",new TaskVar() {Type = "string",Value = "testMeasurment"} }
                }
            };
        }

        #endregion Data Mocks
    }
}