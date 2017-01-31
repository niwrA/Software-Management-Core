using CommandsShared;
using SoftwareManagementCoreApi;
using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Xunit;
using ProjectsShared;
using SoftwareManagementCoreApiTests;
using Moq;
using SoftwareManagementCoreWeb.Controllers;
using ProductsShared;
using System.Collections.Generic;
using System.Linq;
using ContactsShared;

namespace SoftwareManagementCoreApiTests
{
    [Trait("Controller", "CommandController")]
    public class CommandControllerTests
    {
        [Fact(DisplayName = "PostCommands")]
        public void CanPostCommands()
        {
            var commandManager = new Mock<ICommandManager>();
            var commandRepo = new Mock<ICommandStateRepository>();

            var projectsService = new Mock<IProjectService>();
            var productsService = new Mock<IProductService>();
            var contactsService = new Mock<IContactService>();

            var projectCommandDto = new Fakes.RenameProjectCommandDto();
            var productCommandDto = new Fakes.RenameProductCommandDto();

            commandRepo.Setup(s => s.CreateCommandState()).Returns(new Fakes.CommandState());

            var projectCommand = new Fakes.RenameProjectCommand(projectCommandDto, commandRepo.Object);
            var productCommand = new Fakes.RenameProductCommand(productCommandDto, commandRepo.Object);

            commandManager.Setup(s => s.ProcessCommand(projectCommandDto)).Returns(projectCommand);
            commandManager.Setup(s => s.ProcessCommand(productCommandDto)).Returns(productCommand);

            var sut = new CommandsController(commandManager.Object, productsService.Object, projectsService.Object, contactsService.Object);
            var sutResult = sut.Post(new List<CommandDto> { projectCommandDto, productCommandDto });

            Assert.Equal(2, sutResult.Count());
        }
    }
    [Trait("Controller", "ProductController")]
    public class CommandControllerIntegrationTests : IClassFixture<TestFixture<Startup>>
    {
        private readonly HttpClient _client;
        public CommandControllerIntegrationTests(TestFixture<Startup> fixture)
        {
            _client = fixture.Client;
        }
        [Fact(DisplayName = "CanPostCreateProductCommand_IntegrationTest", Skip = "Setup not yet complete")]
        public void CreatePostReturnsCreatedCommandAsync()
        {
            // Arrange
            var testGuid = Guid.NewGuid();
            var newIdea = new CommandDto { Entity = "Product", Name = "Create", EntityGuid = testGuid, ParametersJson = @"{ Name: 'Test Project'}" };

            // Act
            var response = _client.PostAsJsonAsync("/api/commands", newIdea).Result;

            // Assert
            response.EnsureSuccessStatusCode();
            var returnedSession = response.Content.ReadAsJsonAsync<CommandDto>().Result;
            Assert.Equal(testGuid, returnedSession.EntityGuid);
        }
    }
}
