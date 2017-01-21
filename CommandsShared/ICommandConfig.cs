namespace CommandsShared
{
    public interface ICommandConfig
    {
        string Key { get; }
        string Name { get; set; }
        ICommand GetCommand(string parametersJson);
        ICommandProcessor Processor { get; set; }
        string ProcessorName { get; set; }
        string NameSpace { get; set; }
        string Assembly { get; set; }

    }

    public interface IProcessorConfig
    {
        ICommandProcessor Processor { get; set; }
        string Entity { get; set; }
        string NameSpace { get; set; }
        string Assembly { get; set; }
        ICommand GetCommand(string name, string entity, string parametersJson);

    }
}