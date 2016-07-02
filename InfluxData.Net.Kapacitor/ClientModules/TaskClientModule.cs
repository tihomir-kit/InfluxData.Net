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

        public virtual async Task<KapacitorTask> GetTaskAsync(string taskName)
        {
            var requestParams = new Dictionary<string, string>
            {
                { QueryParams.Name, HttpUtility.UrlEncode(taskName) }
            };
            var response = await base.RequestClient.GetAsync(RequestPaths.Task, requestParams).ConfigureAwait(false);
            var task = response.ReadAs<KapacitorTask>();

            return task;
        }

        public virtual async Task<IEnumerable<KapacitorTask>> GetTasksAsync()
        {
            var requestParams = new Dictionary<string, string>
            {
                { QueryParams.Tasks, String.Empty }
            };
            var response = await base.RequestClient.GetAsync(RequestPaths.Tasks, requestParams).ConfigureAwait(false);
            var tasks = response.ReadAs<KapacitorTasks>();

            return tasks.Tasks;
        }

        public virtual async Task<IInfluxDataApiResponse> DefineTaskAsync(DefineTaskParams taskParams)
        {
            var content = JsonConvert.SerializeObject(new Dictionary <string, object>
            {
                { QueryParams.Id, taskParams.TaskId },
                { QueryParams.Type, taskParams.TaskType.ToString().ToLower() },
                { QueryParams.Dbrps, new List<IDictionary<string, string>>
                {
                    new Dictionary<string, string>()
                    {
                        { QueryParams.Db, taskParams.DBRPsParams.DbName },
                        { QueryParams.RetentionPolicy, taskParams.DBRPsParams.RetentionPolicy }
                    }
                }},
                { QueryParams.Script, taskParams.TickScript }
            });

            return await base.RequestClient.PostAsync(RequestPaths.Tasks, content: content).ConfigureAwait(false);
        }

        protected virtual string SerializeContent(IDictionary<string, string> properties)
        {
            string serializedProperties = String.Empty;

            foreach (var property in properties)
            {
                serializedProperties += String.Format("\"{0}\": \"{1}\",", property.Key, property.Value);
            }

            return String.Format("{{{0}}}", serializedProperties);
        }

        public virtual async Task<IInfluxDataApiResponse> DeleteTaskAsync(string taskId)
        {
            return await base.RequestClient.DeleteAsync(RequestPaths.Tasks, HttpUtility.UrlEncode(taskId)).ConfigureAwait(false);
        }

        public virtual async Task<IInfluxDataApiResponse> EnableTaskAsync(string taskId)
        {
            var requestParams = new Dictionary<string, string>
            {
                { QueryParams.Name, HttpUtility.UrlEncode(taskId) }
            };

            return await base.RequestClient.PostAsync(RequestPaths.Enable, requestParams, String.Empty).ConfigureAwait(false);
        }

        public virtual async Task<IInfluxDataApiResponse> DisableTaskAsync(string taskId)
        {
            var requestParams = new Dictionary<string, string>
            {
                { QueryParams.Name, HttpUtility.UrlEncode(taskId) }
            };

            return await base.RequestClient.PostAsync(RequestPaths.Disable, requestParams, String.Empty).ConfigureAwait(false);
        }
    }
}
