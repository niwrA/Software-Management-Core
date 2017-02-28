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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SoftwareManagementCoreWeb.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class CommandsController : Controller
    {
        private IProductService _productService;
        private IProjectService _projectService;
        private IContactService _contactService;
        private ICompanyService _companyService;
        private IEmploymentService _employmentService;

        private ICommandService _commandManager;

        public CommandsController(ICommandService commandManager, IProductService productService, IProjectService projectService, IContactService contactService, IEmploymentService employmentService, ICompanyService companyService)
        {
            _commandManager = commandManager;

            _productService = productService;
            _projectService = projectService;
            _contactService = contactService;
            _companyService = companyService;
            _employmentService = employmentService;

            // todo: move to configuration
            ConfigureCommandManager();
        }

        private void ConfigureCommandManager()
        {
            var projectsConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProjectsShared", Entity = "Project", Processor = _projectService };
            var productsConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProductsShared", Entity = "Product", Processor = _productService };
            var contactsConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ContactsShared", Entity = "Contact", Processor = _contactService };
            var companiesConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "CompaniesShared", Entity = "Company", Processor = _companyService };
            var employmentsConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "EmploymentsShared", Entity = "Employment", Processor = _employmentService };

            _commandManager.AddConfig(projectsConfig);
            _commandManager.AddConfig(productsConfig);
            _commandManager.AddConfig(contactsConfig);
            _commandManager.AddConfig(companiesConfig);
            _commandManager.AddConfig(employmentsConfig);
        }

        // GET: api/commands
        [HttpGet]
        public IEnumerable<ICommandState> Get()
        {
            return new List<ICommandState>();
        }

        // POST api/commands/batch
        [HttpPost]
        [Route("batch")]
        public IEnumerable<CommandDto> Post([FromBody]IEnumerable<CommandDto> commands)
        {
            // simple, direct execution of one or more commands, like a transaction

            foreach (var command in commands)
            {
                var typedCommand = _commandManager.ProcessCommand(command);
                command.ExecutedOn = typedCommand.ExecutedOn;
            }

            // these can be all the same contexts, but may also be different
            _productService.PersistChanges();   
            _projectService.PersistChanges();
            _contactService.PersistChanges();
            _companyService.PersistChanges();
            _employmentService.PersistChanges();
            _commandManager.PersistChanges();

            return commands;
        }
    }
}
