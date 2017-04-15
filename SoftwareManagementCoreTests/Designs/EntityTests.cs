using DesignsShared;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Designs
{
    [Trait("Entity", "EntityElement")]
    public class EntityTests
    {
        [Fact(DisplayName = "Can Rename EntityElement")]
        public void CanRenameEntityElement()
        {
            var stateMock = new Mock<IEntityElementState>();
            var repoMock = new Mock<IDesignStateRepository>();
            var sut = new EntityElement(stateMock.Object, repoMock.Object);

            sut.Rename("new", "old");

            stateMock.Setup(s => s.Name).Returns("old");
            sut.Rename("new", "old");
            stateMock.VerifySet(t => t.Name = "new");
        }

        [Fact(DisplayName = "Can Change Description of EntityElement")]
        public void CanChangeDescriptionOfEntityElement()
        {
            var stateMock = new Mock<IEntityElementState>();
            var repoMock = new Mock<IDesignStateRepository>();
            var sut = new EntityElement(stateMock.Object, repoMock.Object);

            sut.ChangeDescription("new");

            stateMock.VerifySet(t => t.Description = "new");
        }

        [Fact(DisplayName = "Can Add CommandElement to EntityElement")]
        public void CanAddCommandElement()
        {
            var repoMock = new Mock<IDesignStateRepository>();
            var stateMock = new Mock<IEntityElementState>();
            var epicStateMock = new Mock<IEpicElementState>();
            var commandStateMock = new Mock<ICommandElementState>();
            var sut = new EntityElement(stateMock.Object, repoMock.Object);
            const string name = "new";

            var epicGuid = Guid.NewGuid();
            var entityGuid = sut.Guid;
            var designGuid = Guid.NewGuid();
            var commandGuid = Guid.NewGuid();

            stateMock.Setup(s => s.Guid).Returns(entityGuid);
            stateMock.Setup(s => s.DesignGuid).Returns(designGuid);
            stateMock.Setup(s => s.EpicElementGuid).Returns(epicGuid);

            commandStateMock.Setup(s => s.Guid).Returns(commandGuid);
            commandStateMock.Setup(s => s.EntityElementGuid).Returns(entityGuid);
            commandStateMock.Setup(s => s.EpicElementGuid).Returns(epicGuid);
            commandStateMock.Setup(s => s.DesignGuid).Returns(designGuid);
            commandStateMock.Setup(s => s.Name).Returns(name);

            repoMock.Setup(t => t.CreateCommandElementState(designGuid, epicGuid, entityGuid, commandGuid, name)).Returns(commandStateMock.Object);

            var sutResult = sut.AddCommandElement(commandGuid, name);

            Assert.Equal(commandGuid, sutResult.Guid);
            Assert.Equal(entityGuid, sutResult.EntityGuid);
            Assert.Equal(designGuid, sutResult.DesignGuid);
            Assert.Equal(epicGuid, sutResult.EpicGuid);
            Assert.Equal(name, sutResult.Name);
        }

        [Fact(DisplayName = "Can Add PropertyElement to EntityElement")]
        public void CanAddPropertyElement()
        {
            var repoMock = new Mock<IDesignStateRepository>();
            var stateMock = new Mock<IEntityElementState>();
            var epicStateMock = new Mock<IEpicElementState>();
            var propertyStateMock = new Mock<IPropertyElementState>();
            var sut = new EntityElement(stateMock.Object, repoMock.Object);
            const string name = "new";

            var epicGuid = Guid.NewGuid();
            var entityGuid = sut.Guid;
            var designGuid = Guid.NewGuid();
            var propertyGuid = Guid.NewGuid();

            stateMock.Setup(s => s.Guid).Returns(entityGuid);
            stateMock.Setup(s => s.DesignGuid).Returns(designGuid);
            stateMock.Setup(s => s.EpicElementGuid).Returns(epicGuid);

            propertyStateMock.Setup(s => s.Guid).Returns(propertyGuid);
            propertyStateMock.Setup(s => s.EntityElementGuid).Returns(entityGuid);
            propertyStateMock.Setup(s => s.EpicElementGuid).Returns(epicGuid);
            propertyStateMock.Setup(s => s.DesignGuid).Returns(designGuid);
            propertyStateMock.Setup(s => s.Name).Returns(name);

            repoMock.Setup(t => t.CreatePropertyElementState(designGuid, epicGuid, entityGuid, propertyGuid, name)).Returns(propertyStateMock.Object);

            var sutResult = sut.AddPropertyElement(propertyGuid, name);

            Assert.Equal(propertyGuid, sutResult.Guid);
            Assert.Equal(entityGuid, sutResult.EntityGuid);
            Assert.Equal(designGuid, sutResult.DesignGuid);
            Assert.Equal(epicGuid, sutResult.EpicGuid);
            Assert.Equal(name, sutResult.Name);
        }

    }
}
