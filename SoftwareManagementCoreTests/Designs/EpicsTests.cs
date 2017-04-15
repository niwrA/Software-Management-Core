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

        [Fact(DisplayName = "Can Add EntityElement to EpicElement")]
        public void CanAddEntityElement()
        {
            var repoMock = new Mock<IDesignStateRepository>();
            var stateMock = new Mock<IEpicElementState>();
            var sut = new EpicElement(stateMock.Object, repoMock.Object);
            var entityStateMock = new Mock<IEntityElementState>();
            const string name = "new";

            var epicGuid = sut.Guid;
            var designGuid = Guid.NewGuid();
            var entityGuid = Guid.NewGuid();

            stateMock.Setup(s => s.Guid).Returns(epicGuid);
            stateMock.Setup(s => s.DesignGuid).Returns(designGuid);

            entityStateMock.Setup(s => s.Guid).Returns(entityGuid);
            entityStateMock.Setup(s => s.EpicGuid).Returns(epicGuid);
            entityStateMock.Setup(s => s.DesignGuid).Returns(designGuid);
            entityStateMock.Setup(s => s.Name).Returns(name);

            repoMock.Setup(t => t.CreateEntityElementState(designGuid, epicGuid, entityGuid, name)).Returns(entityStateMock.Object);

            var sutResult = sut.AddEntityElement(entityGuid, name);

            Assert.Equal(entityGuid, sutResult.Guid);
            Assert.Equal(designGuid, sutResult.DesignGuid);
            Assert.Equal(epicGuid, sutResult.EpicGuid);
            Assert.Equal(name, sutResult.Name);
        }

    }
}
