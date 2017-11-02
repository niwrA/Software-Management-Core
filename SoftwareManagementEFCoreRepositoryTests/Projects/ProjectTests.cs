using Microsoft.EntityFrameworkCore;
using SoftwareManagementEFCoreRepository;
using SoftwareManagementEFCoreRepositoryTests.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SoftwareManagementEFCoreRepositoryTests.Projects
{
    [Trait("EFCore", "ProjectState")]
    public class ProjectTests
    {
        [Fact(DisplayName = "CanAddProjectRoleStateToProjectState")]
        public void AddProjectRoleState_Succeeds_WithNewRole_AndCreatesRoleState()
        {
            var projectGuid = Guid.NewGuid();
            const string projectName = "Cool Project";
            var roleGuid = Guid.NewGuid();
            const string roleName = "Tester";
            var inMemoryDatabaseBuilder = new InMemoryDatabaseBuilder();
            var options = inMemoryDatabaseBuilder
                .WithProjectState(projectGuid, projectName)
                .Build("GetProjectState", true);

            // Run the test against a clean instance of the context
            using (var context = new MainContext(options))
            {
                inMemoryDatabaseBuilder.InitializeContext(context);
                var sut = new MainRepository(context);
                sut.AddRoleToProjectState(projectGuid, roleGuid, roleName);
                var projectState = sut.GetProjectState(projectGuid);
                var roleState = projectState.ProjectRoleStates.Single(w => w.Guid == roleGuid);

                Assert.Equal(EntityState.Added, context.Entry(roleState).State);
                Assert.Equal(roleName, roleState.Name);
            }
        }

        [Fact(DisplayName = "CanRemoveProjectRoleStateToProjectState")]
        public void RemoveProjectRoleState_Succeeds()
        {
            var projectGuid = Guid.NewGuid();
            const string projectName = "Cool Project";
            var roleGuid = Guid.NewGuid();
            const string roleName = "Tester";
            var inMemoryDatabaseBuilder = new InMemoryDatabaseBuilder();
            var options = inMemoryDatabaseBuilder
                .WithProjectState(projectGuid, projectName)
                .WithProjectRoleState(roleGuid, roleName, projectGuid)
                .Build("GetProjectState", true);

            // Run the test against a clean instance of the context
            using (var context = new MainContext(options))
            {
                inMemoryDatabaseBuilder.InitializeContext(context);
                var sut = new MainRepository(context);
                sut.RemoveRoleFromProjectState(projectGuid, roleGuid);
                var projectState = sut.GetProjectState(projectGuid);
                var roleState = projectState.ProjectRoleStates.SingleOrDefault(w => w.Guid == roleGuid);
                //Assert.Null(roleState); inconsistent testbehavior vs CompanyRole
                roleState = context.ProjectRoleStates.Find(roleGuid);
                Assert.Equal(EntityState.Deleted, context.Entry(roleState).State);
                Assert.Equal(roleName, roleState.Name);
            }
        }

        [Fact(DisplayName = "DeleteProjectState")]
        public void CanDeleteProjectState()
        {
            var guid = Guid.NewGuid();
            var name = "To be deleted.";
            var inMemoryDatabaseBuilder = new InMemoryDatabaseBuilder();
            var options = inMemoryDatabaseBuilder.WithProjectState(guid, name).Build("DeleteProjectState", true);
            // Run the test against a clean instance of the context
            using (var context = new MainContext(options))
            {
                inMemoryDatabaseBuilder.InitializeContext(context);
                var sut = new MainRepository(context);
                var state = context.ProjectStates.Find(guid);
                Assert.Equal(EntityState.Unchanged, context.Entry(state).State);

                sut.DeleteProjectState(guid);

                Assert.Equal(EntityState.Deleted, context.Entry(state).State);
            }
        }
    }

    public class ProjectStateBuilder : EntityStateBuilder<ProjectState>
    {
    }
    public class ProjectRoleStateBuilder : EntityStateBuilder<ProjectRoleState>
    {
        private Guid _projectGuid;
        public ProjectRoleStateBuilder WithProjectGuid(Guid projectGuid)
        {
            _projectGuid = projectGuid;
            return this;
        }
        public override ProjectRoleState Build()
        {
            var state = base.Build();
            state.ProjectGuid = _projectGuid;
            return state;
        }
    }

}
