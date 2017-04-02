using ProjectRoleAssignmentsShared;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.ProjectRoleAssignments
{
    [Trait("Category", "ProjectRoleAssignment")]
    public class ProjectRoleAssignmentTests
    {
        [Fact(DisplayName = "CanCreate")]
        public void CanCreateProjectRoleAssignment()
        {
            var sutBuilder = new ProjectRoleAssignmentSutBuilder();
            var sut = sutBuilder.Build();

            Guid guid = Guid.NewGuid();
            Guid contactGuid = Guid.NewGuid();
            Guid companyRoleGuid = Guid.NewGuid();

            var startDate = DateTime.Now.Date as DateTime?;
            var projectroleassignment = sut.CreateProjectRoleAssignment(guid, contactGuid, companyRoleGuid, startDate, null, null);

            sutBuilder.RepoMock.Verify(v => v.CreateProjectRoleAssignmentState(guid, contactGuid, companyRoleGuid), Times.Once);
        }

        [Fact(DisplayName = "ReflectsState")]
        public void ProjectRoleAssignmentReflectsState()
        {
            var state = new Fakes.ProjectRoleAssignmentState();
            var sut = new ProjectRoleAssignment(state);

            Assert.Equal(state.Guid, sut.Guid);
            Assert.Equal(state.ContactGuid, sut.ContactGuid);
            Assert.Equal(state.ProjectRoleGuid, sut.ProjectRoleGuid);
            Assert.Equal(state.StartDate, sut.StartDate);
            Assert.Equal(state.EndDate, sut.EndDate);
            Assert.Equal(state.ContactName, sut.ContactName);
        }

        public class ProjectRoleAssignmentSutBuilder
        {
            Mock<IProjectRoleAssignmentStateRepository> _repo;
            public Mock<IProjectRoleAssignmentStateRepository> RepoMock { get { return _repo; } }
            public ProjectRoleAssignmentService Build()
            {
                _repo = new Mock<IProjectRoleAssignmentStateRepository>();
                _repo.Setup(s => s.CreateProjectRoleAssignmentState(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(new Fakes.ProjectRoleAssignmentState());
                var sut = new ProjectRoleAssignmentService(_repo.Object);
                return sut;
            }
        }
    }
}
