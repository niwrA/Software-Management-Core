using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CommandsShared;
using ProductsShared;
using Microsoft.AspNetCore.Authorization;
using static CommandsShared.CommandManager;
using Microsoft.AspNetCore.Cors;
using ProjectsShared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SoftwareManagementCoreWeb.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class CommandsController : Controller
    {
        private IProductService _productService;
        private IProductStateRepository _productStateRepository;
        private IProjectService _projectService;
        private IProjectStateRepository _projectStateRepository;
        private ICommandRepository _commandRepository;
        private ICommandManager _commandManager;

        public CommandsController(ICommandRepository commandRepository, ICommandManager commandManager, IProductService productService, IProductStateRepository productStateRepository, IProjectStateRepository projectStateRepository, IProjectService projectService)
        {
            _commandRepository = commandRepository;
            _commandManager = commandManager;

            _productService = productService;
            _productStateRepository = productStateRepository;
            _projectService = projectService;
            _projectStateRepository = projectStateRepository;

            // todo: move to configuration
            ConfigureCommandManager();
        }

        private void ConfigureCommandManager()
        {
            var projectsConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProjectsShared", Entity = "Project", Processor = _projectService };
            var productsConfig = new ProcessorConfig { Assembly = "SoftwareManagementCore", NameSpace = "ProductsShared", Entity = "Product", Processor = _productService };

            _commandManager.AddConfig(projectsConfig);
            _commandManager.AddConfig(productsConfig);
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
                var typedCommand = _commandManager.ProcessCommand(command, _commandRepository);
                command.ExecutedOn = typedCommand.ExecutedOn;
            }

            // these can be all the same contexts, but may also be different
            _productStateRepository.PersistChanges();   
            _projectStateRepository.PersistChanges();
            _commandRepository.PersistChanges();
            // todo command dtos should be updated from commands
            return commands;
        }
    }
}
