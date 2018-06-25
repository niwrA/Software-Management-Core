using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using niwrA.CommandManager;
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
using niwrA.CommandManager.Contracts;

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
    public string Name { get { return _state.Command; } }
    public string Version { get { return _state.CommandVersion; } }
    public string Entity { get { return _state.Entity; } }
    public string EntityGuid { get { return _state.EntityGuid.ToLower(); } }
    public string EntityRoot { get { return _state.EntityRoot; } }
    public string EntityRootGuid { get { return _state.EntityRootGuid.ToLower(); } }
    public string ParametersJson { get { return _state.ParametersJson; } }
    public string CreatedOn { get { return _state.CreatedOn.ToString("yyyy-MM-dd"); } }
    public string ReceivedOn { get { return _state.ReceivedOn.HasValue ? _state.ReceivedOn.Value.ToString("yyyy-MM-dd") : ""; } }
    public string ExecutedOn { get { return _state.ExecutedOn.HasValue ? _state.ExecutedOn.Value.ToString("yyyy-MM-dd") : ""; } }
    public string UserName { get { return _state.UserName; } }

  }
  public class CommandBatchResultsDto
  {
    public bool Success { get; set; }
    public string Message { get; set; }
    public int Count { get; set; }
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
    private ICommandManager _commandManager;
    private ICodeGenService _codeGenService;

    public CommandsController(ICommandManager commandManager, IProductService productService, IProjectService projectService, IContactService contactService, IEmploymentService employmentService, ICompanyService companyService, IProjectRoleAssignmentService projectRoleAssignmentService, IProductInstallationService productInstallationService, ILinkService linkService, IFileService fileService, IDesignService designService, ICodeGenService codeGenService, ICommandStateRepository commandStateRepository)
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
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "ProjectsShared", entityRoot : "Project", processor : _projectService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "ProductsShared", entityRoot : "Product", processor : _productService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "ProductsShared", entityRoot : "ProductFeature", processor : _productService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "ProductsShared", entityRoot : "ProductIssue", processor : _productService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "ProductsShared", entityRoot : "ProductVersion", processor : _productService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "ProductsShared", entityRoot : "ProductConfigOption", processor : _productService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "DesignsShared", entityRoot : "Design", processor : _designService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "DesignsShared", entityRoot : "EpicElement", processor : _designService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "DesignsShared", entityRoot : "EntityRootElement", processor : _designService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "DesignsShared", entityRoot : "CommandElement", processor : _designService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "DesignsShared", entityRoot : "PropertyElement", processor : _designService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "ContactsShared", entityRoot : "Contact", processor : _contactService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "CompaniesShared", entityRoot : "Company", processor : _companyService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "CompaniesShared", entityRoot : "CompanyRole", processor : _companyService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "CompaniesShared", entityRoot : "CompanyEnvironment", processor : _companyService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "CompaniesShared", entityRoot : "CompanyEnvironmentHardware", processor : _companyService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "LinksShared", entityRoot : "Link", processor : _linkService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "FilesShared", entityRoot : "File", processor : _fileService ),
        new ProcessorConfig ( assembly : "CodeGenCSharpUpdaterCore", nameSpace : "CodeGenShared", entityRoot : "CodeGen", processor : _codeGenService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "EmploymentsShared", entityRoot : "Employment", processor : _employmentService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "ProjectRoleAssignmentsShared", entityRoot : "ProjectRoleAssignment", processor : _projectRoleAssignmentService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "ProductInstallationsShared", entityRoot : "ProductInstallation", processor : _productInstallationService )
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
    public IEnumerable<CommandReadOnlyDto> GetForGuid(string forGuid)
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
      try
      {
        var count = _commandManager.ProcessImportedCommands();
        PersistRepositories();

        result.Success = true;
        result.Message = $"{count} commands processed.";
        result.Count = count;
      }
      catch (Exception ex)
      {
        result.Message = ex.Message;
        result.Success = false;
      }

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
        _commandManager.ProcessCommands(commands);

        PersistRepositories();
        result.Count = commands.Count();
        result.Success = true;
      }
      catch (Exception ex)
      {
        result.Success = false;
        result.Message = ex.Message;
      }
    }

    private void PersistRepositories()
    {
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
    }
  }
}
