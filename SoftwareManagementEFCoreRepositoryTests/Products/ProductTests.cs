using Microsoft.EntityFrameworkCore;
using SoftwareManagementEFCoreRepository;
using SoftwareManagementEFCoreRepositoryTests.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SoftwareManagementEFCoreRepositoryTests.Products
{
    [Trait("EFCore", "ProductState")]
    public class ProductTests
    {
        [Fact(DisplayName = "CreateProductState")]
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
                var name = "new";
                var state = sut.CreateProductState(guid, name);

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

        [Fact(DisplayName = "GetProductState")]
        public void GetProductState_Succeeds_OnlyWhenGuidMatchesExisting()
        {
            var guid = Guid.NewGuid();
            var invalidGuid = Guid.NewGuid();
            const string name = "Cool Product";
            var inMemoryDatabaseBuilder = new InMemoryDatabaseBuilder();
            var options = inMemoryDatabaseBuilder.WithProductState(guid, name).Build("GetProductState");
            // Run the test against a clean instance of the context
            using (var context = new MainContext(options))
            {
                inMemoryDatabaseBuilder.InitializeContext(context);
                var sut = new MainRepository(context);
                var state = sut.GetProductState(guid);
                var invalidState = sut.GetProductState(invalidGuid);

                Assert.Equal(name, state.Name);

                Assert.Null(invalidState);
            }
        }
    }

    public class ProductStateBuilder : EntityStateBuilder<ProductState>
    {
    }
}
