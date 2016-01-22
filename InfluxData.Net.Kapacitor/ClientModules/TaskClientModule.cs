using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxData.Net.Kapacitor.Infrastructure;
using InfluxData.Net.Kapacitor.RequestClients;
using System;
using System.Net.Http;
using System.Text;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb.Enums;
using InfluxData.Net.Kapacitor.Constants;
using InfluxData.Net.Kapacitor.Models;

namespace InfluxData.Net.Kapacitor.ClientModules
{
    public class TaskClientModule : ClientModuleBase, ITaskClientModule
    {
        public TaskClientModule(IKapacitorRequestClient requestClient)
            : base(requestClient)
        {
        }

        public virtual async Task<IInfluxDataApiResponse> DefineTask(DefineTaskParams taskParams)
        {
            var dbrps = String.Format("[{{\"{0}\":\"{1}\", \"{2}\":\"{3}\"}}]", 
                QueryParams.Db, taskParams.DbrpsParams.DbName, QueryParams.RetentionPolicy, taskParams.DbrpsParams.RetentionPolicy);
            var requestParams  = new Dictionary<string, string>
            {
                { QueryParams.Name, HttpUtility.UrlEncode(taskParams.TaskName) },
                { QueryParams.Type, taskParams.TaskType.ToString().ToLower() },
                { QueryParams.Dbrps, HttpUtility.UrlEncode(dbrps) }
            };
            var content = new StringContent(taskParams.TickScript, Encoding.UTF8, "text/plain");

            return await base.RequestClient.PostAsync("task", requestParams, content);
        }
    }
}
