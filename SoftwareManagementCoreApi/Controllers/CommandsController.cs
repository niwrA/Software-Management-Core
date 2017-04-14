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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SoftwareManagementCoreWeb.Controllers
{
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

        private IEmploymentService _employmentService;
        private IProjectRoleAssignmentService _projectRoleAssignmentService;

        private ICommandService _commandManager;

        public CommandsController(ICommandService commandManager, IProductService productService, IProjectService projectService, IContactService contactService, IEmploymentService employmentService, ICompanyService companyService, IProjectRoleAssignmentService projectRoleAssignmentService, ILinkService linkService, IDesignService designService)
        {
            _commandManager = commandManager;

            _productService = productService;
            _designService = designService;
            _projectService = projectService;
            _contactService = contactService;
            _companyService = companyService;
            _linkService = linkService;

            _employmentService = employmentService;
            _projectRoleAssignmentService = projectRoleAssignmentService;

            ConfigureCommandManager();
        }

        private void ConfigureCommandManager()
        {
            var projectsConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProjectsShared", Entity = "Project", Processor = _projectService };
            var productsConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProductsShared", Entity = "Product", Processor = _productService };
            var designsConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "DesignsShared", Entity = "Design", Processor = _designService };
            var epicElementConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "DesignsShared", Entity = "EpicElement", Processor = _designService };
            var entityElementConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "DesignsShared", Entity = "EntityElement", Processor = _designService };
            var commandElementConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "DesignsShared", Entity = "CommandElement", Processor = _designService };
            var propertyElementConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "DesignsShared", Entity = "PropertyElement", Processor = _designService };
            var contactsConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ContactsShared", Entity = "Contact", Processor = _contactService };
            var companiesConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "CompaniesShared", Entity = "Company", Processor = _companyService };
            var environmentsConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "CompaniesShared", Entity = "Environment", Processor = _companyService };
            var linksConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "LinksShared", Entity = "Link", Processor = _linkService };

            var employmentsConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "EmploymentsShared", Entity = "Employment", Processor = _employmentService };
            var projectRoleAssignmentsConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProjectRoleAssignmentsShared", Entity = "ProjectRoleAssignment", Processor = _projectRoleAssignmentService };

            _commandManager.AddConfig(projectsConfig);
            _commandManager.AddConfig(productsConfig);
            _commandManager.AddConfig(designsConfig);
            _commandManager.AddConfig(epicElementConfig);
            _commandManager.AddConfig(entityElementConfig);
            _commandManager.AddConfig(commandElementConfig);
            _commandManager.AddConfig(propertyElementConfig);
            _commandManager.AddConfig(contactsConfig);
            _commandManager.AddConfig(companiesConfig);
            _commandManager.AddConfig(environmentsConfig);
            _commandManager.AddConfig(employmentsConfig);
            _commandManager.AddConfig(projectRoleAssignmentsConfig);
            _commandManager.AddConfig(linksConfig);
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
        public CommandBatchResults Post([FromBody]IEnumerable<CommandDto> commands)
        {
            // simple, direct execution of one or more commands, like a transaction
            var result = new CommandBatchResults();

            try
            {
                foreach (var command in commands)
                {
                    var typedCommand = _commandManager.ProcessCommand(command);
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
                _commandManager.PersistChanges();

                result.ExecutedCommands = commands;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
