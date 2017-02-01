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
    [Trait("Entity", "MainRepository")]
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
    }
    public class CompanyStateBuilder : EntityStateBuilder<CompanyState>
    {
    }
}
