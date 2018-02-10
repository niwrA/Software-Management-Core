using niwrA.CommandManager;
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
using CompaniesShared;
using EmploymentsShared;
using ProjectRoleAssignmentsShared;
using LinksShared;
using DesignsShared;
using CodeGenShared;
using FilesShared;
using ProductInstallationsShared;

namespace SoftwareManagementCoreApiTests
{
  [Trait("Controller", "CommandController")]
  [Trait("TestType", "EndToEnd")]
  public class CommandControllerTests
  {
    [Fact(DisplayName = "PostCommands")]
    public void CanPostCommands()
    {
      var commandService = new Mock<ICommandService>();
      var commandManager = new Mock<ICommandManager>();
      var commandRepo = new Mock<ICommandStateRepository>();

      var projectsService = new Mock<IProjectService>();
      var productsService = new Mock<IProductService>();
      var designsService = new Mock<IDesignService>();
      var contactsService = new Mock<IContactService>();
      var companiesService = new Mock<ICompanyService>();
      var employmentsService = new Mock<IEmploymentService>();
      var projectRoleAssignmentsService = new Mock<IProjectRoleAssignmentService>();
      var linksService = new Mock<ILinkService>();
      var filesService = new Mock<IFileService>();
      var codeGenService = new Mock<ICodeGenService>();
      var productInstallationService = new Mock<IProductInstallationService>();

      var projectCommandDto = new Fakes.RenameProjectCommandDto();
      var productCommandDto = new Fakes.RenameProductCommandDto();

      commandRepo.Setup(s => s.CreateCommandState(Guid.NewGuid())).Returns(new Fakes.CommandState());

      var projectCommand = new Fakes.RenameProjectCommand(projectCommandDto, commandRepo.Object);
      var productCommand = new Fakes.RenameProductCommand(productCommandDto, commandRepo.Object);

      var commands = new List<ICommand> { projectCommand, productCommand };

      commandManager.Setup(s => s.ProcessCommands(new List<CommandDto> { projectCommandDto }));

      var sut = new CommandsController(commandManager.Object, productsService.Object, projectsService.Object, contactsService.Object, employmentsService.Object, companiesService.Object, projectRoleAssignmentsService.Object, productInstallationService.Object, linksService.Object, filesService.Object, designsService.Object, codeGenService.Object, commandRepo.Object);
      var sutResult = sut.Post(new List<CommandDto> { projectCommandDto, productCommandDto });

      Assert.True(sutResult.Success);
      Assert.Equal(2, sutResult.Count);
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
    // todo: inject repository or configure/setup a real test repository
    [Fact(DisplayName = "CanPostRenameProductCommand_EndToEndTest", Skip = "Functioning end-to-end test, only enable for end-to-end testing")]
    public void CreatePostReturnsCreatedCommandAsync()
    {
      // Arrange
      var testGuid = Guid.Parse("DB90B521-D9C2-4F6B-89D7-E89EC36021D2");
      var command = new CommandDto { Guid = Guid.NewGuid(), Entity = "Product", Command = "Rename", EntityGuid = testGuid, ParametersJson = @"{ Name: 'Test Product renamed from test'}", CreatedOn = DateTime.UtcNow };
      var commands = new List<CommandDto> { command };
      // Act
      var response = _client.PostAsJsonAsync("/api/commands/batch", commands).Result;

      // Assert
      response.EnsureSuccessStatusCode();
      var returnedCommands = response.Content.ReadAsJsonAsync<List<CommandDto>>().Result;
      Assert.Equal(testGuid, returnedCommands.First().EntityGuid);
    }
  }
}
