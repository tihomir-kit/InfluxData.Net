using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.Kapacitor.Models;
using InfluxData.Net.Kapacitor.Models.Responses;

namespace InfluxData.Net.Kapacitor.ClientModules
{
    public interface ITaskClientModule
    {
        Task<KapacitorTask> GetTaskAsync(string taskId);

        Task<IEnumerable<KapacitorTask>> GetTasksAsync();

        Task<IInfluxDataApiResponse> DefineTaskAsync(DefineTaskParams taskParams);

        Task<IInfluxDataApiResponse> DefineTaskAsync(TemplateTaskParams taskParams);

        Task<IInfluxDataApiResponse> DeleteTaskAsync(string taskId);

        Task<IInfluxDataApiResponse> EnableTaskAsync(string taskId);

        Task<IInfluxDataApiResponse> DisableTaskAsync(string taskId);
    }
}