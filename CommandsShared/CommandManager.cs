using DateTimeShared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandsShared
{
    public interface ICommandProcessor
    {
    }
    public class ProcessorConfig : IProcessorConfig
    {
        public ICommandProcessor Processor { get; set; }
        public string Entity { get; set; }
        public string NameSpace { get; set; }
        public string Assembly { get; set; }
        public ICommand GetCommand(string name, string entity, string parametersJson)
        {
            var commandConfig = new CommandConfig()
            {
                ProcessorName = entity,
                Name = name,
                NameSpace = this.NameSpace,
                Assembly = this.Assembly,
                Processor = this.Processor
            };
            return commandConfig.GetCommand(parametersJson);
        }
    }
    public class CommandConfig : ICommandConfig
    {
        public string Key { get { return Name + ProcessorName + "Command"; } }
        public string Name { get; set; }
        public string ProcessorName { get; set; }
        public string NameSpace { get; set; }
        public string Assembly { get; set; }
        public ICommandProcessor Processor { get; set; }
        public ICommand GetCommand(string json)
        {
            ICommand command;
            if (!string.IsNullOrWhiteSpace(json))
            {
                var insert = @"'$type': '" + NameSpace + "." + Key + @", " + Assembly + "', ";
                json = json.Trim().Insert(1, insert);
                command = JsonConvert.DeserializeObject<ICommand>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
            }
            else
            {
                Type type = Type.GetType(NameSpace + "." + Key + ", " + Assembly);
                if (type != null)
                {
                    command = Activator.CreateInstance(type) as ICommand;
                }
                else
                {
                    throw new TypeNotFoundException($"{Key} not found in {NameSpace} of {Assembly}");
                }
            }
            return command;
        }
    }

    // todo: move out CommandDto c.s. to separate project
    public class CommandDto
    {
        public Guid Guid { get; set; }
        public Guid EntityGuid { get; set; }
        public string Entity { get; set; }
        public string Name { get; set; }
        public string ParametersJson { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ExecutedOn { get; set; }
        //public long? ReceivedOn { get; set; }
        //public string UserName { get; set; }
    }
    public class CommandManager : ICommandManager
    {
        private Dictionary<string, IProcessorConfig> _configs = new Dictionary<string, IProcessorConfig>();
        private Dictionary<string, ICommandConfig> _commandConfigs = new Dictionary<string, ICommandConfig>();
        private ICommandStateRepository _repo;
        private IDateTimeProvider _dateTimeProvider;
        public CommandManager(ICommandStateRepository repo, IDateTimeProvider dateTimeProvider)
        {
            _repo = repo;
            _dateTimeProvider = dateTimeProvider;
        }
        public void AddConfig(ICommandConfig config)
        {
            _commandConfigs.Add(config.Key, config);
        }
        public void AddConfig(IProcessorConfig config)
        {
            _configs.Add(config.Entity, config);
        }

        public void PersistChanges()
        {
            _repo.PersistChanges();
        }
        public void MergeCommands(IEnumerable<CommandDto> commands)
        {
            // todo: replay should be done once for each unique entity, or we need to refresh
            // the existing entity for every command
            foreach (var command in commands)
            {
                // cheap solution. Should find a way to wrap the state here as well.
                var states = _repo.GetCommandStates(command.EntityGuid);//.Select(s => new CommandDto { Guid = s.Guid, Entity = command.Entity, EntityGuid = s.EntityGuid, Name = s.CommandTypeId.Replace(command.Entity + "Command", ""), ParametersJson = s.ParametersJson, CreatedOn = s.CreatedOn });
                foreach (var state in states)
                {
                    // these will always be the same Entity
                    ProcessCommand(command, state);
                }
                ProcessCommand(command);
            }
        }
        public ICommand ProcessCommand(CommandDto command)
        {
            return ProcessCommand(command, null);
        }
        private ICommand ProcessCommand(CommandDto command, ICommandState state)
        {
            ICommand typedCommand = null;
            ICommandProcessor processor = null;
            // either take existing name from state, or construct from dto
            // the replace is a bit hacky, should probably clean the commandstore
            var commandName = state == null ? command.Name  : state.CommandTypeId.Replace(command.Entity + "Command", "");
            var parametersJson = state == null ? command.ParametersJson : state.ParametersJson;
            if (_commandConfigs.TryGetValue(commandName + command.Entity + "Command", out ICommandConfig commandConfig))
            {
                typedCommand = commandConfig.GetCommand(parametersJson);
                processor = commandConfig.Processor;
            }
            else if (_configs.TryGetValue(command.Entity, out IProcessorConfig config))
            {
                typedCommand = config.GetCommand(commandName, command.Entity, parametersJson);
                processor = config.Processor;
            }
            if (typedCommand != null)
            {
                CopyCommandDtoIntoCommand(command, _repo, processor, typedCommand, state);

                typedCommand.Execute();
                typedCommand.ExecutedOn = _dateTimeProvider.GetUtcDateTime();

                return typedCommand;
            }

            throw new CommandNotConfiguredException($"The command named '{command.Name}' for entity '{command.Entity}' does not have a matching configuration.");
        }

        private void CopyCommandDtoIntoCommand(CommandDto command, ICommandStateRepository commandRepository, ICommandProcessor processor, ICommand typedCommand, ICommandState state)
        {
            ((CommandBase)typedCommand).State = state;
            typedCommand.CommandRepository = commandRepository;
            typedCommand.CreatedOn = command.CreatedOn;
            typedCommand.ReceivedOn = _dateTimeProvider.GetSessionUtcDateTime();
            typedCommand.EntityGuid = command.EntityGuid;
            typedCommand.Guid = command.Guid;
            typedCommand.ParametersJson = command.ParametersJson;
            typedCommand.CommandProcessor = processor;
        }
    }
}
