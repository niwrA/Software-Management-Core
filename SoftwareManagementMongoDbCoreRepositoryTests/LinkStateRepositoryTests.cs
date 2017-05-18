using MongoDB.Driver;
using Moq;
using SoftwareManagementMongoDbCoreRepository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace SoftwareManagementMongoDbCoreRepositoryTests
{
   public class LinkStateRepositoryTests
    {

        [Fact(DisplayName = "PersistCreatedLinkState")]
        public void WhenCreateLinkState_PersistChanges_InsertsIntoCollection()
        {
            var sutBuilder = new SutBuilder().WithLinkCollection();
            var sut = sutBuilder.Build();

            var name = "Link name";
            var guid = Guid.NewGuid();

            var state = sut.CreateLinkState(guid, name) as LinkState;

            sut.PersistChanges();

            sutBuilder.LinkStateCollection.Verify(s => s.InsertMany(
                It.Is<ICollection<LinkState>>(
                l => l.Contains(state) &&
                l.Count == 1)
                , null, CancellationToken.None), Times.Once, "InsertMany was not called correctly");
        }

        [Fact(DisplayName = "PersistDeletedLinkState")]
        public void WhenDeleteLinkState_PersistChanges_DeletesFromCollection()
        {
            var sutBuilder = new SutBuilder().WithLinkCollection();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();

            sut.DeleteLinkState(guid);

            sut.PersistChanges();

            sutBuilder.LinkStateCollection.Verify(s => s.DeleteOne(It.IsAny<FilterDefinition<LinkState>>(), CancellationToken.None), Times.Once, "DeleteOne was not called");
        }

        public class SutBuilder
        {
            private Mock<IMongoDatabase> _databaseMock;
            private Mock<IMongoClient> _clientMock;

            public Mock<IMongoDatabase> Database { get { return _databaseMock; } }
            public Mock<IMongoClient> Client { get { return _clientMock; } }
            public Mock<IMongoCollection<LinkState>> LinkStateCollection { get; private set; }

            public LinkStateRepository Build()
            {
                _clientMock = new Mock<IMongoClient>();
                _databaseMock = new Mock<IMongoDatabase>();

                if (LinkStateCollection != null)
                {
                    _databaseMock.Setup(s => s.GetCollection<LinkState>("LinkStates", null)).Returns(LinkStateCollection.Object);
                }

                _clientMock.Setup(s => s.GetDatabase("SoftwareManagement", null)).Returns(_databaseMock.Object);

                var sut = new LinkStateRepository(_clientMock.Object);
                return sut;
            }
            public SutBuilder WithLinkCollection()
            {
                LinkStateCollection = new Mock<IMongoCollection<LinkState>>();
                return this;
            }
        }


    }
}
