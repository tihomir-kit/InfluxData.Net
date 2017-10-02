using System;
using System.Threading.Tasks;
using FluentAssertions;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.Tests.Common.AppSettings;
using Moq;
using System.Diagnostics;

namespace InfluxData.Net.Integration.Kapacitor
{
    public abstract class IntegrationFixtureFactory : IDisposable
    {
        public string _fakeDbPrefix;

        private static readonly Random _random = new Random();
        private static readonly object _syncLock = new object();
        private MockRepository _mockRepository;

        public IInfluxDbClient InfluxDbClient { get; set; }

        public string DbName { get; set; }

        public bool VerifyAll { get; set; }

        protected IntegrationFixtureFactory(string fakeDbPrefix, string influxDbEndpointUriKey, InfluxDbVersion influxDbVersion, bool throwOnWarning)
        {
            Debug.WriteLine(influxDbEndpointUriKey);
            _fakeDbPrefix = fakeDbPrefix;

            this.DbName = CreateRandomDbName();

            this.InfluxDbClient = new InfluxDbClient(
                ConfigurationManager.Get(influxDbEndpointUriKey),
                ConfigurationManager.Get("InfluxSettings:InfluxDbUsername"),
                ConfigurationManager.Get("InfluxSettings:InfluxDbPassword"),
                influxDbVersion, 
                throwOnWarning: throwOnWarning);

            Task.Run(() => this.PurgeFakeDatabases()).Wait();
            Task.Run(() => this.CreateEmptyDatabase()).Wait();
        }

        public void Dispose()
        {
            Task.Run(() => this.DropDatabase(this.DbName)).Wait();
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

        public async Task CreateEmptyDatabase(string dbName = null)
        {
            var createDbResponse = await this.InfluxDbClient.Database.CreateDatabaseAsync(dbName ?? this.DbName);
            createDbResponse.Success.Should().BeTrue();
        }

        public async Task DropDatabase(string dbName)
        {
            var deleteResponse = this.InfluxDbClient.Database.DropDatabaseAsync(dbName).Result;
            deleteResponse.Success.Should().BeTrue();
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

        public string CreateRandomDbName()
        {
            return CreateRandomName(_fakeDbPrefix);
        }

        public string CreateRandomName(string prefix)
        {
            return String.Format("{0}{1}", prefix, CreateRandomSuffix());
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
    }
}