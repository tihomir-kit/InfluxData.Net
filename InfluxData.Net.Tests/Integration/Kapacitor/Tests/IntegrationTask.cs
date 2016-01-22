using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace InfluxData.Net.Integration.Kapacitor.Tests
{
    [Collection("Kapacitor Integration")] // for sharing of the fixture instance between multiple test classes
    [Trait("Kapacitor Integration", "Task")] // for organization of tests; by category, subtype
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
            var taskParams = await _fixture.MockAndSaveTask();

            var response = await _fixture.Sut.Task.GetTask(taskParams.TaskName);
            response.Should().NotBeNull();
            response.Name.Should().Be(taskParams.TaskName);

        }

        [Fact]
        public async Task GetTasks_OnExistingTask_ShouldReturnTaskCollection()
        {
            var taskParams = await _fixture.MockAndSaveTask();

            var response = await _fixture.Sut.Task.GetTasks();
            response.Should().NotBeNull();
            response.Count().Should().BeGreaterOrEqualTo(1);
            var mockedTask = response.FirstOrDefault(p => p.Name == taskParams.TaskName);
            mockedTask.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteTask_OnExistingTask_ShouldDeleteSuccessfully()
        {
            var taskParams = await _fixture.MockAndSaveTask();

            var response = await _fixture.Sut.Task.DeleteTask(taskParams.TaskName);
            response.Success.Should().BeTrue();

            var existingTasks = await _fixture.Sut.Task.GetTasks();
            existingTasks.FirstOrDefault(p => p.Name == taskParams.TaskName).Should().BeNull();
        }
    }
}
