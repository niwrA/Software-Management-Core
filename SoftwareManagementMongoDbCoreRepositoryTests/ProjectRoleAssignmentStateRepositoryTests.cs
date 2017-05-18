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
    public class ProjectRoleAssignmentStateRepositoryTests
    {
        [Fact(DisplayName = "CreateProjectRoleAssignmentState")]
        public void CanCreateProjectRoleAssignmentState()
        {
            var sutBuilder = new SutBuilder();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();
            var projectRoleGuid = Guid.NewGuid();
            var contactGuid = Guid.NewGuid();

            var state = sut.CreateProjectRoleAssignmentState(guid, contactGuid, projectRoleGuid);

            Assert.Equal(guid, state.Guid);
            Assert.Equal(contactGuid, state.ContactGuid);
            Assert.Equal(projectRoleGuid, state.ProjectRoleGuid);
        }

        [Fact(DisplayName = "DeleteProjectRoleAssignmentState")]
        public void CanDeleteProjectRoleAssignmentState()
        {
            var sutBuilder = new SutBuilder();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();

            sut.DeleteProjectRoleAssignmentState(guid);
        }

        [Fact(DisplayName = "PersistCreatedProjectRoleAssignmentState")]
        public void WhenCreateProjectRoleAssignmentState_PersistChanges_InsertsIntoCollection()
        {
            var sutBuilder = new SutBuilder().WithProjectRoleAssignmentCollection();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();
            var projectRoleGuid = Guid.NewGuid();
            var contactGuid = Guid.NewGuid();

            var state = sut.CreateProjectRoleAssignmentState(guid, contactGuid, projectRoleGuid) as ProjectRoleAssignmentState;

            sut.PersistChanges();

            sutBuilder.ProjectRoleAssignmentStateCollection.Verify(s => s.InsertMany(
                It.Is<ICollection<ProjectRoleAssignmentState>>(
                l => l.Contains(state) &&
                l.Count == 1)
                , null, CancellationToken.None), Times.Once, "InsertMany was not called correctly");
        }

        [Fact(DisplayName = "PersistDeletedProjectRoleAssignmentState")]
        public void WhenDeleteProjectRoleAssignmentState_PersistChanges_DeletesFromCollection()
        {
            var sutBuilder = new SutBuilder().WithProjectRoleAssignmentCollection();
            var sut = sutBuilder.Build();

            var guid = Guid.NewGuid();

            sut.DeleteProjectRoleAssignmentState(guid);

            sut.PersistChanges();

            sutBuilder.ProjectRoleAssignmentStateCollection.Verify(s => s.DeleteOne(It.IsAny<FilterDefinition<ProjectRoleAssignmentState>>(), null, CancellationToken.None), Times.Once, "DeleteOne was not called");
        }

        public class SutBuilder
        {
            private Mock<IMongoDatabase> _databaseMock;
            private Mock<IMongoClient> _clientMock;

            public Mock<IMongoDatabase> Database { get { return _databaseMock; } }
            public Mock<IMongoClient> Client { get { return _clientMock; } }
            public Mock<IMongoCollection<ProjectRoleAssignmentState>> ProjectRoleAssignmentStateCollection { get; private set; }
            public ProjectRoleAssignmentStateRepository Build()
            {
                _clientMock = new Mock<IMongoClient>();
                _databaseMock = new Mock<IMongoDatabase>();

                if (ProjectRoleAssignmentStateCollection != null)
                {
                    _databaseMock.Setup(s => s.GetCollection<ProjectRoleAssignmentState>("ProjectRoleAssignmentStates", null)).Returns(ProjectRoleAssignmentStateCollection.Object);
                }

                _clientMock.Setup(s => s.GetDatabase("SoftwareManagement", null)).Returns(_databaseMock.Object);

                var sut = new ProjectRoleAssignmentStateRepository(_clientMock.Object);
                return sut;
            }
            public SutBuilder WithProjectRoleAssignmentCollection()
            {
                ProjectRoleAssignmentStateCollection = new Mock<IMongoCollection<ProjectRoleAssignmentState>>();
                return this;
            }
        }

    }
}
