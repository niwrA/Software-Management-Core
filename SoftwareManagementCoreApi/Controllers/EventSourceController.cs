using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using niwrA.CommandManager;
using ProductsShared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using ProjectsShared;
using ContactsShared;
using CompaniesShared;
using SoftwareManagementEventSourceRepository;
using DateTimeShared;
using niwrA.CommandManager.Contracts;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SoftwareManagementCoreWeb.Controllers
{
  [EnableCors("SiteCorsPolicy")]
  [Route("api/[controller]")]
  [AllowAnonymous]
  public class EventSourceController : Controller
  {
    private IProductService _productService;
    private IProjectService _projectService;
    private IContactService _contactService;
    private ICompanyService _companyService;
    private ICommandManager _commandManager;
    private IContactStateRepository _contactStateRepository;
    private ICompanyStateRepository _companyStateRepository;
    private IProjectStateRepository _projectStateRepository;
    private IProductStateRepository _productStateRepository;

    public EventSourceController(ICommandManager commandManager)
    {
      _commandManager = commandManager;

      // set this one to eventsourced for testing

      // you would normally do this in DI, but we're supporting different
      // implementations here side-by-side for demonstration purposes
      _companyStateRepository = new EventSourcedMainRepository();
      _companyService = new CompanyService(_companyStateRepository, new DateTimeProvider());
      _contactStateRepository = new EventSourcedMainRepository();
      _contactService = new ContactService(_contactStateRepository, new DateTimeProvider());
      _projectStateRepository = new EventSourcedMainRepository();
      _projectService = new ProjectService(_projectStateRepository, new DateTimeProvider());
      _productStateRepository = new EventSourcedMainRepository();
      _productService = new ProductService(_productStateRepository, new DateTimeProvider());
      ConfigureCommandManager();
    }

    private void ConfigureCommandManager()
    {
      var processorConfigs = new List<IProcessorConfig>
      {
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "ProjectsShared", entityRoot : "Project", processor : _projectService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "ProductsShared", entityRoot : "Product", processor : _productService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "ContactsShared", entityRoot : "Contact", processor : _contactService ),
        new ProcessorConfig ( assembly : "SoftwareManagementCore", nameSpace : "CompaniesShared", entityRoot : "Company", processor : _companyService )
      };

      _commandManager.AddProcessorConfigs(processorConfigs);
    }

    // GET: api/commands
    [HttpGet]
    public IEnumerable<ICommandState> Get()
    {
      return new List<ICommandState>();
    }

    // todo: this is a proof of concept.
    // is this only for validation?
    // POST api/commands/batch
    [HttpPost]
    [Route("batch")]
    public IEnumerable<CommandDto> Post([FromBody]IEnumerable<CommandDto> commands)
    {
      _commandManager.MergeCommands(commands);
      // if the commands were accepted, or always?
      _commandManager.PersistChanges();
      // todo command dtos should be updated from commands
      return commands;
    }
  }
}
