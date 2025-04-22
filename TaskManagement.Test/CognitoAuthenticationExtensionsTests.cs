using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TaskManagement.Core.Authentication;
using TaskManagement.Core.Extensions;
using TaskManagement.Core.Model.Dto;
using TaskManagement.Core.Services;
using Xunit;
using static TaskManagement.Core.Model.Enums;

namespace TaskManagement.Test
{
    public class CognitoAuthenticationExtensionsTests
    {
        [Fact]
        public void AddCognitoAuthentication_WithValidConfiguration_ShouldConfigureServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var configurationMock = new Mock<IConfiguration>();
            var cognitoSectionMock = new Mock<IConfigurationSection>();

            cognitoSectionMock.Setup(c => c["Region"]).Returns("us-east-1");
            cognitoSectionMock.Setup(c => c["UserPoolId"]).Returns("test-pool-id");
            cognitoSectionMock.Setup(c => c["AppClientId"]).Returns("test-client-id");
            cognitoSectionMock.Setup(c => c["AppClientSecret"]).Returns("test-secret");
            configurationMock.Setup(c => c.GetSection("AWS:Cognito")).Returns(cognitoSectionMock.Object);

            // Act
            services.AddCognitoAuthentication(configurationMock.Object.GetValue<string>("AWS:Cognito:Region"),
                    configurationMock.Object.GetValue<string>("AWS:Cognito:UserPoolId"),
                    configurationMock.Object.GetValue<string>("AWS:Cognito:AppClientId"));
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var cognitoSettings = serviceProvider.GetService<CognitoSettings>();
            Assert.NotNull(cognitoSettings);
            Assert.Equal("us-east-1", cognitoSettings.Region);
            Assert.Equal("test-pool-id", cognitoSettings.UserPoolId);
        }

        [Fact]
        public async Task TaskService_GetProjectTasks_ShouldReturnPagedResponse()
        {
            // Arrange
            var taskServiceMock = new Mock<ITaskService>();
            var expectedResponse = new PagedResponseDto<TaskDto>
            {
                Items = new List<TaskDto>
                {
                    new TaskDto
                    {
                        Id = 1,
                        Title = "Test Task",
                        ProjectId = 1
                    }
                },
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 1
            };

            taskServiceMock.Setup(x => x.GetProjectTasks(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await taskServiceMock.Object.GetProjectTasks(1, 1, 10, "fsd435");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(10, result.PageSize);
        }

        [Fact]
        public async Task TaskService_CreateTask_ShouldReturnCreatedTask()
        {
            // Arrange
            var taskServiceMock = new Mock<ITaskService>();
            var createTaskDto = new CreateTaskDto
            {
                Title = "New Task",
                Description = "Test Description",
                Status = StatusOfTask.ToDo
            };

            var expectedTask = new TaskDto
            {
                Id = 1,
                Title = "New Task",
                Description = "Test Description",
                Status = StatusOfTask.ToDo,
                ProjectId = 1
            };

            taskServiceMock.Setup(x => x.CreateTask(
                It.IsAny<int>(),
                It.IsAny<CreateTaskDto>(),
                It.IsAny<string>()))
                .ReturnsAsync(expectedTask);

            // Act
            var result = await taskServiceMock.Object.CreateTask(1, createTaskDto, "32423");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Task", result.Title);
            Assert.Equal(StatusOfTask.ToDo, result.Status);
        }

        [Fact]
        public void CognitoSettings_WithMissingConfiguration_ShouldThrowException()
        {
            // Arrange
            var services = new ServiceCollection();
            var configurationMock = new Mock<IConfiguration>();
            var emptySectionMock = new Mock<IConfigurationSection>();

            configurationMock.Setup(c => c.GetSection("AWS:Cognito")).Returns(emptySectionMock.Object);
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                services.AddCognitoAuthentication(configurationMock.Object.GetValue<string>("AWS:Cognito:Region"),
                    configurationMock.Object.GetValue<string>("AWS:Cognito:UserPoolId"),
                    configurationMock.Object.GetValue<string>("AWS:Cognito:AppClientId"));
                services.BuildServiceProvider();
            });

            Assert.Contains("Cognito settings are not properly configured", exception.Message);
        }

        [Theory]
        [InlineData(-1, 10)]
        [InlineData(1, 0)]
        [InlineData(1, 101)]
        public async Task TaskService_GetProjectTasks_WithInvalidParameters_ShouldThrowException(int pageNumber, int pageSize)
        {
            // Arrange
            var taskServiceMock = new Mock<ITaskService>();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                taskServiceMock.Object.GetProjectTasks(1, pageNumber, pageSize, "fsdf545"));
        }
    }
}
