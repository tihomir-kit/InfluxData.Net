using System;
using System.Configuration;
using System.Threading.Tasks;
using FluentAssertions;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb.Enums;
using InfluxData.Net.Kapacitor;
using InfluxData.Net.Kapacitor.Models;

namespace InfluxData.Net.Integration.Kapacitor
{
    public class IntegrationFixture : IntegrationFixtureBase, IDisposable
    {
        public static readonly string _fakeTaskPrefix = "FakeTask";

        public IKapacitorClient Sut { get; set; }

        public IntegrationFixture() : base("FakeKapacitorDb")
        {
            KapacitorVersion kapacitorVersion;
            if (!Enum.TryParse(ConfigurationManager.AppSettings.Get("version"), out kapacitorVersion))
                kapacitorVersion = KapacitorVersion.v_0_10_1;

            this.Sut = new KapacitorClient(
                ConfigurationManager.AppSettings.Get("kapacitorEndpointUri"),
                kapacitorVersion);

            this.Sut.Should().NotBeNull();
        }

        public void Dispose()
        {
            Task.Run(() => this.PurgeFakeTasks()).Wait();
        }

        public string CreateRandomTaskName()
        {
            return base.CreateRandomName(_fakeTaskPrefix);
        }

        private async Task PurgeFakeTasks()
        {
            var tasks = await this.Sut.Task.GetTasksAsync();

            foreach (var task in tasks)
            {
                if (task.Name.StartsWith(_fakeTaskPrefix))
                    await this.Sut.Task.DeleteTaskAsync(task.Name);
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

        public DefineTaskParams MockDefineTaskParams()
        {
            return new DefineTaskParams()
            {
                TaskName = CreateRandomTaskName(),
                TaskType = TaskType.Stream,
                DBRPsParams = new DBRPsParams()
                {
                    DbName = this.DbName,
                    RetentionPolicy = "default"
                },
                TickScript = "stream\r\n" +
                             "    .from().measurement('reading')\r\n" +
                             "    .alert()\r\n" +
                             "        .crit(lambda: \"Humidity\" < 36)\r\n" +
                             "        .log('/tmp/alerts.log')\r\n"
            };
        }

        #endregion Data Mocks
    }
}