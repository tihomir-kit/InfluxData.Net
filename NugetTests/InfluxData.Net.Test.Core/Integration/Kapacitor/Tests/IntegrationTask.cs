using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using InfluxData.Net.Common.Infrastructure;
using Xunit;
using InfluxData.Net.Tests.Common.AppSettings;
using Newtonsoft.Json;

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
        public virtual async Task DefineTemplateTask_OnValidArguments_ShouldDefineSuccessfully()
        {
            Dictionary<string, object> defineTemplateDictionary = new Dictionary<string, object>()
            {
                { "id", "TestTemplate" },
                { "type", "stream" },
                {
                    "script",
                    "var measurement string\nvar where_filter = lambda: TRUE\nvar info = lambda: TRUE\n  stream\n     |from()\n         .measurement(measurement)\n         .where(where_filter)\n     |alert()\n          .info(info)\n          .log('/home/or/templateLog') "
                }
            };

            string kapacitorUrl = ConfigurationManager.Get("InfluxSettings:KapacitorEndpointUri_v_1_0_0");
            var content = JsonConvert.SerializeObject(defineTemplateDictionary);
            HttpClient client = new HttpClient();
            client.PostAsync(String.Format("{0}/kapacitor/v1/templates", kapacitorUrl), new StringContent(content)).Wait();

            var taskParams = _fixture.MockTemplateTaskParams();

            var response = await _fixture.Sut.Task.DefineTaskAsync(taskParams);
            response.Success.Should().BeTrue();

            client.DeleteAsync(String.Format("{0}/kapacitor/v1/templates/{1}", kapacitorUrl, taskParams.TemplateId)).Wait();
        }

        [Fact]
        public virtual async Task DefineTask_OnNoTickScriptSpecified_ShouldThrowException()
        {
            var taskParams = _fixture.MockDefineTaskParams();
            taskParams.TickScript = String.Empty;

            Func<Task> act = async () => { await _fixture.Sut.Task.DefineTaskAsync(taskParams); };

            act.ShouldThrow<InfluxDataApiException>()
                .WithMessage("InfluxData API responded with status code=BadRequest, response={\n    \"error\": \"must provide TICKscript\"\n}");
        }

        [Fact]
        public virtual async Task DefineTask_OnInvalidTickScriptSpecified_ShouldThrowException()
        {
            var taskParams = _fixture.MockDefineTaskParams();
            taskParams.TickScript = "invalidScript(); // '' borken[[|}";

            Func<Task> act = async () => { await _fixture.Sut.Task.DefineTaskAsync(taskParams); };

            act.ShouldThrow<InfluxDataApiException>()
                .WithMessage("InfluxData API responded with status code=BadRequest, response={\n    \"error\": \"invalid TICKscript: parser: unexpected unknown state line 1 char 16 in \\\"idScript(); // '' bo\\\". expected: \\\"number\\\",\\\"string\\\",\\\"duration\\\",\\\"identifier\\\",\\\"TRUE\\\",\\\"FALSE\\\",\\\"==\\\",\\\"(\\\",\\\"-\\\",\\\"!\\\"\"\n}");
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
                .WithMessage("InfluxData API responded with status code=NotFound, response={\n    \"error\": \"no task exists\"\n}");
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
            disabledTask.Status.Should().Be("disabled");
            disabledTask.Executing.Should().BeFalse();

            var response = await _fixture.Sut.Task.EnableTaskAsync(taskParams.TaskId);
            response.Success.Should().BeTrue();

            var enabledTask = await _fixture.Sut.Task.GetTaskAsync(taskParams.TaskId);
            enabledTask.Status.Should().Be("enabled");
            enabledTask.Executing.Should().BeTrue();
        }

        [Fact]
        public virtual async Task EnableTask_OnNonExistingTask_ShouldThrowException()
        {
            Func<Task> act = async () => { await _fixture.Sut.Task.EnableTaskAsync("nonexistingtask"); };

            act.ShouldThrow<InfluxDataApiException>()
                .WithMessage("InfluxData API responded with status code=NotFound, response={\n    \"error\": \"task does not exist, cannot update\"\n}");
        }

        [Fact]
        public virtual async Task DisableTask_OnExistingTask_ShouldDisableSuccessfully()
        {
            var taskParams = await _fixture.MockAndSaveTask();
            var enableResponse = await _fixture.Sut.Task.EnableTaskAsync(taskParams.TaskId);
            enableResponse.Success.Should().BeTrue();

            var enabledTask = await _fixture.Sut.Task.GetTaskAsync(taskParams.TaskId);
            enabledTask.Status.Should().Be("enabled");
            enabledTask.Executing.Should().BeTrue();

            var disableResponse = await _fixture.Sut.Task.DisableTaskAsync(taskParams.TaskId);
            disableResponse.Success.Should().BeTrue();

            var disabledTask = await _fixture.Sut.Task.GetTaskAsync(taskParams.TaskId);
            disabledTask.Status.Should().Be("disabled");
            disabledTask.Executing.Should().BeFalse();
        }
    }
}
