using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CommandsShared;
using ProductsShared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using ProjectsShared;
using ContactsShared;
using CompaniesShared;
using EmploymentsShared;
using ProjectRoleAssignmentsShared;
using LinksShared;
using DesignsShared;
using CodeGenShared;
using FilesShared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SoftwareManagementCoreWeb.Controllers
{
  public class CommandReadOnlyDto
  {
    private ICommandState _state;
    public CommandReadOnlyDto(ICommandState state)
    {
      _state = state;
    }
    public Guid Guid {  get { return _state.Guid; } }
    public string Name { get { return _state.CommandTypeId.Replace(_state.Entity + "Command", ""); } }
    public string Entity { get { return _state.Entity; } }
    public string ParametersJson { get { return _state.ParametersJson; } }
    public Guid EntityGuid {  get { return _state.EntityGuid; } }
    public string CreatedOn { get { return _state.CreatedOn.ToString("yyyy-MM-dd"); } }
    public string ReceivedOn { get { return _state.ReceivedOn.HasValue ? _state.ReceivedOn.Value.ToString("yyyy-MM-dd") : ""; } }
    public string ExecutedOn { get { return _state.ExecutedOn.HasValue ? _state.ExecutedOn.Value.ToString("yyyy-MM-dd") : ""; } }
    public string UserName { get { return _state.UserName; } }

  }
  public class CommandBatchResults
  {
    public bool Success { get; set; }
    public IEnumerable<CommandDto> ExecutedCommands { get; set; }
    public string Message { get; set; }
  }

  [EnableCors("SiteCorsPolicy")]
  [Route("api/[controller]")]
  [AllowAnonymous]
  public class CommandsController : Controller
  {
    private IProductService _productService;
    private IDesignService _designService;
    private IProjectService _projectService;
    private IContactService _contactService;
    private ICompanyService _companyService;
    private ILinkService _linkService;
    private IFileService _fileService;

    private IEmploymentService _employmentService;
    private IProjectRoleAssignmentService _projectRoleAssignmentService;
    private ICommandStateRepository _commandStateRepository;
    private ICommandService _commandManager;
    private ICodeGenService _codeGenService;

    public CommandsController(ICommandService commandManager, IProductService productService, IProjectService projectService, IContactService contactService, IEmploymentService employmentService, ICompanyService companyService, IProjectRoleAssignmentService projectRoleAssignmentService, ILinkService linkService, IFileService fileService, IDesignService designService, ICodeGenService codeGenService, ICommandStateRepository commandStateRepository)
    {
      _commandManager = commandManager;

      _productService = productService;
      _designService = designService;
      _projectService = projectService;
      _contactService = contactService;
      _companyService = companyService;
      _linkService = linkService;
      _fileService = fileService;
      _codeGenService = codeGenService;

      _employmentService = employmentService;
      _projectRoleAssignmentService = projectRoleAssignmentService;

      _commandStateRepository = commandStateRepository;

      ConfigureCommandManager();
    }

    private void ConfigureCommandManager()
    {
      var projectsConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProjectsShared", Entity = "Project", Processor = _projectService };
      var productsConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProductsShared", Entity = "Product", Processor = _productService };
      var productFeaturesConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProductsShared", Entity = "ProductFeature", Processor = _productService };
      var productIssuesConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProductsShared", Entity = "ProductIssue", Processor = _productService };
      var productVersionsConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProductsShared", Entity = "ProductVersion", Processor = _productService };
      var productConfigOptionConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProductsShared", Entity = "ProductConfigOption", Processor = _productService };
      var designsConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "DesignsShared", Entity = "Design", Processor = _designService };
      var epicElementConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "DesignsShared", Entity = "EpicElement", Processor = _designService };
      var entityElementConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "DesignsShared", Entity = "EntityElement", Processor = _designService };
      var commandElementConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "DesignsShared", Entity = "CommandElement", Processor = _designService };
      var propertyElementConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "DesignsShared", Entity = "PropertyElement", Processor = _designService };
      var contactsConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ContactsShared", Entity = "Contact", Processor = _contactService };
      var companiesConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "CompaniesShared", Entity = "Company", Processor = _companyService };
      var environmentsConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "CompaniesShared", Entity = "CompanyEnvironment", Processor = _companyService };
      var hardwareConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "CompaniesShared", Entity = "CompanyEnvironmentHardware", Processor = _companyService };
      var linksConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "LinksShared", Entity = "Link", Processor = _linkService };
      var filesConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "FilesShared", Entity = "File", Processor = _fileService };
      var codeGenConfig = new ProcessorConfig { Assembly = "CodeGenCSharpUpdaterCore", NameSpace = "CodeGenShared", Entity = "CodeGen", Processor = _codeGenService };

      var employmentsConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "EmploymentsShared", Entity = "Employment", Processor = _employmentService };
      var projectRoleAssignmentsConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProjectRoleAssignmentsShared", Entity = "ProjectRoleAssignment", Processor = _projectRoleAssignmentService };

      _commandManager.AddConfig(projectsConfig);
      _commandManager.AddConfig(productsConfig);
      _commandManager.AddConfig(productVersionsConfig);
      _commandManager.AddConfig(productFeaturesConfig);
      _commandManager.AddConfig(productIssuesConfig);
      _commandManager.AddConfig(productConfigOptionConfig);
      _commandManager.AddConfig(designsConfig);
      _commandManager.AddConfig(epicElementConfig);
      _commandManager.AddConfig(entityElementConfig);
      _commandManager.AddConfig(commandElementConfig);
      _commandManager.AddConfig(propertyElementConfig);
      _commandManager.AddConfig(contactsConfig);
      _commandManager.AddConfig(companiesConfig);
      _commandManager.AddConfig(environmentsConfig);
      _commandManager.AddConfig(hardwareConfig);
      _commandManager.AddConfig(employmentsConfig);
      _commandManager.AddConfig(projectRoleAssignmentsConfig);
      _commandManager.AddConfig(linksConfig);
      _commandManager.AddConfig(filesConfig);
      _commandManager.AddConfig(codeGenConfig);
    }

    // GET: api/commands
    [HttpGet]
    public IEnumerable<CommandReadOnlyDto> Get()
    {
      var states = _commandStateRepository.GetCommandStates();
      var dtos = states.Select(s => new CommandReadOnlyDto(s)).ToList();
      return dtos;
    }
    // GET api/links/forguid/5
    [HttpGet("forGuid/{forGuid}")]
    public IEnumerable<CommandReadOnlyDto> GetForGuid(Guid forGuid)
    {
      var states = _commandStateRepository.GetCommandStates(forGuid);
      var dtos = states.Select(s => new CommandReadOnlyDto(s)).ToList();
      return dtos;
    }
    [HttpGet]
    [Route("executenew")]
    public CommandBatchResults ExecuteNew()
    {
      var result = new CommandBatchResults();
      var commands = _commandManager.GetUnprocessedCommands();

      ExecuteCommands(result, commands);

      return result;
    }

    // POST api/commands/batch
    [HttpPost]
    [Route("batch")]
    public CommandBatchResults Post([FromBody]IEnumerable<CommandDto> commands)
    {
      // simple, direct execution of one or more commands, like a transaction
      var result = new CommandBatchResults();

      ExecuteCommands(result, commands);

      return result;
    }

    private void ExecuteCommands(CommandBatchResults result, IEnumerable<CommandDto> commands)
    {
      try
      {

        foreach (var command in commands)
        {
          var typedCommand = _commandManager.ProcessCommand(command, command.State);
          command.ExecutedOn = typedCommand.ExecutedOn;
        }

        // these can be all the same contexts, but may also be different
        _productService.PersistChanges();
        _designService.PersistChanges();
        _projectService.PersistChanges();
        _contactService.PersistChanges();
        _companyService.PersistChanges();
        _employmentService.PersistChanges();
        _projectRoleAssignmentService.PersistChanges();
        _linkService.PersistChanges();
        _fileService.PersistChanges();
        _commandManager.PersistChanges();
        // todo: add persistchanges to codegen if possible

        result.ExecutedCommands = commands;
        result.Success = true;
      }
      catch (Exception ex)
      {
        result.Success = false;
        result.Message = ex.Message;
      }
    }

  }
}
