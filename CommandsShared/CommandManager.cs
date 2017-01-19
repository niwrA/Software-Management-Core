using System;
using System.Collections.Generic;
using System.Text;

namespace CommandsShared
{
    public interface ICommandProcessor
    {
       // void ProcessCommand(ICommand command);
    }
    public class CommandConfig: ICommandConfig
    {
        public string Key { get { return ProcessorName + '.' + Name; } }
        public string Name { get; set; }
        public string ProcessorName { get; set; }
        public ICommandProcessor Processor { get; set; }
    }
    public class CommandManager: ICommandManager
    {
        private Dictionary<string, ICommandConfig> _configs = new Dictionary<string, ICommandConfig>();
        public void AddConfig(ICommandConfig config)
        {
            _configs.Add(config.Key, config);
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
