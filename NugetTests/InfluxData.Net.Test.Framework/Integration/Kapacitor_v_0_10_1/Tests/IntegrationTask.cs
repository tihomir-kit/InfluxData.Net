using System;
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

        [Fact(Skip = "Test not applicable for this Kapacitor version")]
        public override async Task DefineTemplateTask_OnValidArguments_ShouldDefineSuccessfully()
        {
            return;
        }

        [Fact]
        public virtual async Task DefineTask_OnNoTaskIdSpecified_ShouldThrowException()
        {
            var taskParams = _fixture.MockDefineTaskParams();
            taskParams.TaskId = String.Empty;

            Func<Task> act = async () => { await _fixture.Sut.Task.DefineTaskAsync(taskParams); };

            act.ShouldThrow<InfluxDataApiException>()
                .WithMessage("InfluxData API responded with status code=InternalServerError, response={\n    \"Error\": \"key required\"\n}");
        }

        [Fact]
        public override async Task DefineTask_OnNoTickScriptSpecified_ShouldThrowException()
        {
            var taskParams = _fixture.MockDefineTaskParams();
            taskParams.TickScript = String.Empty;

            Func<Task> act = async () => { await _fixture.Sut.Task.DefineTaskAsync(taskParams); };

            act.ShouldThrow<InfluxDataApiException>()
                .WithMessage("InfluxData API responded with status code=BadRequest, response={\n    \"Error\": \"must provide TICKscript via POST data.\"\n}");
        }

        [Fact]
        public override async Task DefineTask_OnInvalidTickScriptSpecified_ShouldThrowException()
        {
            var taskParams = _fixture.MockDefineTaskParams();
            taskParams.TickScript = "invalidScript(); // '' borken[[|}";

            Func<Task> act = async () => { await _fixture.Sut.Task.DefineTaskAsync(taskParams); };

            act.ShouldThrow<InfluxDataApiException>()
                .WithMessage("InfluxData API responded with status code=InternalServerError, response={\n    \"Error\": \"invalid task: parser: unexpected unknown state line 1 char 16 in \\\"idScript(); // '' bo\\\". expected: \\\"number\\\",\\\"string\\\",\\\"duration\\\",\\\"identifier\\\",\\\"TRUE\\\",\\\"FALSE\\\",\\\"==\\\",\\\"(\\\",\\\"-\\\",\\\"!\\\"\"\n}");
        }

        [Fact]
        public override async Task GetTask_OnNonExistingTask_ShouldThrowException()
        {
            Func<Task> act = async () => { await _fixture.Sut.Task.GetTaskAsync("nonexistingtask"); };

            act.ShouldThrow<InfluxDataApiException>()
                .WithMessage("InfluxData API responded with status code=NotFound, response={\n    \"Error\": \"unknown task nonexistingtask\"\n}");
        }

        [Fact]
        public override async Task EnableTask_OnExistingTask_ShouldEnableSuccessfully()
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
        public override async Task EnableTask_OnNonExistingTask_ShouldThrowException()
        {
            Func<Task> act = async () => { await _fixture.Sut.Task.EnableTaskAsync("nonexistingtask"); };

            act.ShouldThrow<InfluxDataApiException>()
                .WithMessage("InfluxData API responded with status code=InternalServerError, response={\n    \"Error\": \"unknown task nonexistingtask\"\n}");
        }

        [Fact]
        public override async Task DisableTask_OnExistingTask_ShouldDisableSuccessfully()
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
