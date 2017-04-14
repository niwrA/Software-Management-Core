using SoftwareManagementCoreApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreApiTests.Designs
{
    [Trait("Entity", "DesignDto")]
    public class DesignDtoTests
    {
        [Fact(DisplayName = "DesignDtoMapsState")]
        public void DesignDtoMapsDesignState()
        {
            var fakeState = new Fakes.DesignState();
            var sut = new DesignDto(fakeState);

            Assert.Equal(fakeState.Guid, sut.Guid);
            Assert.Equal(fakeState.Name, sut.Name);
            Assert.Equal(fakeState.Description, sut.Description);
            Assert.Equal(fakeState.CreatedOn.ToString("yyyy-MM-dd"), sut.CreatedOn);
            Assert.Equal(fakeState.UpdatedOn.ToString("yyyy-MM-dd"), sut.UpdatedOn);
            Assert.Equal(fakeState.EpicElementStates.Count, sut.EpicElements.Count);
            Assert.Equal(fakeState.EpicElementStates.First().EntityElementStates.Count, sut.EpicElements.First().EntityElements.Count);
            Assert.Equal(fakeState.EpicElementStates.First().EntityElementStates.First().PropertyElementStates.Count, sut.EpicElements.First().EntityElements.First().PropertyElements.Count);
            Assert.Equal(fakeState.EpicElementStates.First().EntityElementStates.First().CommandElementStates.Count, sut.EpicElements.First().EntityElements.First().CommandElements.Count);
        }
    }

}
