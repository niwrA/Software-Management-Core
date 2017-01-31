using DateTimeShared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandsShared
{
    public interface ICommandProcessor
    {
        // void ProcessCommand(ICommand command);
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

        public ICommand ProcessCommand(CommandDto command)
        {
            // todo: config to a centralized location or find a way to inject, or create new but inject repository only like now

            ICommandConfig commandConfig;
            IProcessorConfig config;
            ICommand typedCommand = null;
            ICommandProcessor processor = null;
            if (_commandConfigs.TryGetValue(command.Name + command.Entity + "Command", out commandConfig))
            {
                typedCommand = commandConfig.GetCommand(command.ParametersJson);
                processor = commandConfig.Processor;
            }
            else if (_configs.TryGetValue(command.Entity, out config))
            {
                typedCommand = config.GetCommand(command.Name, command.Entity, command.ParametersJson);
                processor = config.Processor;
            }
            if (typedCommand != null)
            {
                CopyCommandDtoIntoCommand(command, _repo, processor, typedCommand);

                typedCommand.Execute();
                typedCommand.ExecutedOn = _dateTimeProvider.GetUtcDateTime();

                return typedCommand;
            }

            throw new CommandNotConfiguredException($"The command named '{command.Name}' for entity '{command.Entity}' does not have a matching configuration.");
        }

        private void CopyCommandDtoIntoCommand(CommandDto command, ICommandStateRepository commandRepository, ICommandProcessor processor, ICommand typedCommand)
        {
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
