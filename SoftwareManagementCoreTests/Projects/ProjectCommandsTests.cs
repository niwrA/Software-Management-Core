using CommandsShared;
using Moq;
using ProjectsShared;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Projects
{
    [Trait("Entity", "Project")]
    public class ProjectCommandsTests
    {
        [Fact(DisplayName = "DeleteProjectCommand")]
        public void DeleteCommand()
        {
            var commandsRepoMock = new Mock<ICommandRepository>();
            commandsRepoMock.Setup(s => s.Create()).Returns(new Fakes.CommandState());

            var projectsMock = new Mock<IProjectService>();
            var guid = Guid.NewGuid();
            var sut = new DeleteProjectCommand
            {
                CommandRepository = commandsRepoMock.Object,
                CommandProcessor = projectsMock.Object, EntityGuid = guid
            };

            sut.Execute();

            projectsMock.Verify(s => s.DeleteProject(guid));
        }
    }
}
