using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using System.Threading.Tasks;
using InfluxData.Net.Common.Enums;
using Xunit;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.Kapacitor.Models;

namespace InfluxData.Net.Integration.Kapacitor.Tests
{
    [Collection("Kapacitor Integration")]
    [Trait("Kapacitor Integration", "Task")]
    public class IntegrationTask : IDisposable
    {
        private readonly IntegrationFixture _fixture;

        public IntegrationTask(IntegrationFixture fixture)
        {
            _fixture = fixture;
            _fixture.TestSetup();
        }

        public void Dispose()
        {
            _fixture.TestTearDown();
        }

        [Fact]
        public async Task DefineTask_OnValidArguments_ShouldDefineSuccessfully()
        {
            var defineTaskParams = _fixture.MockDefineTaskParams();

            var response = await _fixture.Sut.Task.DefineTask(defineTaskParams);
            response.Success.Should().BeTrue();
        }

        [Fact]
        public async Task GetTask_OnExistingTask_ShouldReturnTask()
        {
            var defineTaskParams = await _fixture.MockAndPostDefineTaskParams();

            var response = await _fixture.Sut.Task.GetTask(defineTaskParams.TaskName);

        }
    }
}
