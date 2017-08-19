using System.Threading.Tasks;
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