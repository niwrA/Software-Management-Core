using static CommandsShared.CommandManager; // todo have commanddto implement ICommand?

namespace CommandsShared
{
    public interface ICommandManager
    {
        void AddConfig(ICommandConfig config);
        void AddConfig(IProcessorConfig config);
        //void ProcessCommand(ICommand command);
        ICommand ProcessCommand(CommandDto command, ICommandRepository commandRepository);
    }
}