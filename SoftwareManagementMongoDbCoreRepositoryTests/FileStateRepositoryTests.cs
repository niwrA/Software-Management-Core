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
  [Trait("MongoDb", "FileState")]
  public class FileStateRepositoryTests
    {

        [Fact(DisplayName = "PersistCreatedFileState")]
        public void WhenCreateFileState_PersistChanges_InsertsIntoCollection()
        {
            var sutBuilder = new SutBuilder().WithFileCollection();
            var sut = sutBuilder.Build();

            var name = "File name";
            var guid = Guid.NewGuid();

            var state = sut.CreateFileState(guid, name) as FileState;

            sut.PersistChanges();

            sutBuilder.FileStateCollection.Verify(s => s.InsertMany(
                It.Is<ICollection<FileState>>(
                l => l.Contains(state) &&
                l.Count == 1)
                , null, CancellationToken.None), Times.Once, "InsertMany was not called correctly");
        }

        [Fact(DisplayName = "PersistDeletedFileState")]
        public void WhenDeleteFileState_PersistChanges_DeletesFromCollection()
        {
            var sutBuilder = new SutBuilder().WithFileCollection();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();

            sut.DeleteFileState(guid);

            sut.PersistChanges();

            sutBuilder.FileStateCollection.Verify(s => s.DeleteOne(It.IsAny<FilterDefinition<FileState>>(), CancellationToken.None), Times.Once, "DeleteOne was not called");
        }

        public class SutBuilder
        {
            private Mock<IMongoDatabase> _databaseMock;
            private Mock<IMongoClient> _clientMock;

            public Mock<IMongoDatabase> Database { get { return _databaseMock; } }
            public Mock<IMongoClient> Client { get { return _clientMock; } }
            public Mock<IMongoCollection<FileState>> FileStateCollection { get; private set; }

            public FileStateRepository Build()
            {
                _clientMock = new Mock<IMongoClient>();
                _databaseMock = new Mock<IMongoDatabase>();

                if (FileStateCollection != null)
                {
                    _databaseMock.Setup(s => s.GetCollection<FileState>("FileStates", null)).Returns(FileStateCollection.Object);
                }

                _clientMock.Setup(s => s.GetDatabase("SoftwareManagement", null)).Returns(_databaseMock.Object);

                var sut = new FileStateRepository(_clientMock.Object);
                return sut;
            }
            public SutBuilder WithFileCollection()
            {
                FileStateCollection = new Mock<IMongoCollection<FileState>>();
                return this;
            }
        }


    }
}
