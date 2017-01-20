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
    public class CommandConfig : ICommandConfig
    {
        public string Key { get { return Name + ProcessorName + "Command"; } }
        public string Name { get; set; }
        public string ProcessorName { get; set; }
        public ICommandProcessor Processor { get; set; }
        public ICommand GetCommand(string json)
        {
            ICommand command;
            if (!string.IsNullOrWhiteSpace(json))
            {
                var insert = @"'$type': 'ProductsShared." + Key + @", SoftwareManagementCore', ";
                json = json.Trim().Insert(1, insert);
                command = JsonConvert.DeserializeObject<ICommand>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

            }
            else
            {
                Type type = Type.GetType("ProductsShared." + Key + ", SoftwareManagementCore");
                command = Activator.CreateInstance(type) as ICommand;
            }
            return command;
        }
    }
    public class CommandDto
    {
        public Guid Guid { get; set; }
        public Guid EntityGuid { get; set; }
        public string Entity { get; set; }
        public string Name { get; set; }
        public string ParametersJson { get; set; }
        public DateTime CreatedOn { get; set; }
        // todo: add user created date
        //public long? ExecutedOn { get; set; }
        //public long? ReceivedOn { get; set; }
        //public string UserName { get; set; }
    }
    public class CommandManager : ICommandManager
    {
        private Dictionary<string, ICommandConfig> _configs = new Dictionary<string, ICommandConfig>();
        private ICommandRepository _repo;
        private IDateTimeProvider _dateTimeProvider;
        public CommandManager(ICommandRepository repo, IDateTimeProvider dateTimeProvider)
        {
            _repo = repo;
            _dateTimeProvider = dateTimeProvider;
        }
        public void AddConfig(ICommandConfig config)
        {
            _configs.Add(config.Key, config);
        }
        public ICommand ProcessCommand(CommandDto command, ICommandRepository commandRepository)
        {
            // todo: config to a centralized location or find a way to inject, or create new but inject repository only like now

            ICommandConfig config;
            if (_configs.TryGetValue(command.Name + command.Entity + "Command", out config))
            {
                var typedCommand = config.GetCommand(command.ParametersJson);

                typedCommand.CommandRepository = commandRepository;
                typedCommand.CreatedOn = command.CreatedOn;
                typedCommand.ReceivedOn = _dateTimeProvider.GetSessionUtcDateTime();
                typedCommand.EntityGuid = command.EntityGuid;
                typedCommand.Guid = command.Guid;
                typedCommand.ParametersJson = command.ParametersJson;
                typedCommand.CommandProcessor = config.Processor;
                typedCommand.Execute();
                typedCommand.ExecutedOn = _dateTimeProvider.GetUtcDateTime();
                return typedCommand;
            }
            return null; // todo: handle as command not found error
        }

        public void ProcessCommand(ICommand command)
        {
            ICommandConfig config;
            if (_configs.TryGetValue(command.CommandTypeId, out config))
            {
                command.CommandProcessor = config.Processor;
                command.Execute();
            }
        }
    }
}
