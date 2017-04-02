using CommandsShared;
using Moq;
using LinksShared;
using SoftwareManagementCoreTests.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Links
{
    [Trait("Entity", "Link")]
    public class CompanyCommandsTests
    {
        [Fact(DisplayName = "CreateLinkCommand")]
        public void CreateCommand()
        {
            var linksMock = new Mock<ILinkService>();
            var sut = new CommandBuilder<CreateLinkCommand>().Build(linksMock.Object) as CreateLinkCommand;

            sut.Name = "New Link";
            sut.Url = "http://somewhere.nice";
            sut.LinkForGuid = Guid.NewGuid();
            sut.Execute();

            linksMock.Verify(s => s.CreateLink(sut.EntityGuid, sut.LinkForGuid, sut.Url, sut.Name), Times.Once);
        }

        [Fact(DisplayName = "DeleteLinkCommand")]
        public void DeleteCommand()
        {
            var linksMock = new Mock<ILinkService>();
            var sut = new CommandBuilder<DeleteLinkCommand>().Build(linksMock.Object) as DeleteLinkCommand;

            sut.Execute();

            linksMock.Verify(s => s.DeleteLink(sut.EntityGuid), Times.Once);
        }

        [Fact(DisplayName = "RenameLinkCommand")]
        public void RenameCommand()
        {
            var sutBuilder = new LinkCommandBuilder<RenameLinkCommand>();
            var sut = sutBuilder.Build() as RenameLinkCommand;

            sut.Name = "New Name";
            sut.OriginalName = "Original Name";
            sut.Execute();

            sutBuilder.LinkMock.Verify(s => s.Rename(sut.Name, sut.OriginalName), Times.Once);
        }

        [Fact(DisplayName = "ChangeUrlForLinkCommand")]
        public void ChangeUrlForLinkCommand()
        {
            var sutBuilder = new LinkCommandBuilder<ChangeUrlForLinkCommand>();
            var sut = sutBuilder.Build() as ChangeUrlForLinkCommand;

            sut.Url = "New";
            sut.OriginalUrl = "Original";
            sut.Execute();

            sutBuilder.LinkMock.Verify(s => s.ChangeUrl(sut.Url, sut.OriginalUrl), Times.Once);
        }
    }

    class LinkCommandBuilder<T> where T : ICommand, new()
    {
        public Mock<ILink> LinkMock { get; set; }
        public ICommand Build()
        {
            var linksMock = new Mock<ILinkService>();
            var linkMock = new Mock<ILink>();
            this.LinkMock = linkMock;

            var sut = new CommandBuilder<T>().Build(linksMock.Object);

            linksMock.Setup(s => s.GetLink(sut.EntityGuid)).Returns(linkMock.Object);

            return sut;
        }
    }
}
