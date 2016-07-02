using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using InfluxData.Net.Common.Infrastructure;
using Xunit;
using InfluxData.Net.Integration.Kapacitor;

namespace InfluxData.Net.Integration.Kapacitor.Tests
{
    public abstract class IntegrationTask : IDisposable
    {
        protected readonly IIntegrationFixture _fixture;

        public IntegrationTask(IIntegrationFixture fixture)
        {
            _fixture = fixture;
            _fixture.TestSetup();
        }

        public void Dispose()
        {
            _fixture.TestTearDown();
        }

        [Fact]
        public virtual async Task DefineTask_OnValidArguments_ShouldDefineSuccessfully()
        {
            var taskParams = _fixture.MockDefineTaskParams();

            var response = await _fixture.Sut.Task.DefineTaskAsync(taskParams);
            response.Success.Should().BeTrue();
        }

        [Fact]
        public virtual async Task DefineTask_OnNoTaskNameSpecified_ShouldThrowException()
        {
            var taskParams = _fixture.MockDefineTaskParams();
            taskParams.TaskId = String.Empty;

            Func<Task> act = async () => { await _fixture.Sut.Task.DefineTaskAsync(taskParams); };

            act.ShouldThrow<InfluxDataApiException>()
                .WithMessage("InfluxData API responded with status code=InternalServerError, response={\n    \"Error\": \"key required\"\n}");
        }

        [Fact]
        public virtual async Task DefineTask_OnNoTickScriptSpecified_ShouldThrowException()
        {
            var taskParams = _fixture.MockDefineTaskParams();
            taskParams.TickScript = String.Empty;

            Func<Task> act = async () => { await _fixture.Sut.Task.DefineTaskAsync(taskParams); };

            act.ShouldThrow<InfluxDataApiException>()
                .WithMessage("InfluxData API responded with status code=BadRequest, response={\n    \"Error\": \"must provide TICKscript via POST data.\"\n}");
        }

        [Fact]
        public virtual async Task DefineTask_OnInvalidTickScriptSpecified_ShouldThrowException()
        {
            var taskParams = _fixture.MockDefineTaskParams();
            taskParams.TickScript = "invalidScript(); // '' borken[[|}";

            Func<Task> act = async () => { await _fixture.Sut.Task.DefineTaskAsync(taskParams); };

            act.ShouldThrow<InfluxDataApiException>()
                .WithMessage("InfluxData API responded with status code=InternalServerError, response={\n    \"Error\": \"invalid task: parser: unexpected unknown state line 1 char 16 in \\\"idScript(); // '' bo\\\". expected: \\\"number\\\",\\\"string\\\",\\\"duration\\\",\\\"identifier\\\",\\\"TRUE\\\",\\\"FALSE\\\",\\\"==\\\",\\\"(\\\",\\\"-\\\",\\\"!\\\"\"\n}");
        }

        [Fact]
        public virtual async Task GetTask_OnExistingTask_ShouldReturnTask()
        {
            var taskParams = await _fixture.MockAndSaveTask();

            var response = await _fixture.Sut.Task.GetTaskAsync(taskParams.TaskId);
            response.Should().NotBeNull();
            response.Id.Should().Be(taskParams.TaskId);

        }

        [Fact]
        public virtual async Task GetTask_OnNonExistingTask_ShouldThrowException()
        {
            Func<Task> act = async () => { await _fixture.Sut.Task.GetTaskAsync("nonexistingtask"); };

            act.ShouldThrow<InfluxDataApiException>()
                .WithMessage("InfluxData API responded with status code=NotFound, response={\n    \"Error\": \"unknown task nonexistingtask\"\n}");
        }

        [Fact]
        public virtual async Task GetTasks_OnExistingTask_ShouldReturnTaskCollection()
        {
            var taskParams = await _fixture.MockAndSaveTask();

            var response = await _fixture.Sut.Task.GetTasksAsync();
            response.Should().NotBeNull();
            response.Count().Should().BeGreaterOrEqualTo(1);
            var mockedTask = response.FirstOrDefault(p => p.Id == taskParams.TaskId);
            mockedTask.Should().NotBeNull();
        }

        [Fact]
        public virtual async Task DeleteTask_OnExistingTask_ShouldDeleteSuccessfully()
        {
            var taskParams = await _fixture.MockAndSaveTask();

            var response = await _fixture.Sut.Task.DeleteTaskAsync(taskParams.TaskId);
            response.Success.Should().BeTrue();

            var existingTasks = await _fixture.Sut.Task.GetTasksAsync();
            existingTasks.FirstOrDefault(p => p.Id == taskParams.TaskId).Should().BeNull();
        }

        [Fact]
        public virtual async Task EnableTask_OnExistingTask_ShouldEnableSuccessfully()
        {
            var taskParams = await _fixture.MockAndSaveTask();

            var disabledTask = await _fixture.Sut.Task.GetTaskAsync(taskParams.TaskId);
            disabledTask.Enabled.Should().BeFalse();
            disabledTask.Executing.Should().BeFalse();

            var response = await _fixture.Sut.Task.EnableTaskAsync(taskParams.TaskId);
            response.Success.Should().BeTrue();

            var enabledTask = await _fixture.Sut.Task.GetTaskAsync(taskParams.TaskId);
            enabledTask.Enabled.Should().BeTrue();
            enabledTask.Executing.Should().BeTrue();
        }

        [Fact]
        public virtual async Task EnableTask_OnNonExistingTask_ShouldThrowException()
        {
            Func<Task> act = async () => { await _fixture.Sut.Task.EnableTaskAsync("nonexistingtask"); };

            act.ShouldThrow<InfluxDataApiException>()
                .WithMessage("InfluxData API responded with status code=InternalServerError, response={\n    \"Error\": \"unknown task nonexistingtask\"\n}");
        }

        [Fact]
        public virtual async Task DisableTask_OnExistingTask_ShouldDisableSuccessfully()
        {
            var taskParams = await _fixture.MockAndSaveTask();
            var enableResponse = await _fixture.Sut.Task.EnableTaskAsync(taskParams.TaskId);
            enableResponse.Success.Should().BeTrue();

            var enabledTask = await _fixture.Sut.Task.GetTaskAsync(taskParams.TaskId);
            enabledTask.Enabled.Should().BeTrue();
            enabledTask.Executing.Should().BeTrue();

            var disableResponse = await _fixture.Sut.Task.DisableTaskAsync(taskParams.TaskId);
            disableResponse.Success.Should().BeTrue();

            var disabledTask = await _fixture.Sut.Task.GetTaskAsync(taskParams.TaskId);
            disabledTask.Enabled.Should().BeFalse();
            disabledTask.Executing.Should().BeFalse();
        }
    }
}
