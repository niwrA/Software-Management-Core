using CommandsShared;
using Moq;
using ProjectRoleAssignmentsShared;
using SoftwareManagementCoreTests.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.ProjectRoleAssignments
{
    [Trait("Entity", "ProjectRoleAssignment")]
    public class ProjectRoleAssignmentCommandsTests
    {
        [Fact(DisplayName = "CreateProjectRoleAssignmentCommand")]
        public void CreateCommand()
        {
            var projectroleassignmentsMock = new Mock<IProjectRoleAssignmentService>();
            var sut = new CommandBuilder<CreateProjectRoleAssignmentCommand>().Build(projectroleassignmentsMock.Object) as CreateProjectRoleAssignmentCommand;

            sut.ContactGuid = Guid.NewGuid();
            sut.CompanyRoleGuid = Guid.NewGuid();
            sut.StartDate = DateTime.Now.Date;
            sut.EndDate = DateTime.Now.Date.AddYears(1);
            sut.Execute();

            projectroleassignmentsMock.Verify(s => s.CreateProjectRoleAssignment(sut.EntityGuid, sut.ContactGuid, sut.CompanyRoleGuid, sut.StartDate, sut.EndDate, sut.ContactName), Times.Once);
        }

        // todo: duplicate check on ProjectRoleAssignments

        [Fact(DisplayName = "DeleteProjectRoleAssignmentCommand")]
        public void DeleteCommand()
        {
            var projectroleassignmentsMock = new Mock<IProjectRoleAssignmentService>();
            var sut = new CommandBuilder<DeleteProjectRoleAssignmentCommand>().Build(projectroleassignmentsMock.Object) as DeleteProjectRoleAssignmentCommand;

            sut.Execute();

            projectroleassignmentsMock.Verify(s => s.DeleteProjectRoleAssignment(sut.EntityGuid), Times.Once);
        }

        [Fact(DisplayName = "ChangeStartDateOfProjectRoleAssignmentCommand")]
        public void ChangeProjectRoleAssignmentOfStartDateCommand()
        {
            var sutBuilder = new ProjectRoleAssignmentCommandBuilder<ChangeStartDateOfProjectRoleAssignmentCommand>();
            var sut = sutBuilder.Build() as ChangeStartDateOfProjectRoleAssignmentCommand;

            sut.OriginalStartDate = DateTime.Now;
            sut.StartDate = DateTime.Now;
            sut.Execute();

            sutBuilder.ProjectRoleAssignmentMock.Verify(s => s.ChangeStartDate(sut.StartDate, sut.OriginalStartDate), Times.Once);
        }

        [Fact(DisplayName = "ChangeEndDateOfProjectRoleAssignmentCommand")]
        public void ChangeEndDateOfProjectRoleAssignmentCommand()
        {
            var sutBuilder = new ProjectRoleAssignmentCommandBuilder<ChangeEndDateOfProjectRoleAssignmentCommand>();
            var sut = sutBuilder.Build() as ChangeEndDateOfProjectRoleAssignmentCommand;

            sut.OriginalEndDate = DateTime.Now;
            sut.EndDate = DateTime.Now;
            sut.Execute();

            sutBuilder.ProjectRoleAssignmentMock.Verify(s => s.ChangeEndDate(sut.EndDate, sut.OriginalEndDate), Times.Once);
        }
    }

    class ProjectRoleAssignmentCommandBuilder<T> where T : ICommand, new()
    {
        public Mock<IProjectRoleAssignment> ProjectRoleAssignmentMock { get; set; }
        public ICommand Build()
        {
            var projectroleassignmentsMock = new Mock<IProjectRoleAssignmentService>();
            var projectroleassignmentMock = new Mock<IProjectRoleAssignment>();
            this.ProjectRoleAssignmentMock = projectroleassignmentMock;

            var sut = new CommandBuilder<T>().Build(projectroleassignmentsMock.Object);

            projectroleassignmentsMock.Setup(s => s.GetProjectRoleAssignment(sut.EntityGuid)).Returns(projectroleassignmentMock.Object);

            return sut;
        }
    }
}
