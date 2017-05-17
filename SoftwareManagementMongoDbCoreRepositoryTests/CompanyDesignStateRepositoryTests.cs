using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Xunit;
using System.Threading;
using SoftwareManagementMongoDbCoreRepository;
using MongoDB.Driver;
using System.Linq;
using DesignsShared;

namespace SoftwareManagementMongoDbCoreRepositoryTests
{
    [Trait("Entity", "DesignState")]
    public class CompanyDesignStateRepositoryTests
    {
        [Fact(DisplayName = "CreateDesignState")]
        public void CanCreateDesignState()
        {
            var sutBuilder = new DesignRepositorySutBuilder();
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
            var sutBuilder = new DesignRepositorySutBuilder();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();

            sut.DeleteDesignState(guid);
        }

        [Fact(DisplayName = "PersistCreatedDesignState")]
        public void WhenCreateDesignState_PersistChanges_InsertsIntoCollection()
        {
            var sutBuilder = new DesignRepositorySutBuilder().WithDesignCollection();
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
            var sutBuilder = new DesignRepositorySutBuilder().WithDesignCollection();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();

            sut.DeleteDesignState(guid);

            sut.PersistChanges();

            sutBuilder.DesignStateCollection.Verify(s => s.DeleteOne(It.IsAny<FilterDefinition<DesignState>>(), CancellationToken.None), Times.Once, "DeleteOne was not called");
        }

        [Fact(DisplayName = "AddEpicElementState")]
        public void CanAddEpicElementState()
        {
            var sutBuilder = new DesignRepositorySutBuilder().WithDesignCollection();
            var sut = sutBuilder.Build();
            var guid = Guid.NewGuid();
            var epicGuid = Guid.NewGuid();
            var designState = (DesignState)sut.CreateDesignState(guid, "testdesignstate");
            const string name = "testepicstate";
            var state = sut.CreateEpicElementState(guid, epicGuid, name);

            sut.PersistChanges();

            Assert.Equal(guid, state.DesignGuid);
            Assert.Equal(epicGuid, state.Guid);
            Assert.Equal(name, state.Name);

            sutBuilder.DesignStateCollection.Verify(s => s.InsertMany(
                It.Is<ICollection<DesignState>>(
                    l => l.Contains(designState) &&
                    l.Count == 1 &&
                    l.First().EpicElementStates.Contains(state) &&
                    l.First().EpicElementStates.Count == 1),
                null, CancellationToken.None), Times.Once,
                "InsertMany was not called with the expected state");
        }

        [Fact(DisplayName = "DeleteEpicElementState", Skip = "todo: inject some state to delete?")]
        public void CanDeleteEpicElementState()
        {
            var sutBuilder = new DesignRepositorySutBuilder().WithDesignCollection();
            var sut = sutBuilder.Build();
            var guid = Guid.NewGuid();
            var epicGuid = Guid.NewGuid();
            var designState = new DesignState { Guid = guid, Name = "testdesignstate", EpicElementStates = new List<IEpicElementState> { new EpicElementState { Guid = Guid.NewGuid(), Name = "Epic" } } };

            // todo: setup to return this designState from GetDesignState
            // fake return from collection? Does not seem possible because of
            // extensionmethod

            sutBuilder.DesignStateCollection.Setup(s => s.Find(It.IsAny<FilterDefinition<DesignState>>(), null).FirstOrDefault(CancellationToken.None)).Returns(designState);

            sut.PersistChanges();

            sutBuilder.DesignStateCollection.Verify(s => s.InsertMany(
                It.Is<ICollection<DesignState>>(
                    l => l.Contains(designState) &&
                    l.Count == 1 &&
                    l.First().EpicElementStates.Count == 0),
                null, CancellationToken.None), Times.Once,
                "InsertMany was not called with the expected state");
        }



    }
    public class DesignRepositorySutBuilder
    {
        private Mock<IMongoDatabase> _databaseMock;
        private Mock<IMongoClient> _clientMock;

        public Mock<IMongoDatabase> Database { get { return _databaseMock; } }
        public Mock<IMongoClient> Client { get { return _clientMock; } }
        public Mock<IMongoCollection<DesignState>> DesignStateCollection { get; private set; }

        public DesignStateRepository Build()
        {
            _clientMock = new Mock<IMongoClient>();
            _databaseMock = new Mock<IMongoDatabase>();

            DesignStateCollection = new Mock<IMongoCollection<DesignState>>();
            _databaseMock.Setup(s => s.GetCollection<DesignState>("DesignStates", null)).Returns(DesignStateCollection.Object);

            _clientMock.Setup(s => s.GetDatabase("SoftwareManagement", null)).Returns(_databaseMock.Object);

            var sut = new DesignStateRepository(_clientMock.Object);
            return sut;
        }
        public DesignRepositorySutBuilder WithDesignCollection()
        {
            DesignStateCollection = new Mock<IMongoCollection<DesignState>>();
            return this;
        }
    }

}
