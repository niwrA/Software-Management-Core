using niwrA.CommandManager;
using Moq;
using System;
using niwrA.CommandManager.Contracts;

namespace SoftwareManagementCoreTests.Commands
{
    public class CommandBuilder<T> where T : ICommand, new()
  {
    public T Build(ICommandProcessor processor)
    {
      var commandRepoMock = new Mock<ICommandStateRepository>();
      var dateTimeProviderMock = new Mock<IDateTimeProvider>();
      var commandService = new CommandService(commandRepoMock.Object, dateTimeProviderMock.Object);

      dateTimeProviderMock.Setup(s => s.GetServerDateTime()).Returns(new DateTime(2017, 1, 1));
      commandRepoMock.Setup(s => s.CreateCommandState(It.IsAny<Guid>())).Returns(new Fakes.CommandState());

      var cmd = commandService.CreateCommand<T>();

      cmd.EntityGuid = Guid.NewGuid().ToString();
      cmd.EntityRootGuid = Guid.NewGuid().ToString();
      cmd.CommandProcessor = processor;

      return (T)cmd;
    }
  }
}
