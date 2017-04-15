using DesignsShared;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Designs
{
    [Trait("Entity", "CommandElement")]
    public class CommandsTests
    {
        [Fact(DisplayName = "Can Rename CommandElement")]
        public void CanRenameCommand()
        {
            var stateMock = new Mock<ICommandElementState>();
            var repoMock = new Mock<IDesignStateRepository>();
            var sut = new CommandElement(stateMock.Object, repoMock.Object);

            sut.Rename("new", "old");

            stateMock.Setup(s => s.Name).Returns("old");
            sut.Rename("new", "old");
            stateMock.VerifySet(t => t.Name = "new");
        }

        [Fact(DisplayName = "Can Change Description of CommandElement")]
        public void CanChangeDescriptionOfEpicElement()
        {
            var stateMock = new Mock<ICommandElementState>();
            var repoMock = new Mock<IDesignStateRepository>();
            var sut = new CommandElement(stateMock.Object, repoMock.Object);

            sut.ChangeDescription("new");

            stateMock.VerifySet(t => t.Description = "new");
        }

    }
}
