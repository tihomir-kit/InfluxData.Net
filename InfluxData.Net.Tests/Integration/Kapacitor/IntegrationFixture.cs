using FluentAssertions;
using Moq;
using Ploeh.AutoFixture;
using System;
using System.Configuration;
using System.Threading.Tasks;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Enums;
using InfluxData.Net.Kapacitor;
using InfluxData.Net.Kapacitor.Models;

namespace InfluxData.Net.Integration.Kapacitor
{
    public class IntegrationFixture : IDisposable
    {
        public static readonly string _fakeDbPrefix = "FakeKapacitorDb";
        public static readonly string _fakeTaskPrefix = "FakeTask";

        private static readonly Random _random = new Random();
        private static readonly object _syncLock = new object();
        private MockRepository _mockRepository;

        public IKapacitorClient Sut { get; set; }
        public IInfluxDbClient InfluxDbClient { get; set; }

        public string DbName { get; set; }

        public bool VerifyAll { get; set; }

        public IntegrationFixture()
        {
            KapacitorVersion kapacitorVersion;
            if (!Enum.TryParse(ConfigurationManager.AppSettings.Get("version"), out kapacitorVersion))
                kapacitorVersion = KapacitorVersion.v_0_2_4;

            this.Sut = new KapacitorClient(
                ConfigurationManager.AppSettings.Get("kapacitorEndpointUri"),
                kapacitorVersion);

            this.Sut.Should().NotBeNull();

            this.DbName = CreateRandomDbName();

            this.InfluxDbClient = new InfluxDbClient(
                ConfigurationManager.AppSettings.Get("influxDbEndpointUri"),
                ConfigurationManager.AppSettings.Get("influxDbUsername"),
                ConfigurationManager.AppSettings.Get("influxDbPassword"),
                InfluxDbVersion.Latest);

            Task.Run(() => this.PurgeFakeDatabases()).Wait();
            Task.Run(() => this.CreateEmptyDatabase()).Wait();
        }

        public void Dispose()
        {
            var deleteResponse = this.InfluxDbClient.Database.DropDatabaseAsync(this.DbName).Result;

            deleteResponse.Success.Should().BeTrue();
        }

        // Per-test
        public void TestSetup()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            VerifyAll = true;
        }

        // Per-test
        public void TestTearDown()
        {
            if (VerifyAll)
            {
                _mockRepository.VerifyAll();
            }
            else
            {
                _mockRepository.Verify();
            }
        }

        public string CreateRandomDbName()
        {
            return String.Format("{0}{1}", _fakeDbPrefix, CreateRandomSuffix());
        }

        public string CreateRandomTaskName()
        {
            return String.Format("{0}{1}", _fakeTaskPrefix, CreateRandomSuffix());
        }

        /// <see cref="http://stackoverflow.com/a/768001/413785"/>
        public static string CreateRandomSuffix()
        {
            var timestamp = DateTime.UtcNow.ToUnixTime();
            lock (_syncLock)
            {
                var randomInt = _random.Next(Int32.MaxValue);
                return String.Format("{0}{1}", timestamp, randomInt);
            }
        }

        public async Task CreateEmptyDatabase()
        {
            var createDbResponse = await this.InfluxDbClient.Database.CreateDatabaseAsync(this.DbName);
            createDbResponse.Success.Should().BeTrue();
        }

        private async Task PurgeFakeDatabases()
        {
            var dbs = await this.InfluxDbClient.Database.GetDatabasesAsync();

            foreach (var db in dbs)
            {
                if (db.Name.StartsWith(_fakeDbPrefix))
                    await this.InfluxDbClient.Database.DropDatabaseAsync(db.Name);
            }
        }

        public async Task<DefineTaskParams> MockAndPostTask()
        {
            var task = MockDefineTaskParams();

            var defineResponse = await this.Sut.Task.DefineTask(task);
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
    }
}