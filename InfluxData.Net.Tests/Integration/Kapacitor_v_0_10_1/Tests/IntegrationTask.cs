using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using InfluxData.Net.Common.Infrastructure;
using Xunit;

namespace InfluxData.Net.Integration.Kapacitor.Tests
{
    [Collection("Kapacitor v0.10.1 Integration")] // for sharing of the fixture instance between multiple test classes
    [Trait("Kapacitor v0.10.1 Integration", "Task")] // for organization of tests; by category, subtype
    public class IntegrationTask_v_0_10_1 : IntegrationTask
    {
        public IntegrationTask_v_0_10_1(IntegrationFixture_v_0_10_1 fixture) : base(fixture)
        {
        }
    }
}
