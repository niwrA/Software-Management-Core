using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CommandsShared;
using ProductsShared;
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
using ProductInstallationsShared;

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
    public Guid Guid { get { return _state.Guid; } }
    public string Name { get { return _state.CommandTypeId.Replace(_state.Entity + "Command", ""); } }
    public string Entity { get { return _state.Entity; } }
    public string ParametersJson { get { return _state.ParametersJson; } }
    public Guid EntityGuid { get { return _state.EntityGuid; } }
    public string CreatedOn { get { return _state.CreatedOn.ToString("yyyy-MM-dd"); } }
    public string ReceivedOn { get { return _state.ReceivedOn.HasValue ? _state.ReceivedOn.Value.ToString("yyyy-MM-dd") : ""; } }
    public string ExecutedOn { get { return _state.ExecutedOn.HasValue ? _state.ExecutedOn.Value.ToString("yyyy-MM-dd") : ""; } }
    public string UserName { get { return _state.UserName; } }

  }
  public class CommandBatchResultsDto
  {
    public bool Success { get; set; }
    public IEnumerable<CommandDto> ExecutedCommands { get; set; }
    public string Message { get; set; }
  }

  [EnableCors("SiteCorsPolicy")]
  [Route("api/[controller]")]
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
    private IProductInstallationService _productInstallationService;
    private ICommandStateRepository _commandStateRepository;
    private ICommandService _commandManager;
    private ICodeGenService _codeGenService;

    public CommandsController(ICommandService commandManager, IProductService productService, IProjectService projectService, IContactService contactService, IEmploymentService employmentService, ICompanyService companyService, IProjectRoleAssignmentService projectRoleAssignmentService, IProductInstallationService productInstallationService, ILinkService linkService, IFileService fileService, IDesignService designService, ICodeGenService codeGenService, ICommandStateRepository commandStateRepository)
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
      _productInstallationService = productInstallationService;

      _commandStateRepository = commandStateRepository;

      ConfigureCommandManager();
    }

    private void ConfigureCommandManager()
    {
      var processorConfigs = new List<IProcessorConfig>
      {
        new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProjectsShared", Entity = "Project", Processor = _projectService },
        new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProductsShared", Entity = "Product", Processor = _productService },
        new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProductsShared", Entity = "ProductFeature", Processor = _productService },
        new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProductsShared", Entity = "ProductIssue", Processor = _productService },
        new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProductsShared", Entity = "ProductVersion", Processor = _productService },
        new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProductsShared", Entity = "ProductConfigOption", Processor = _productService },
        new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "DesignsShared", Entity = "Design", Processor = _designService },
        new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "DesignsShared", Entity = "EpicElement", Processor = _designService },
        new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "DesignsShared", Entity = "EntityElement", Processor = _designService },
        new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "DesignsShared", Entity = "CommandElement", Processor = _designService },
        new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "DesignsShared", Entity = "PropertyElement", Processor = _designService },
        new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ContactsShared", Entity = "Contact", Processor = _contactService },
        new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "CompaniesShared", Entity = "Company", Processor = _companyService },
        new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "CompaniesShared", Entity = "CompanyEnvironment", Processor = _companyService },
        new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "CompaniesShared", Entity = "CompanyEnvironmentHardware", Processor = _companyService },
        new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "LinksShared", Entity = "Link", Processor = _linkService },
        new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "FilesShared", Entity = "File", Processor = _fileService },
        new ProcessorConfig { Assembly = "CodeGenCSharpUpdaterCore", NameSpace = "CodeGenShared", Entity = "CodeGen", Processor = _codeGenService },
        new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "EmploymentsShared", Entity = "Employment", Processor = _employmentService },
        new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProjectRoleAssignmentsShared", Entity = "ProjectRoleAssignment", Processor = _projectRoleAssignmentService },
        new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProductInstallationsShared", Entity = "ProductInstallation", Processor = _productInstallationService }
      };

      _commandManager.AddProcessorConfigs(processorConfigs);
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
    public CommandBatchResultsDto ExecuteNew()
    {
      var result = new CommandBatchResultsDto();
      var commands = _commandManager.GetUnprocessedCommands();

      ExecuteCommands(result, commands);

      return result;
    }

    // POST api/commands/batch
    [HttpPost]
    [Route("batch")]
    public CommandBatchResultsDto Post([FromBody]IEnumerable<CommandDto> commands)
    {
      // simple, direct execution of one or more commands, like a transaction
      var result = new CommandBatchResultsDto();

      ExecuteCommands(result, commands);

      return result;
    }

    // todo: perhaps also support an async pipeline that awaits execution of all commands in paralel
    private void ExecuteCommands(CommandBatchResultsDto result, IEnumerable<CommandDto> commands)
    {
      try
      {

        foreach (var command in commands)
        {
          // if we are in an authorized context, always record the authenticated user
          if (User != null && User.Identity != null)
          {
            if (User.Identity.Name != null)
            {
              command.UserName = User.Identity.Name;
            }
          }
          var typedCommands = _commandManager.ProcessCommand(command, command.State);
          // todo: return info per processor
          foreach (var typedCommand in typedCommands)
          {
            command.ExecutedOn = typedCommand.ExecutedOn;
          }
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
