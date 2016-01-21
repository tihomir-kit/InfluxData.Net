using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.Kapacitor.Infrastructure;
using System;
using InfluxData.Net.InfluxDb.Enums;
using InfluxData.Net.Kapacitor.Models;

namespace InfluxData.Net.Kapacitor.ClientModules
{
    public interface ITaskClientModule
    {
        Task<IKapacitorApiResponse> DefineTask(DefineTaskParams taskParams);
    }
}