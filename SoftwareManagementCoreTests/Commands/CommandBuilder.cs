using niwrA.CommandManager;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using niwrA.CommandManager.Repositories;

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

      cmd.EntityGuid = Guid.NewGuid();
      cmd.EntityRootGuid = Guid.NewGuid();
      cmd.CommandProcessor = processor;

      return (T)cmd;
    }
  }

}
