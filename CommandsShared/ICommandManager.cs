namespace CommandsShared
{
    public interface ICommandManager
    {
        void AddConfig(ICommandConfig config);
        void ProcessCommand(ICommand command);
    }
}