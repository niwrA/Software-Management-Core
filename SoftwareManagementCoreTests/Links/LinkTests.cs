using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Xunit;
using LinksShared;

namespace SoftwareManagementCoreTests.Links
{
    [Trait("Entity","Link")]
    public class LinkTests
    {
        [Fact]
        public void ChangeUrlImplementsInterfaces()
        {
            var linkDetailsMock = new Mock<ILinkDetailsProcessor>();
            var stateFake = new Fakes.LinkState();
            var repoMock = new Mock<ILinkStateRepository>();
            var sut = new Link(stateFake, repoMock.Object, linkDetailsMock.Object);

            var linkDetails = new Fakes.LinkDetails();

            linkDetailsMock.Setup(s => s.ProcessLinkDetails("new url")).Returns(linkDetails);

            sut.ChangeUrl("new url", stateFake.Url);

            Assert.Equal(linkDetails.Description, sut.Description);
            Assert.Equal(linkDetails.Title, sut.Name);
            Assert.Equal(linkDetails.SiteName, sut.SiteName);
            Assert.Equal(linkDetails.Image.Url, sut.ImageUrl);

        }
    }
}
