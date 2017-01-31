using Microsoft.EntityFrameworkCore;
using SoftwareManagementEFCoreRepository;
using SoftwareManagementEFCoreRepositoryTests.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SoftwareManagementEFCoreRepositoryTests.Contacts
{
    [Trait("Entity", "MainRepository")]
    public class ContactTests
    {
        [Fact(DisplayName = "CreateContactState")]
        public void CreateContactState_Succeeds()
        {
            var options = new DbContextOptionsBuilder<MainContext>()
                .UseInMemoryDatabase(databaseName: "CreateContactState_adds_to_context")
                .Options;

            // Run the test against one instance of the context
            using (var context = new MainContext(options))
            {
                var sut = new MainRepository(context);
                var guid = Guid.NewGuid();
                var name = "new";
                var state = sut.CreateContactState(guid, name);

                Assert.Equal(state, context.ContactStates.Local.First());

                sut.PersistChanges();

                Assert.Equal(state, context.ContactStates.First());
                Assert.Equal(1, context.ContactStates.Count());
                Assert.Equal(guid, context.ContactStates.First().Guid);
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new MainContext(options))
            {
                Assert.Equal(1, context.ContactStates.Count());
            }
        }

        [Fact(DisplayName = "GetContactState")]
        public void GetContactState_Succeeds_OnlyWhenGuidMatchesExisting()
        {
            var guid = Guid.NewGuid();
            var invalidGuid = Guid.NewGuid();
            const string name = "Cool Contact";
            var inMemoryDatabaseBuilder = new InMemoryDatabaseBuilder();
            var options = inMemoryDatabaseBuilder.WithContactState(guid, name).Build("GetContactState");
            // Run the test against a clean instance of the context
            using (var context = new MainContext(options))
            {
                inMemoryDatabaseBuilder.InitializeContext(context);
                var sut = new MainRepository(context);
                var state = sut.GetContactState(guid);
                var invalidState = sut.GetContactState(invalidGuid);

                Assert.Equal(name, state.Name);

                Assert.Null(invalidState);
            }
        }
        [Fact(DisplayName = "DeleteContactState")]
        public void CanDeleteContactState()
        {
            var guid = Guid.NewGuid();
            var name = "To be deleted.";
            var inMemoryDatabaseBuilder = new InMemoryDatabaseBuilder();
            var options = inMemoryDatabaseBuilder.WithContactState(guid, name).Build("DeleteContactState", true);
            // Run the test against a clean instance of the context
            using (var context = new MainContext(options))
            {
                inMemoryDatabaseBuilder.InitializeContext(context);
                var sut = new MainRepository(context);
                var state = context.ContactStates.Find(guid);
                Assert.Equal(EntityState.Unchanged, context.Entry(state).State);

                sut.DeleteContactState(guid);

                Assert.Equal(EntityState.Deleted, context.Entry(state).State);
            }
        }
    }
    public class ContactStateBuilder : EntityStateBuilder<ContactState>
    {
    }
}
