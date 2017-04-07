using LinksShared;
using System;
using Xunit;
using Moq;
using DateTimeShared;

namespace SoftwareManagementCoreTests
{
    [Trait("Entity", "Link")]
    public class LinksTests
    {
        [Fact(DisplayName = "Create")]
        public void CreateLink_ImplementsIRepository()
        {
            var repoMock = new Moq.Mock<ILinkStateRepository>();
            var linkDetailsProcessorMock = new Mock<ILinkDetailsProcessor>();
            var sut = new LinkService(repoMock.Object, new DateTimeProvider(), linkDetailsProcessorMock.Object);
            var stateMock = new Mock<ILinkState>();
            var linkDetailsMock = new Mock<ILinkDetails>();

            repoMock.Setup(t => t.CreateLinkState(It.IsAny<Guid>(), It.IsAny<string>())).Returns(stateMock.Object);
            linkDetailsProcessorMock.Setup(s => s.ProcessLinkDetails(It.IsAny<string>())).Returns(linkDetailsMock.Object);

            var guid = Guid.NewGuid();
            var name = "New Link";
            var forGuid = Guid.NewGuid();
            var url = "http://someplace.unsafe";
            sut.CreateLink(guid, forGuid, url, name);

            repoMock.Verify(s => s.CreateLinkState(guid, name), Times.Once);
            linkDetailsProcessorMock.Verify(s => s.ProcessLinkDetails(It.IsAny<string>()), Times.Once);
        }

        [Fact(DisplayName = "Delete")]
        public void CanDeleteLink()
        {
            var repoMock = new Mock<ILinkStateRepository>();
            var linkDetailsMock = new Mock<ILinkDetailsProcessor>();
            var sut = new LinkService(repoMock.Object, new DateTimeProvider(), linkDetailsMock.Object);
            var guid = Guid.NewGuid();

            sut.DeleteLink(guid);

            repoMock.Verify(s => s.DeleteLinkState(guid));
        }

        [Fact(DisplayName = "Get")]
        public void CanGetLink()
        {
            var repoMock = new Mock<ILinkStateRepository>();
            var linkDetailsMock = new Mock<ILinkDetailsProcessor>();
            var sut = new LinkService(repoMock.Object, new DateTimeProvider(), linkDetailsMock.Object);
            var stateMock = new Mock<ILinkState>();
            var stateMockAlt = new Mock<ILinkState>();

            var stateGuid = Guid.NewGuid();
            var altStateGuid = Guid.NewGuid();

            repoMock.Setup(t => t.GetLinkState(stateGuid)).Returns(stateMock.Object);
            repoMock.Setup(t => t.GetLinkState(altStateGuid)).Returns(stateMockAlt.Object);

            sut.GetLink(stateGuid);
            repoMock.Verify(t => t.GetLinkState(stateGuid));

            sut.GetLink(altStateGuid);
            repoMock.Verify(t => t.GetLinkState(altStateGuid));
        }

        [Fact(DisplayName = "Rename")]
        public void CanRenameLink()
        {
            var stateMock = new Mock<ILinkState>();
            var sut = new Link(stateMock.Object);

            stateMock.Setup(s => s.Name).Returns("old");

            sut.Rename("new", "old");

            stateMock.VerifySet(t => t.Name = "new");
        }
    }
    public class LinkServiceSutBuilder
    {
        public Mock<ILinkStateRepository> RepoMock { get; set; }
        public Mock<ILinkDetailsProcessor> LinkDetailsMock { get; set; }
        public LinkService Build()
        {
            var RepoMock = new Mock<ILinkStateRepository>();
            var LinkDetailsMock = new Mock<ILinkDetailsProcessor>();
            var sut = new LinkService(RepoMock.Object, new DateTimeProvider(), LinkDetailsMock.Object);

            return sut;
        }
    }
}
