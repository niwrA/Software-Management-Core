using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CommandsShared
{
    public interface ICommands
    {
        ObservableCollection<ICommand> PostedCommands { get; set; }
        ICommandRepository Repository { get; }

        IEnumerable<ICommand> GetProcessedCommands();
        void PostCommand(ICommand command);
        void ProcessCommands(IList<ICommand> commands);
    }
}