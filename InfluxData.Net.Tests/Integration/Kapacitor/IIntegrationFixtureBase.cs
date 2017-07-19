using System;
using System.Threading.Tasks;
using FluentAssertions;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb.Enums;
using InfluxData.Net.Kapacitor;
using InfluxData.Net.Kapacitor.Models;

namespace InfluxData.Net.Integration.Kapacitor
{
    public interface IIntegrationFixture : IIntegrationFixtureFactory
    {
        IKapacitorClient Sut { get; set; }

        void Dispose();

        string CreateRandomTaskId();

        #region Data Mocks

        Task<DefineTaskParams> MockAndSaveTask();

        DefineTaskParams MockDefineTaskParams();

        DefineTemplatedTaskParams MockTemplateTaskParams();

        #endregion Data Mocks
    }
}