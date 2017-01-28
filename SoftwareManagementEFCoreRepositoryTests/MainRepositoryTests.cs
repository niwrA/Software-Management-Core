using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using SoftwareManagementEFCoreRepository;
using CommandsShared;
using System.Collections.Generic;
using ProductsShared;
using Microsoft.Data.Sqlite;

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



    public class InMemoryDatabaseBuilder
    {
        private List<ProductState> _productStates = new List<ProductState>();
        private List<ProjectState> _projectStates = new List<ProjectState>();
        private List<ProjectRoleState> _productRoleStates = new List<ProjectRoleState>();

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

        public InMemoryDatabaseBuilder WithProductRoleState(Guid guid, string name)
        {
            var state = new ProjectRoleStateBuilder().WithGuid(guid).WithName(name).Build();
            _productRoleStates.Add(state);
            return this;
        }

        public DbContextOptions<MainContext> Build(string databaseName, bool? useSqlLite = false)
        {
            DbContextOptionsBuilder<MainContext> contextOptionsBuilder;
            contextOptionsBuilder = new DbContextOptionsBuilder<MainContext>();
            DbContextOptions<MainContext> options;

            if (useSqlLite.HasValue && useSqlLite.Value)
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                options = new DbContextOptionsBuilder<MainContext>()
                    .UseSqlite(connection)
                    .Options;
            }
            else
            {
                options = contextOptionsBuilder.UseInMemoryDatabase(databaseName: databaseName).Options;
            }

            return options;
        }

        public void InitializeContext(MainContext context)
        {
            context.Database.EnsureCreated();
            var sut = new MainRepository(context);
            context.ProductStates.AddRange(_productStates);
            context.ProjectStates.AddRange(_projectStates);
            sut.PersistChanges();
        }

        public InMemoryDatabaseBuilder WithProjectState(Guid guid, string name)
        {
            var state = new ProjectStateBuilder().WithGuid(guid).WithName(name).Build();
            _projectStates.Add(state);
            return this;
        }
    }

    public class ProjectStateBuilder : EntityStateBuilder<ProjectState>
    {
    }
    public class ProductStateBuilder : EntityStateBuilder<ProductState>
    {
    }
    public class ProjectRoleStateBuilder : EntityStateBuilder<ProjectRoleState>
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
