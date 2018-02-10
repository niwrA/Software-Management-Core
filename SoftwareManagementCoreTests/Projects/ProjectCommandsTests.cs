using niwrA.CommandManager;
using Moq;
using ProjectsShared;
using SoftwareManagementCoreTests.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Projects
{
    [Trait("Entity", "Project")]
    public class ProjectCommandsTests
    {
        [Fact(DisplayName = "CreateProjectCommand")]
        public void CreateCommand()
        {
            var projectsMock = new Mock<IProjectService>();
            var sut = new CommandBuilder<CreateProjectCommand>().Build(projectsMock.Object) as CreateProjectCommand;

            sut.Name = "New Project";
            sut.Execute();

            projectsMock.Verify(s => s.CreateProject(sut.EntityGuid, sut.Name), Times.Once);
        }

        [Fact(DisplayName = "DeleteProjectCommand")]
        public void DeleteCommand()
        {
            var projectsMock = new Mock<IProjectService>();
            var sut = new CommandBuilder<DeleteProjectCommand>().Build(projectsMock.Object) as DeleteProjectCommand;

            sut.Execute();

            projectsMock.Verify(s => s.DeleteProject(sut.EntityGuid), Times.Once);
        }

        [Fact(DisplayName = "RenameProjectCommand")]
        public void RenameCommand()
        {
            var sutBuilder = new ProjectCommandBuilder<RenameProjectCommand>();
            var sut = sutBuilder.Build() as RenameProjectCommand;

            sut.Name = "New Name";
            sut.OriginalName = "Original Name";
            sut.Execute();

            sutBuilder.ProjectMock.Verify(s => s.Rename(sut.Name, sut.OriginalName), Times.Once);
        }

        [Fact(DisplayName = "ChangeStartDateOfProjectCommand")]
        public void ChangeProjectOfStartDateCommand()
        {
            var sutBuilder = new ProjectCommandBuilder<ChangeStartDateOfProjectCommand>();
            var sut = sutBuilder.Build() as ChangeStartDateOfProjectCommand;

            sut.OriginalStartDate = DateTime.Now;
            sut.StartDate = DateTime.Now;
            sut.Execute();

            sutBuilder.ProjectMock.Verify(s => s.ChangeStartDate(sut.StartDate, sut.OriginalStartDate), Times.Once);
        }

        [Fact(DisplayName = "ChangeEndDateOfProjectCommand")]
        public void ChangeEndDateOfProjectCommand()
        {
            var sutBuilder = new ProjectCommandBuilder<ChangeEndDateOfProjectCommand>();
            var sut = sutBuilder.Build() as ChangeEndDateOfProjectCommand;

            sut.OriginalEndDate = DateTime.Now;
            sut.EndDate = DateTime.Now;
            sut.Execute();

            sutBuilder.ProjectMock.Verify(s => s.ChangeEndDate(sut.EndDate, sut.OriginalEndDate), Times.Once);
        }

        [Fact(DisplayName = "AddRoleToProjectCommand")]
        public void AddRoleToProjectCommand()
        {
            var sutBuilder = new ProjectCommandBuilder<AddRoleToProjectCommand>();
            var sut = sutBuilder.Build() as AddRoleToProjectCommand;

            sut.RoleGuid = Guid.NewGuid();
            sut.RoleName = "New Name";
            sut.Execute();

            sutBuilder.ProjectMock.Verify(s => s.AddRoleToProject(sut.RoleGuid, sut.RoleName), Times.Once);
        }


        [Fact(DisplayName = "RemoveRoleFromProjectCommand")]
        public void RemoveRoleFromProjectCommand()
        {
            var sutBuilder = new ProjectCommandBuilder<RemoveRoleFromProjectCommand>();
            var sut = sutBuilder.Build() as RemoveRoleFromProjectCommand;

            sut.RoleGuid = Guid.NewGuid();
            sut.Execute();

            sutBuilder.ProjectMock.Verify(s => s.RemoveRoleFromProject(sut.RoleGuid), Times.Once);
        }
    }

    class ProjectCommandBuilder<T> where T: ICommand, new()
    {
        public Mock<IProject> ProjectMock { get; set; }
        public ICommand Build()
        {
            var projectsMock = new Mock<IProjectService>();
            var projectMock = new Mock<IProject>();
            this.ProjectMock = projectMock;

            var sut = new CommandBuilder<T>().Build(projectsMock.Object);

            projectsMock.Setup(s => s.GetProject(sut.EntityGuid)).Returns(projectMock.Object);

            return sut;
        }
    }
}
