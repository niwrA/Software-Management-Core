using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Xunit;
using System.Threading;
using SoftwareManagementMongoDbCoreRepository;
using MongoDB.Driver;

namespace SoftwareManagementMongoDbCoreRepositoryTests
{
    [Trait("Entity", "DesignState")]
    public class DesignsTests
    {
        [Fact(DisplayName = "CreateDesignState")]
        public void CanCreateDesignState()
        {
            var sutBuilder = new SutBuilder();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();
            var name = "new design name";
            var state = sut.CreateDesignState(guid, name);

            Assert.Equal(guid, state.Guid);
            Assert.Equal(name, state.Name);
        }

        [Fact(DisplayName = "DeleteDesignState")]
        public void CanDeleteDesignState()
        {
            var sutBuilder = new SutBuilder();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();

            sut.DeleteDesignState(guid);
        }

        [Fact(DisplayName = "PersistCreatedDesignState")]
        public void WhenCreateDesignState_PersistChanges_InsertsIntoCollection()
        {
            var sutBuilder = new SutBuilder().WithDesignCollection();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();
            var name = "new design name";

            var state = sut.CreateDesignState(guid, name) as DesignState;

            sut.PersistChanges();

            sutBuilder.DesignStateCollection.Verify(s => s.InsertMany(
                It.Is<ICollection<DesignState>>(
                l => l.Contains(state) &&
                l.Count == 1)
                , null, CancellationToken.None), Times.Once, "InsertMany was not called correctly");
        }

        [Fact(DisplayName = "PersistDeletedDesignState")]
        public void WhenDeleteDesignState_PersistChanges_DeletesFromCollection()
        {
            var sutBuilder = new SutBuilder().WithDesignCollection();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();

            sut.DeleteDesignState(guid);

            sut.PersistChanges();

            sutBuilder.DesignStateCollection.Verify(s => s.DeleteOne(It.IsAny<FilterDefinition<DesignState>>(), CancellationToken.None), Times.Once, "DeleteOne was not called");
        }

    }
}
