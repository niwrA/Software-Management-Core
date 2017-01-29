using DateTimeShared;
using Moq;
using ProjectsShared;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Projects
{
    [Trait("Entity", "Project")]
    public class ProjectsTests
    {
        [Fact(DisplayName = "Delete")]
        public void CanDeleteProject()
        {
            var repoMock = new Mock<IProjectStateRepository>();
            var sut = new ProjectService(repoMock.Object, new DateTimeProvider());
            var guid = Guid.NewGuid();

            sut.DeleteProject(guid);

            repoMock.Verify(s => s.DeleteProjectState(guid));
        }

        [Fact(DisplayName = "Create")]
        public void CanCreateProject()
        {
            var repoMock = new Mock<IProjectStateRepository>();
            var stateMock = new Mock<IProjectState>();
            var guid = Guid.NewGuid();
            var name = "new";

            repoMock.Setup(s => s.CreateProjectState(guid, name)).Returns(stateMock.Object);

            var sut = new ProjectService(repoMock.Object, new DateTimeProvider());
            sut.CreateProject(guid, name);

            repoMock.Verify(s => s.CreateProjectState(guid, name));
        }

        [Fact(DisplayName = "CanAddRoleToProject")]
        public void CanAddRoleToProject()
        {
            var repoMock = new Mock<IProjectStateRepository>();
            var stateMock = new Fakes.ProjectState { Guid = Guid.NewGuid() };
            var sut = new Project(stateMock, repoMock.Object);

            var roleGuid = Guid.NewGuid();
            var roleName = "Tester";

            sut.AddRoleToProject(roleGuid, roleName);

            repoMock.Verify(s => s.AddRoleToProjectState(stateMock.Guid, roleGuid, roleName), Times.Once);
        }

        [Fact(DisplayName = "CanRemoveRoleFromProject")]
        public void CanRemoveRoleFromProject()
        {
            var repoMock = new Mock<IProjectStateRepository>();
            var stateMock = new Fakes.ProjectState { Guid = Guid.NewGuid() };
            var sut = new Project(stateMock, repoMock.Object);

            var roleGuid = Guid.NewGuid();

            sut.RemoveRoleFromProject(roleGuid);

            repoMock.Verify(s => s.RemoveRoleFromProjectState(stateMock.Guid, roleGuid), Times.Once);
        }
    }
}
