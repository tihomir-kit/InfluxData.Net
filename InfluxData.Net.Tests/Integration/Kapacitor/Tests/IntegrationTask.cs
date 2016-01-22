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
            var taskParams = _fixture.MockDefineTaskParams();

            var response = await _fixture.Sut.Task.DefineTask(taskParams);
            response.Success.Should().BeTrue();
        }

        [Fact]
        public async Task GetTask_OnExistingTask_ShouldReturnTask()
        {
            var taskParams = await _fixture.MockAndPostTask();

            var response = await _fixture.Sut.Task.GetTask(taskParams.TaskName);
            response.Should().NotBeNull();
            response.Name.Should().Be(taskParams.TaskName);

        }

        [Fact]
        public async Task GetTasks_OnExistingTask_ShouldReturnTaskCollection()
        {
            var taskParams = await _fixture.MockAndPostTask();

            var response = await _fixture.Sut.Task.GetTasks();
            response.Should().NotBeNull();
            response.Count().Should().BeGreaterOrEqualTo(1);
            var mockedTask = response.FirstOrDefault(p => p.Name == taskParams.TaskName);
            mockedTask.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteTask_OnExistingTask_ShouldDeleteSuccessfully()
        {
            var taskParams = await _fixture.MockAndPostTask();

            var response = await _fixture.Sut.Task.DeleteTask(taskParams.TaskName);
            response.Success.Should().BeTrue();

            var existingTasks = await _fixture.Sut.Task.GetTasks();
            existingTasks.FirstOrDefault(p => p.Name == taskParams.TaskName).Should().BeNull();
        }
    }
}
