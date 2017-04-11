using DesignsShared;
using System;
using Xunit;
using Moq;
using DateTimeShared;

namespace SoftwareManagementCoreTests
{
    [Trait("Entity", "Design")]
    public class DesignsTests
    {
        [Fact(DisplayName = "Create implements IRepository")]
        public void CreateDesign_ImplementsIRepository()
        {
            var repoMock = new Moq.Mock<IDesignStateRepository>();
            var sut = new DesignService(repoMock.Object, new DateTimeProvider());
            var stateMock = new Mock<IDesignState>();

            repoMock.Setup(t => t.CreateDesignState(It.IsAny<Guid>(), It.IsAny<string>())).Returns(stateMock.Object);

            var guid = Guid.NewGuid();
            var name = "New Design";
            sut.CreateDesign(guid, name);

            repoMock.Verify(s => s.CreateDesignState(guid, name), Times.Once);
        }

        [Fact(DisplayName = "Get Implements IRepository")]
        public void CanGetDesign()
        {
            var repoMock = new Mock<IDesignStateRepository>();
            var sut = new DesignService(repoMock.Object, new DateTimeProvider());
            var stateMock = new Mock<IDesignState>();
            var stateMockAlt = new Mock<IDesignState>();

            var stateGuid = Guid.NewGuid();
            var altStateGuid = Guid.NewGuid();

            repoMock.Setup(t => t.GetDesignState(stateGuid)).Returns(stateMock.Object);
            repoMock.Setup(t => t.GetDesignState(altStateGuid)).Returns(stateMockAlt.Object);

            sut.GetDesign(stateGuid);
            repoMock.Verify(t => t.GetDesignState(stateGuid));

            sut.GetDesign(altStateGuid);
            repoMock.Verify(t => t.GetDesignState(altStateGuid));
        }

        [Fact(DisplayName = "Get Implements IRepository")]
        public void CanRenameDesign()
        {
            var stateMock = new Mock<IDesignState>();
            var repoMock = new Mock<IDesignStateRepository>();
            var sut = new Design(stateMock.Object, repoMock.Object);

            sut.Rename("new", "old");

            stateMock.Setup(s => s.Name).Returns("old");
            sut.Rename("new", "old");
            stateMock.VerifySet(t => t.Name = "new");
        }

        [Fact(DisplayName = "AddEpic Implements IRepository")]
        public void CanAddRootElement()
        {
            var repoMock = new Mock<IDesignStateRepository>();
            var designStateMock = new Mock<IDesignState>();
            var sut = new Design(designStateMock.Object, repoMock.Object);
            var stateMock = new Mock<IEpicElementState>();
            const string name = "new";

            var guid = Guid.NewGuid();
            var designGuid = Guid.NewGuid();

            designStateMock.Setup(s => s.Guid).Returns(designGuid);
            repoMock.Setup(t => t.CreateEpicElementState(designGuid, guid, name)).Returns(stateMock.Object);

            var result = sut.AddEpic(guid, name);

            stateMock.VerifySet(t => t.ParentGuid = designGuid);
        }

    }
}
