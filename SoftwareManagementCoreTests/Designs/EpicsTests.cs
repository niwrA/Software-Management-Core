using DesignsShared;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Designs
{
    [Trait("Entity", "EpicElement")]
    public class EpicsTests
    {
        [Fact(DisplayName = "Can Rename EpicElement")]
        public void CanRenameDesign()
        {
            var stateMock = new Mock<IEpicElementState>();
            var repoMock = new Mock<IDesignStateRepository>();
            var sut = new EpicElement(stateMock.Object, repoMock.Object);

            sut.Rename("new", "old");

            stateMock.Setup(s => s.Name).Returns("old");
            sut.Rename("new", "old");
            stateMock.VerifySet(t => t.Name = "new");
        }

        [Fact(DisplayName = "Can Change Description of EpicElement")]
        public void CanChangeDescriptionOfEpicElement()
        {
            var stateMock = new Mock<IEpicElementState>();
            var repoMock = new Mock<IDesignStateRepository>();
            var sut = new EpicElement(stateMock.Object, repoMock.Object);

            sut.ChangeDescription("new");

            stateMock.VerifySet(t => t.Description = "new");
        }

    }
}
