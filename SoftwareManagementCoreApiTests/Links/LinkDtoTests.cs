using SoftwareManagementCoreApi.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
namespace SoftwareManagementCoreApiTests.Links
{
    [Trait("Entity", "LinkDto")]
    public class LinkDtoTests
    {
        [Fact(DisplayName = "LinkDtoMapsState")]
        public void LinkDtoMapsLinkState()
        {
            var fakeState = new Fakes.LinkState();
            var sut = new LinkDto(fakeState);
            Assert.Equal(fakeState.CreatedOn.ToString("yyyy-MM-dd"), sut.CreatedOn);
            Assert.Equal(fakeState.Description, sut.Description);
            Assert.Equal(fakeState.EntityGuid, sut.EntityGuid);
            Assert.Equal(fakeState.ForGuid, sut.LinkForGuid);
            Assert.Equal(fakeState.Guid, sut.Guid);
            Assert.Equal(fakeState.ImageUrl, sut.ImageUrl);
            Assert.Equal(fakeState.Name, sut.Name);
            Assert.Equal(fakeState.UpdatedOn.ToString("yyyy-MM-dd"), sut.UpdatedOn);
            Assert.Equal(fakeState.Url, sut.Url);
        }
    }
}
