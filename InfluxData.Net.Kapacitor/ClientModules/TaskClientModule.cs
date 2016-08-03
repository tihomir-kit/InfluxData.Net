using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxData.Helpers;
using InfluxData.Net.Kapacitor.Constants;
using InfluxData.Net.Kapacitor.Models;
using InfluxData.Net.Kapacitor.Models.Responses;
using InfluxData.Net.Kapacitor.RequestClients;
using Newtonsoft.Json;

namespace InfluxData.Net.Kapacitor.ClientModules
{
    public class TaskClientModule : ClientModuleBase, ITaskClientModule
    {
        public TaskClientModule(IKapacitorRequestClient requestClient)
            : base(requestClient)
        {
        }

        public virtual async Task<KapacitorTask> GetTaskAsync(string taskId)
        {
            var response = await base.RequestClient.GetAsync(RequestPaths.Tasks, HttpUtility.UrlEncode(taskId)).ConfigureAwait(false);
            var task = response.ReadAs<KapacitorTask>();

            return task;
        }

        public virtual async Task<IEnumerable<KapacitorTask>> GetTasksAsync()
        {
            var response = await base.RequestClient.GetAsync(RequestPaths.Tasks).ConfigureAwait(false);
            var tasks = response.ReadAs<KapacitorTasks>();

            return tasks.Tasks;
        }

        public virtual async Task<IInfluxDataApiResponse> DefineTaskAsync(BaseTaskParams taskParams)
        {
            var jsonDictionary = taskParams.ToJsonDictionary();

            var content = JsonConvert.SerializeObject(jsonDictionary);

            return await base.RequestClient.PostAsync(RequestPaths.Tasks, content: content).ConfigureAwait(false);
        }

        public virtual async Task<IInfluxDataApiResponse> DeleteTaskAsync(string taskId)
        {
            return await base.RequestClient.DeleteAsync(RequestPaths.Tasks, HttpUtility.UrlEncode(taskId)).ConfigureAwait(false);
        }

        public virtual async Task<IInfluxDataApiResponse> EnableTaskAsync(string taskId)
        {
            var content = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                { BodyParams.Status, "enabled" },
            });

            return await base.RequestClient.PatchAsync(RequestPaths.Tasks, HttpUtility.UrlEncode(taskId), content).ConfigureAwait(false);
        }

        public virtual async Task<IInfluxDataApiResponse> DisableTaskAsync(string taskId)
        {
            var content = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                { BodyParams.Status, "disabled" },
            });

            return await base.RequestClient.PatchAsync(RequestPaths.Tasks, HttpUtility.UrlEncode(taskId), content).ConfigureAwait(false);
        }
    }
}