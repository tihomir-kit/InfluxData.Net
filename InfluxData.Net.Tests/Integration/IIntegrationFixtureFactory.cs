using System;
using System.Configuration;
using System.Threading.Tasks;
using FluentAssertions;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb;
using Moq;

namespace InfluxData.Net.Integration.Kapacitor
{
    public interface IIntegrationFixtureFactory : IDisposable
    {
        IInfluxDbClient InfluxDbClient { get; set; }

        string DbName { get; set; }

        bool VerifyAll { get; set; }

        void Dispose();

        // Per-test
        void TestSetup();

        // Per-test
        void TestTearDown();

        Task CreateEmptyDatabase(string dbName = null);

        Task DropDatabase(string dbName);

        string CreateRandomDbName();

        string CreateRandomName(string prefix);
    }
}