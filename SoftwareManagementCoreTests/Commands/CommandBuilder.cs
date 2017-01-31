using CommandsShared;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareManagementCoreTests.Commands
{
    public class CommandBuilder<T> where T : ICommand, new()
    {
        public ICommand Build(ICommandProcessor processor)
        {
            var commandRepoMock = new Mock<ICommandStateRepository>();
            var commandState = new Fakes.CommandState();

            commandRepoMock.Setup(t => t.CreateCommandState()).Returns(commandState);

            ICommand cmd = new T()
            {
                CommandRepository = commandRepoMock.Object,
                EntityGuid = Guid.NewGuid(),
                CommandProcessor = processor
            };
            return cmd;
        }
    }

}
