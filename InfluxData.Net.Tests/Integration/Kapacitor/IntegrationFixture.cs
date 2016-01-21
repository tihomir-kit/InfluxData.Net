using FluentAssertions;
using Moq;
using Ploeh.AutoFixture;
using System;
using System.Configuration;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.Kapacitor;

namespace InfluxData.Net.Integration.Kapacitor
{
    public class IntegrationFixture : IDisposable
    {
        private static readonly object _syncLock = new object();
        private MockRepository _mockRepository;

        public IKapacitorClient Sut { get; set; }

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
        }

        public void Dispose()
        {
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
    }
}