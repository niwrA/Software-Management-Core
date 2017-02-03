using Microsoft.EntityFrameworkCore;
using SoftwareManagementEFCoreRepository;
using SoftwareManagementEFCoreRepositoryTests.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SoftwareManagementEFCoreRepositoryTests.Employments
{
    [Trait("Entity", "MainRepository")]
    public class EmploymentTests
    {
        [Fact(DisplayName = "CreateEmploymentState")]
        public void CreateEmploymentState_Succeeds()
        {
            var options = new DbContextOptionsBuilder<MainContext>()
                .UseInMemoryDatabase(databaseName: "CreateEmploymentState_adds_to_context")
                .Options;

            // Run the test against one instance of the context
            using (var context = new MainContext(options))
            {
                var sut = new MainRepository(context);
                var guid = Guid.NewGuid();
                var contactGuid = Guid.NewGuid();
                var companyRoleGuid = Guid.NewGuid();

                var state = sut.CreateEmploymentState(guid, contactGuid, companyRoleGuid);

                Assert.Equal(state, context.EmploymentStates.Local.First());

                sut.PersistChanges();

                Assert.Equal(state, context.EmploymentStates.First());
                Assert.Equal(1, context.EmploymentStates.Count());
                Assert.Equal(guid, context.EmploymentStates.First().Guid);
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new MainContext(options))
            {
                Assert.Equal(1, context.EmploymentStates.Count());
            }
        }

        [Fact(DisplayName = "GetEmploymentState")]
        public void GetEmploymentState_Succeeds_OnlyWhenGuidMatchesExisting()
        {
            var guid = Guid.NewGuid();
            var contactGuid = Guid.NewGuid();
            var companyRoleGuid = Guid.NewGuid();
            var invalidGuid = Guid.NewGuid();
            var inMemoryDatabaseBuilder = new InMemoryDatabaseBuilder();
            var options = inMemoryDatabaseBuilder.WithEmploymentState(guid, contactGuid, companyRoleGuid).Build("GetEmploymentState");
            // Run the test against a clean instance of the context
            using (var context = new MainContext(options))
            {
                inMemoryDatabaseBuilder.InitializeContext(context);
                var sut = new MainRepository(context);
                var state = sut.GetEmploymentState(guid);
                var invalidState = sut.GetEmploymentState(invalidGuid);

                Assert.Equal(contactGuid, state.ContactGuid);
                Assert.Equal(companyRoleGuid, state.CompanyRoleGuid);

                Assert.Null(invalidState);
            }
        }
        [Fact(DisplayName = "DeleteEmploymentState")]
        public void CanDeleteEmploymentState()
        {
            var guid = Guid.NewGuid();
            var contactGuid = Guid.NewGuid();
            var companyRoleGuid = Guid.NewGuid();
            var inMemoryDatabaseBuilder = new InMemoryDatabaseBuilder();
            var options = inMemoryDatabaseBuilder.WithEmploymentState(guid, contactGuid, companyRoleGuid).Build("DeleteEmploymentState", true);
            // Run the test against a clean instance of the context
            using (var context = new MainContext(options))
            {
                inMemoryDatabaseBuilder.InitializeContext(context);
                var sut = new MainRepository(context);
                var state = context.EmploymentStates.Find(guid);
                Assert.Equal(EntityState.Unchanged, context.Entry(state).State);

                sut.DeleteEmploymentState(guid);

                Assert.Equal(EntityState.Deleted, context.Entry(state).State);
            }
        }
    }
    public class EmploymentStateBuilder : EntityStateBuilder<EmploymentState>
    {
    }
}
