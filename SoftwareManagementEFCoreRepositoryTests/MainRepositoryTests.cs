using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using SoftwareManagementEFCoreRepository;
using CommandsShared;
using System.Collections.Generic;
using ProductsShared;

namespace SoftwareManagementEFCoreRepositoryTests
{
    [Trait("Entity", "MainRepository")]
    public class MainRepositoryTests
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

        [Fact(DisplayName = "GetProductState")]
        public void GetProductState_Succeeds_OnlyWhenGuidMatchesExisting()
        {
            var guid = Guid.NewGuid();
            var invalidGuid = Guid.NewGuid();
            const string name = "Cool Product";
            var options = new InMemoryDatabaseBuilder().WithProductState(guid, name).Build("GetProductState");
            // Run the test against a clean instance of the context
            using (var context = new MainContext(options))
            {
                var sut = new MainRepository(context);
                var state = sut.GetProductState(guid);
                var invalidState = sut.GetProductState(invalidGuid);

                Assert.Equal(name, state.Name);

                Assert.Null(invalidState);
            }
        }
    }

    public class InMemoryDatabaseBuilder
    {
        private List<ProductState> _productStates = new List<ProductState>();

        public InMemoryDatabaseBuilder WithDefaultProductStates()
        {
            _productStates.AddRange(new List<ProductState> {
                new ProductStateBuilder().WithName("Nice Suite").Build(),
                new ProductStateBuilder().WithName("Cool Suite").Build(),
                new ProductStateBuilder().WithName("Expensive Suite").Build()
            });
            return this;
        }

        public InMemoryDatabaseBuilder WithProductState(Guid guid, string name)
        {
            var state = new ProductStateBuilder().WithGuid(guid).WithName(name).Build();
            _productStates.Add(state);
            return this;
        }

        public DbContextOptions<MainContext> Build(string databaseName)
        {
            var options = new DbContextOptionsBuilder<MainContext>()
    .UseInMemoryDatabase(databaseName: databaseName)
    .Options;

            using (var context = new MainContext(options))
            {
                var sut = new MainRepository(context);
                context.ProductStates.AddRange(_productStates);
                sut.PersistChanges();
            }

            return options;
        }
    }

    public class ProductStateBuilder : EntityStateBuilder<ProductState>
    {
    }
    public class EntityStateBuilder<T> where T : IEntityState, new()
    {
        private Guid _guid;
        private string _name;

        public EntityStateBuilder<T> WithGuid(Guid guid)
        {
            _guid = guid;
            return this;
        }

        public EntityStateBuilder<T> WithName(string name)
        {
            _name = name;
            return this;
        }

        public T Build()
        {
            EnsureGuid();
            var state = new T();
            state.Guid = _guid;
            state.Name = _name;
            return state;
        }

        private void EnsureGuid()
        {
            if (_guid == null || _guid == Guid.Empty)
            {
                _guid = Guid.NewGuid();
            }
        }
    }
}
