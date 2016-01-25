using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using InfluxData.Net.Common.Infrastructure;
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

            var response = await _fixture.Sut.Task.DefineTaskAsync(taskParams);
            response.Success.Should().BeTrue();
        }

        [Fact]
        public async Task DefineTask_OnNoTaskNameSpecified_ShouldThrowException()
        {
            var taskParams = _fixture.MockDefineTaskParams();
            taskParams.TaskName = String.Empty;

            Func<Task> act = async () => { await _fixture.Sut.Task.DefineTaskAsync(taskParams); };

            act.ShouldThrow<InfluxDataApiException>()
                .WithMessage("InfluxData API responded with status code=InternalServerError, response={\n    \"Error\": \"key required\"\n}");
        }

        [Fact]
        public async Task DefineTask_OnNoTickScriptSpecified_ShouldThrowException()
        {
            var taskParams = _fixture.MockDefineTaskParams();
            taskParams.TickScript = String.Empty;

            Func<Task> act = async () => { await _fixture.Sut.Task.DefineTaskAsync(taskParams); };

            act.ShouldThrow<InfluxDataApiException>()
                .WithMessage("InfluxData API responded with status code=BadRequest, response={\n    \"Error\": \"must provide TICKscript via POST data.\"\n}");
        }

        [Fact]
        public async Task DefineTask_OnInvalidTickScriptSpecified_ShouldThrowException()
        {
            var taskParams = _fixture.MockDefineTaskParams();
            taskParams.TickScript = "invalidScript(); // '' borken[[|}";

            Func<Task> act = async () => { await _fixture.Sut.Task.DefineTaskAsync(taskParams); };

            act.ShouldThrow<InfluxDataApiException>()
                .WithMessage("InfluxData API responded with status code=InternalServerError, response={\n    \"Error\": \"invalid task: parser: unexpected unknown state line 1 char 16 in \\\"idScript(); // '' bo\\\". expected: \\\"number\\\",\\\"string\\\",\\\"duration\\\",\\\"identifier\\\",\\\"TRUE\\\",\\\"FALSE\\\",\\\"==\\\",\\\"(\\\",\\\"-\\\",\\\"!\\\"\"\n}");
        }

        [Fact]
        public async Task GetTask_OnExistingTask_ShouldReturnTask()
        {
            var taskParams = await _fixture.MockAndSaveTask();

            var response = await _fixture.Sut.Task.GetTaskAsync(taskParams.TaskName);
            response.Should().NotBeNull();
            response.Name.Should().Be(taskParams.TaskName);

        }

        [Fact]
        public async Task GetTask_OnNonExistingTask_ShouldThrowException()
        {
            Func<Task> act = async () => { await _fixture.Sut.Task.GetTaskAsync("nonexistingtask"); };

            act.ShouldThrow<InfluxDataApiException>()
                .WithMessage("InfluxData API responded with status code=NotFound, response={\n    \"Error\": \"unknown task nonexistingtask\"\n}");
        }

        [Fact]
        public async Task GetTasks_OnExistingTask_ShouldReturnTaskCollection()
        {
            var taskParams = await _fixture.MockAndSaveTask();

            var response = await _fixture.Sut.Task.GetTasksAsync();
            response.Should().NotBeNull();
            response.Count().Should().BeGreaterOrEqualTo(1);
            var mockedTask = response.FirstOrDefault(p => p.Name == taskParams.TaskName);
            mockedTask.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteTask_OnExistingTask_ShouldDeleteSuccessfully()
        {
            var taskParams = await _fixture.MockAndSaveTask();

            var response = await _fixture.Sut.Task.DeleteTaskAsync(taskParams.TaskName);
            response.Success.Should().BeTrue();

            var existingTasks = await _fixture.Sut.Task.GetTasksAsync();
            existingTasks.FirstOrDefault(p => p.Name == taskParams.TaskName).Should().BeNull();
        }

        [Fact]
        public async Task EnableTask_OnExistingTask_ShouldEnableSuccessfully()
        {
            var taskParams = await _fixture.MockAndSaveTask();

            var disabledTask = await _fixture.Sut.Task.GetTaskAsync(taskParams.TaskName);
            disabledTask.Enabled.Should().BeFalse();
            disabledTask.Executing.Should().BeFalse();

            var response = await _fixture.Sut.Task.EnableTaskAsync(taskParams.TaskName);
            response.Success.Should().BeTrue();

            var enabledTask = await _fixture.Sut.Task.GetTaskAsync(taskParams.TaskName);
            enabledTask.Enabled.Should().BeTrue();
            enabledTask.Executing.Should().BeTrue();
        }

        [Fact]
        public async Task EnableTask_OnNonExistingTask_ShouldThrowException()
        {
            Func<Task> act = async () => { await _fixture.Sut.Task.EnableTaskAsync("nonexistingtask"); };

            act.ShouldThrow<InfluxDataApiException>()
                .WithMessage("InfluxData API responded with status code=InternalServerError, response={\n    \"Error\": \"unknown task nonexistingtask\"\n}");
        }

        [Fact]
        public async Task DisableTask_OnExistingTask_ShouldDisableSuccessfully()
        {
            var taskParams = await _fixture.MockAndSaveTask();
            var enableResponse = await _fixture.Sut.Task.EnableTaskAsync(taskParams.TaskName);
            enableResponse.Success.Should().BeTrue();

            var enabledTask = await _fixture.Sut.Task.GetTaskAsync(taskParams.TaskName);
            enabledTask.Enabled.Should().BeTrue();
            enabledTask.Executing.Should().BeTrue();

            var disableResponse = await _fixture.Sut.Task.DisableTaskAsync(taskParams.TaskName);
            disableResponse.Success.Should().BeTrue();

            var disabledTask = await _fixture.Sut.Task.GetTaskAsync(taskParams.TaskName);
            disabledTask.Enabled.Should().BeFalse();
            disabledTask.Executing.Should().BeFalse();
        }
    }
}
