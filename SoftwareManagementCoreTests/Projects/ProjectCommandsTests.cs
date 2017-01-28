﻿using CommandsShared;
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
            var projectsMock = new Mock<IProjectService>();
            var projectMock = new Mock<IProject>();

            var sut = new CommandBuilder<RenameProjectCommand>().Build(projectsMock.Object) as RenameProjectCommand;

            projectsMock.Setup(s => s.GetProject(sut.EntityGuid)).Returns(projectMock.Object);

            sut.Name = "New Name";
            sut.OriginalName = "Original Name";
            sut.Execute();

            projectMock.Verify(s => s.Rename(sut.Name, sut.OriginalName), Times.Once);
        }

        [Fact(DisplayName = "ChangeStartDateOfProjectCommand")]
        public void ChangeProjectOfStartDateCommand()
        {
            var projectsMock = new Mock<IProjectService>();
            var projectMock = new Mock<IProject>();

            var sut = new CommandBuilder<ChangeStartDateOfProjectCommand>().Build(projectsMock.Object) as ChangeStartDateOfProjectCommand;

            projectsMock.Setup(s => s.GetProject(sut.EntityGuid)).Returns(projectMock.Object);

            sut.OriginalStartDate = DateTime.Now;
            sut.StartDate = DateTime.Now;
            sut.Execute();

            projectMock.Verify(s => s.ChangeStartDate(sut.StartDate, sut.OriginalStartDate), Times.Once);
        }

        [Fact(DisplayName = "ChangeEndDateOfProjectCommand")]
        public void ChangeEndDateOfProjectCommand()
        {
            var projectsMock = new Mock<IProjectService>();
            var projectMock = new Mock<IProject>();

            var sut = new CommandBuilder<ChangeEndDateOfProjectCommand>().Build(projectsMock.Object) as ChangeEndDateOfProjectCommand;

            projectsMock.Setup(s => s.GetProject(sut.EntityGuid)).Returns(projectMock.Object);

            sut.OriginalEndDate = DateTime.Now;
            sut.EndDate = DateTime.Now;
            sut.Execute();

            projectMock.Verify(s => s.ChangeEndDate(sut.EndDate, sut.OriginalEndDate), Times.Once);
        }

        [Fact(DisplayName = "RenameProjectCommand")]
        public void AddRoleToProjectCommand()
        {
            var sutBuilder = new ProjectCommandBuilder<AddRoleToProjectCommand>();
            var sut = sutBuilder.Build() as AddRoleToProjectCommand;

            sut.RoleGuid = Guid.NewGuid();
            sut.RoleName = "New Name";
            sut.Execute();

            sutBuilder.ProjectMock.Verify(s => s.AddRoleToProject(sut.RoleGuid, sut.RoleName), Times.Once);
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
