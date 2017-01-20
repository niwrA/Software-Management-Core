using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CommandsShared;
using ProductsShared;
using Microsoft.AspNetCore.Authorization;
using static CommandsShared.CommandManager;
//using SoftwareManagementEFCoreRepository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SoftwareManagementCoreWeb.Controllers
{


    [Route("api/[controller]")]
    [AllowAnonymous]
    public class CommandsController : Controller
    {
        private IProductService _productService;
        private IProductStateRepository _productStateRepository;
        private ICommandRepository _commandRepository;
        private ICommandManager _commandManager;

        public CommandsController(IProductService productService, IProductStateRepository productStateRepository, ICommandRepository commandRepository, ICommandManager commandManager)
        {
            _productService = productService;
            _productStateRepository = productStateRepository;
            _commandRepository = commandRepository;
            _commandManager = commandManager;
        }
        // GET: api/commands
        [HttpGet]
        public IEnumerable<ICommandState> Get()
        {
            //var repository = new MainRepository(new MainContext());

            // return new List<ICommand> { new CreateProductCommand(repository), new RenameProductCommand(repository) };
            return new List<ICommandState>();
        }

        // POST api/commands
        //[HttpPost]
        //public void Post([FromBody]IEnumerable<ICommand> commands)
        //{
        //    // simple, direct execution like transaction
        //    // todo: save commands, then execute, for eventsourcing
        //    // _commandRepository.PersistChanges();

        //    // consider as transaction
        //    var commandManager = new CommandManager();
        //    var commandConfig = new CommandConfig { Name = "Create", ProcessorName = "Project", Processor = _productService };

        //    foreach (var command in commands)
        //    {
        //        commandManager.ProcessCommand(command);
        //        // get repository by command.CommandTypeId 
        //    }

        //    _productStateRepository.PersistChanges();
        //}
        // POST api/commands
        [HttpPost]
        [Route("batch")]
        public IEnumerable<CommandDto> Post([FromBody]IEnumerable<CommandDto> commands)
        {
            // simple, direct execution like transaction
            // todo: save commands, then execute, for eventsourcing
            // _commandRepository.PersistChanges();

            // consider as transaction
            // todo: inject commandmanager
            var commandConfig = new CommandConfig { Name = "Create", ProcessorName = "Product", Processor = _productService };
            _commandManager.AddConfig(commandConfig);
            var renameCommandConfig = new CommandConfig { Name = "Rename", ProcessorName = "Product", Processor = _productService };
            _commandManager.AddConfig(renameCommandConfig);

            foreach (var command in commands)
            {
                _commandManager.ProcessCommand(command, _commandRepository);
            }

            _productStateRepository.PersistChanges();

            return commands;
        }
    }
}
