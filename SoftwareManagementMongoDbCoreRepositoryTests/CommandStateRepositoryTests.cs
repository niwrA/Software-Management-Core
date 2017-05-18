using Moq;
using System;
using Xunit;
using MongoDB;
using MongoDB.Driver;
using SoftwareManagementMongoDbCoreRepository;
using System.Collections.Generic;
using System.Threading;
using ProductsShared;
using System.Linq;

namespace SoftwareManagementMongoDbCoreRepositoryTests
{
    [Trait("Entity", "MainRepository_Mongo")]
    public class CommandStateRepositoryTests
    {
// todo: do we need this one in all of them?
        [Fact(DisplayName = "CanPersistChanges_WhenNoChanges")]
        public void CanPersistChanges_WhenNoChanges()
        {
            var sutBuilder = new SutBuilder();
            var sut = sutBuilder.Build();

            sut.PersistChanges();
        }
    }

    public class SutBuilder
    {
        private Mock<IMongoDatabase> _databaseMock;
        private Mock<IMongoClient> _clientMock;

        public Mock<IMongoDatabase> Database { get { return _databaseMock; } }
        public Mock<IMongoClient> Client { get { return _clientMock; } }
        public CommandStateRepository Build()
        {
            _clientMock = new Mock<IMongoClient>();
            _databaseMock = new Mock<IMongoDatabase>();

            _clientMock.Setup(s => s.GetDatabase("SoftwareManagement", null)).Returns(_databaseMock.Object);

            var sut = new CommandStateRepository(_clientMock.Object);
            return sut;
        }
    }

}
