namespace CommandsShared
{
    public interface ICommandConfig
    {
        string Key { get; }
        string Name { get; set; }
        ICommandProcessor Processor { get; set; }
        string ProcessorName { get; set; }
    }
}