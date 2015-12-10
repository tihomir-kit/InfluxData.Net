using System.Net.Http;

namespace InfluxData.Net.Contracts
{
    internal interface IRequestContent
    {
        HttpContent GetContent();
    }
}