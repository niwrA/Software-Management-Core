using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using SoftwareManagementEFCoreRepository;

namespace SoftwareManagementEFCoreRepositoryTests
{
    public class MainRepositoryTests
    {
        [Fact]
        public void CreateProductState_Succeeds()
        {
            var options = new DbContextOptionsBuilder<MainContext>()
                .UseInMemoryDatabase(databaseName: "CreateProductState_adds_to_context")
                .Options;

            // Run the test against one instance of the context
            using (var context = new MainContext(options))
            {
                var sut = new MainRepository(context);
                var guid = Guid.NewGuid();
                var state = sut.CreateProductState(guid);
                Assert.Equal(state, context.ProductStates.Local.First());
                sut.PersistChanges();
                Assert.Equal(state, context.ProductStates.First());
                Assert.Equal(1, context.ProductStates.Count());
                Assert.Equal(guid, context.ProductStates.First().Guid);
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new MainContext(options))
            {
                Assert.Equal(1, context.ProductStates.Count());
            }
        }
    }
}
