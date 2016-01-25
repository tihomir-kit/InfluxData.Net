using InfluxData.Net.Kapacitor.ClientModules;

namespace InfluxData.Net.Kapacitor
{
    public interface IKapacitorClient
    {
        ITaskClientModule Task { get; }
    }
}