using MongoDB.Driver;
using Moq;
using SoftwareManagementMongoDbCoreRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace SoftwareManagementMongoDbCoreRepositoryTests
{
  [Trait("MongoDb", "CompanyState")]
  class CompanyStateRepositoryTests
    {
        [Fact(DisplayName = "AddCompanyEnvironmentState")]
        public void CanAddCompanyEnvironmentState()
        {
            var sutBuilder = new SutBuilder().WithCompanyCollection();
            var sut = sutBuilder.Build();
            var guid = Guid.NewGuid();
            var environmentGuid = Guid.NewGuid();
            var companyState = (CompanyState)sut.CreateCompanyState(guid, "testcompanystate");
            var state = sut.AddEnvironmentToCompanyState(guid, environmentGuid, "testenvironmentstate");

            sut.PersistChanges();

            sutBuilder.CompanyStateCollection.Verify(s => s.InsertMany(
                It.Is<ICollection<CompanyState>>(
                    l => l.Contains(companyState) &&
                    l.Count == 1 &&
                    l.First().CompanyEnvironmentStates.Contains(state) &&
                    l.First().CompanyEnvironmentStates.Count == 1),
                null, CancellationToken.None), Times.Once,
                "InsertMany was not called with the expected state");
        }

        [Fact(DisplayName = "DeleteCompanyEnvironmentState", Skip = "In progress")]
        public void CanDeleteCompanyEnvironmentState()
        {
            var sutBuilder = new SutBuilder().WithCompanyCollection();
            var sut = sutBuilder.Build();
            var guid = Guid.NewGuid();
            var environmentGuid = Guid.NewGuid();
            var companyState = (CompanyState)sut.CreateCompanyState(guid, "testcompanystate");

            sut.PersistChanges();

        }

        [Fact(DisplayName = "CanGetEnvironmentState", Skip = "In progress")]
        public void CanGetEnvironmentState()
        {
            var sutBuilder = new SutBuilder().WithCompanyCollection();
            var sut = sutBuilder.Build();
            var guid = Guid.NewGuid();
            var environmentGuid = Guid.NewGuid();

            // todo: setup mock for GetCompanyState

            sut.GetEnvironmentState(guid, environmentGuid);
        }

    // todo: this tests tests too much, makes previous tests unnecessary
    [Fact(DisplayName = "AddCompanyEnvironmentHardwareState")]
    public void CanAddCompanyEnvironmentHardwareState()
    {
      var sutBuilder = new SutBuilder().WithCompanyCollection();
      var sut = sutBuilder.Build();
      var guid = Guid.NewGuid();
      var environmentGuid = Guid.NewGuid();
      var hardwareGuid = Guid.NewGuid();
      var companyState = (CompanyState)sut.CreateCompanyState(guid, "testcompanystate");
      var state = sut.AddEnvironmentToCompanyState(guid, environmentGuid, "testenvironmentstate");
      var hardwareState = sut.AddHardwareToEnvironmentState(state, hardwareGuid, "testhardwarestate");

      sut.PersistChanges();

      sutBuilder.CompanyStateCollection.Verify(s => s.InsertMany(
          It.Is<ICollection<CompanyState>>(
              l => l.Contains(companyState) &&
              l.Count == 1 &&
              l.First().CompanyEnvironmentStates.Contains(state) &&
              l.First().CompanyEnvironmentStates.Count == 1 &&
              l.First().CompanyEnvironmentStates.First().HardwareStates.Count == 1 &&
              l.First().CompanyEnvironmentStates.First().HardwareStates.First() == hardwareState),
          null, CancellationToken.None), Times.Once,
          "InsertMany was not called with the expected state");
    }


    public class SutBuilder
        {
            private Mock<IMongoDatabase> _databaseMock;
            private Mock<IMongoClient> _clientMock;

            public Mock<IMongoDatabase> Database { get { return _databaseMock; } }
            public Mock<IMongoClient> Client { get { return _clientMock; } }
            public Mock<IMongoCollection<CompanyState>> CompanyStateCollection { get; private set; }

            public CompanyStateRepository Build()
            {
                _clientMock = new Mock<IMongoClient>();
                _databaseMock = new Mock<IMongoDatabase>();

                if (CompanyStateCollection != null)
                {
                    _databaseMock.Setup(s => s.GetCollection<CompanyState>("CompanyStates", null)).Returns(CompanyStateCollection.Object);
                }

                _clientMock.Setup(s => s.GetDatabase("SoftwareManagement", null)).Returns(_databaseMock.Object);

                var sut = new CompanyStateRepository(_clientMock.Object);
                return sut;
            }
            public SutBuilder WithCompanyCollection()
            {
                CompanyStateCollection = new Mock<IMongoCollection<CompanyState>>();
                return this;
            }
        }


    }
}
