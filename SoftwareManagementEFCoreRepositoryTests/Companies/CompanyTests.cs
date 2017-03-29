using Microsoft.EntityFrameworkCore;
using SoftwareManagementEFCoreRepository;
using SoftwareManagementEFCoreRepositoryTests.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SoftwareManagementEFCoreRepositoryTests.Companies
{
    [Trait("Entity", "MainRepository_EFC")]
    public class CompanyTests
    {
        [Fact(DisplayName = "CreateCompanyState")]
        public void CreateCompanyState_Succeeds()
        {
            var options = new DbContextOptionsBuilder<MainContext>()
                .UseInMemoryDatabase(databaseName: "CreateCompanyState_adds_to_context")
                .Options;

            // Run the test against one instance of the context
            using (var context = new MainContext(options))
            {
                var sut = new MainRepository(context);
                var guid = Guid.NewGuid();
                var name = "new";
                var state = sut.CreateCompanyState(guid, name);

                Assert.Equal(state, context.CompanyStates.Local.First());

                sut.PersistChanges();

                Assert.Equal(state, context.CompanyStates.First());
                Assert.Equal(1, context.CompanyStates.Count());
                Assert.Equal(guid, context.CompanyStates.First().Guid);
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new MainContext(options))
            {
                Assert.Equal(1, context.CompanyStates.Count());
            }
        }

        [Fact(DisplayName = "GetCompanyState")]
        public void GetCompanyState_Succeeds_OnlyWhenGuidMatchesExisting()
        {
            var guid = Guid.NewGuid();
            var invalidGuid = Guid.NewGuid();
            const string name = "Cool Company";
            var inMemoryDatabaseBuilder = new InMemoryDatabaseBuilder();
            var options = inMemoryDatabaseBuilder.WithCompanyState(guid, name).Build("GetCompanyState");
            // Run the test against a clean instance of the context
            using (var context = new MainContext(options))
            {
                inMemoryDatabaseBuilder.InitializeContext(context);
                var sut = new MainRepository(context);
                var state = sut.GetCompanyState(guid);
                var invalidState = sut.GetCompanyState(invalidGuid);

                Assert.Equal(name, state.Name);

                Assert.Null(invalidState);
            }
        }
        [Fact(DisplayName = "DeleteCompanyState")]
        public void CanDeleteCompanyState()
        {
            var guid = Guid.NewGuid();
            var name = "To be deleted.";
            var inMemoryDatabaseBuilder = new InMemoryDatabaseBuilder();
            var options = inMemoryDatabaseBuilder.WithCompanyState(guid, name).Build("DeleteCompanyState", true);
            // Run the test against a clean instance of the context
            using (var context = new MainContext(options))
            {
                inMemoryDatabaseBuilder.InitializeContext(context);
                var sut = new MainRepository(context);
                var state = context.CompanyStates.Find(guid);
                Assert.Equal(EntityState.Unchanged, context.Entry(state).State);

                sut.DeleteCompanyState(guid);

                Assert.Equal(EntityState.Deleted, context.Entry(state).State);
            }
        }
        [Fact(DisplayName = "CanAddCompanyRoleStateToCompanyState")]
        public void AddCompanyRoleState_Succeeds_WithNewRole_AndCreatesRoleState()
        {
            var companyGuid = Guid.NewGuid();
            const string companyName = "Cool Company";
            var roleGuid = Guid.NewGuid();
            const string roleName = "Tester";
            var inMemoryDatabaseBuilder = new InMemoryDatabaseBuilder();
            var options = inMemoryDatabaseBuilder
                .WithCompanyState(companyGuid, companyName)
                .Build("GetCompanyState", true);

            // Run the test against a clean instance of the context
            using (var context = new MainContext(options))
            {
                inMemoryDatabaseBuilder.InitializeContext(context);
                var sut = new MainRepository(context);
                sut.AddRoleToCompanyState(companyGuid, roleGuid, roleName);
                var companyState = sut.GetCompanyState(companyGuid);
                var roleState = companyState.CompanyRoleStates.Single(w => w.Guid == roleGuid);

                Assert.Equal(EntityState.Added, context.Entry(roleState).State);
                Assert.Equal(roleName, roleState.Name);
            }
        }

        [Fact(DisplayName = "CanRemoveCompanyRoleStateToCompanyState")]
        public void RemoveCompanyRoleState_Succeeds()
        {
            var companyGuid = Guid.NewGuid();
            const string companyName = "Cool Company";
            var roleGuid = Guid.NewGuid();
            const string roleName = "Software Developer 2";
            var inMemoryDatabaseBuilder = new InMemoryDatabaseBuilder();
            var options = inMemoryDatabaseBuilder
                .WithCompanyState(companyGuid, companyName)
                .WithCompanyRoleState(roleGuid, roleName, companyGuid)
                .Build("GetCompanyState", true);

            // Run the test against a clean instance of the context
            using (var context = new MainContext(options))
            {
                inMemoryDatabaseBuilder.InitializeContext(context);
                var sut = new MainRepository(context);
                sut.RemoveRoleFromCompanyState(companyGuid, roleGuid);
                var companyState = sut.GetCompanyState(companyGuid);
                var roleState = companyState.CompanyRoleStates.SingleOrDefault(w => w.Guid == roleGuid);
                roleState = context.CompanyRoleStates.Find(roleGuid);
                Assert.Equal(EntityState.Deleted, context.Entry(roleState).State);
                Assert.Equal(roleName, roleState.Name);
            }
        }

        [Fact(DisplayName = "CanAddCompanyEnvironmentStateToCompanyState")]
        public void AddCompanyEnvironmentState_Succeeds_WithNewEnvironment_AndCreatesEnvironmentState()
        {
            var companyGuid = Guid.NewGuid();
            const string companyName = "Cool Company";
            var environmentGuid = Guid.NewGuid();
            const string environmentName = "Tester";
            var inMemoryDatabaseBuilder = new InMemoryDatabaseBuilder();
            var options = inMemoryDatabaseBuilder
                .WithCompanyState(companyGuid, companyName)
                .Build("GetCompanyState", true);

            // Run the test against a clean instance of the context
            using (var context = new MainContext(options))
            {
                inMemoryDatabaseBuilder.InitializeContext(context);
                var sut = new MainRepository(context);
                sut.AddEnvironmentToCompanyState(companyGuid, environmentGuid, environmentName);
                var companyState = sut.GetCompanyState(companyGuid);
                var environmentState = companyState.CompanyEnvironmentStates.Single(w => w.Guid == environmentGuid);

                Assert.Equal(EntityState.Added, context.Entry(environmentState).State);
                Assert.Equal(environmentName, environmentState.Name);
            }
        }

        [Fact(DisplayName = "CanRemoveCompanyEnvironmentStateToCompanyState")]
        public void RemoveCompanyEnvironmentState_Succeeds()
        {
            var companyGuid = Guid.NewGuid();
            const string companyName = "Cool Company";
            var environmentGuid = Guid.NewGuid();
            const string environmentName = "Software Developer 2";
            var inMemoryDatabaseBuilder = new InMemoryDatabaseBuilder();
            var options = inMemoryDatabaseBuilder
                .WithCompanyState(companyGuid, companyName)
                .WithCompanyEnvironmentState(environmentGuid, environmentName, companyGuid)
                .Build("GetCompanyState", true);

            // Run the test against a clean instance of the context
            using (var context = new MainContext(options))
            {
                inMemoryDatabaseBuilder.InitializeContext(context);
                var sut = new MainRepository(context);
                sut.RemoveEnvironmentFromCompanyState(companyGuid, environmentGuid);
                var companyState = sut.GetCompanyState(companyGuid);
                var environmentState = companyState.CompanyEnvironmentStates.SingleOrDefault(w => w.Guid == environmentGuid);
                environmentState = context.CompanyEnvironmentStates.Find(environmentGuid);
                Assert.Equal(EntityState.Deleted, context.Entry(environmentState).State);
                Assert.Equal(environmentName, environmentState.Name);
            }
        }

    }
    public class CompanyStateBuilder : EntityStateBuilder<CompanyState>
    {
    }
    public class CompanyRoleStateBuilder : EntityStateBuilder<CompanyRoleState>
    {
        private Guid _companyGuid;
        public CompanyRoleStateBuilder WithCompanyGuid(Guid companyGuid)
        {
            _companyGuid = companyGuid;
            return this;
        }
        public override CompanyRoleState Build()
        {
            var state = base.Build();
            state.CompanyGuid = _companyGuid;
            return state;
        }
    }
    public class CompanyEnvironmentStateBuilder : EntityStateBuilder<CompanyEnvironmentState>
    {
        private Guid _companyGuid;
        public CompanyEnvironmentStateBuilder WithCompanyGuid(Guid companyGuid)
        {
            _companyGuid = companyGuid;
            return this;
        }
        public override CompanyEnvironmentState Build()
        {
            var state = base.Build();
            state.CompanyGuid = _companyGuid;
            return state;
        }
    }

}
