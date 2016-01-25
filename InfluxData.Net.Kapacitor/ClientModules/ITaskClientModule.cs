using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.Kapacitor.Models;
using InfluxData.Net.Kapacitor.Models.Responses;

namespace InfluxData.Net.Kapacitor.ClientModules
{
    public interface ITaskClientModule
    {
        Task<KapacitorTask> GetTaskAsync(string taskName);

        Task<IEnumerable<KapacitorTask>> GetTasksAsync();

        Task<IInfluxDataApiResponse> DefineTaskAsync(DefineTaskParams taskParams);

        Task<IInfluxDataApiResponse> DeleteTaskAsync(string taskName);

        Task<IInfluxDataApiResponse> EnableTaskAsync(string taskName);

        Task<IInfluxDataApiResponse> DisableTaskAsync(string taskName);
    }
}