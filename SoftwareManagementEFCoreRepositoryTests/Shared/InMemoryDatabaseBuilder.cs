using CommandsShared;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SoftwareManagementEFCoreRepository;
using SoftwareManagementEFCoreRepositoryTests.Contacts;
using SoftwareManagementEFCoreRepositoryTests.Products;
using SoftwareManagementEFCoreRepositoryTests.Projects;
using System;
using System.Collections.Generic;
using System.Text;
using static SoftwareManagementEFCoreRepositoryTests.Products.ProductTests;

namespace SoftwareManagementEFCoreRepositoryTests.Shared
{
    // move the domain specific parts to an inheriting class in the proper namespace
    public class InMemoryDatabaseBuilder
    {
        private List<ProductState> _productStates = new List<ProductState>();
        private List<ProjectState> _projectStates = new List<ProjectState>();
        private List<ProjectRoleState> _projectRoleStates = new List<ProjectRoleState>();
        private List<ContactState> _contactStates = new List<ContactState>();

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

        public InMemoryDatabaseBuilder WithContactState(Guid guid, string name)
        {
            var state = new ContactStateBuilder().WithGuid(guid).WithName(name).Build();
            _contactStates.Add(state);
            return this;
        }

        public InMemoryDatabaseBuilder WithProjectRoleState(Guid guid, string name, Guid projectGuid)
        {
            var state = new ProjectRoleStateBuilder()
                .WithGuid(guid)
                .WithName(name)
                //.WithProjectGuid(projectGuid) <- reported as a bug to microsoft
                .Build();
            state.ProjectGuid = projectGuid; // <- workaround
            _projectRoleStates.Add(state);
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
            context.ProjectRoleStates.AddRange(_projectRoleStates);
            context.ContactStates.AddRange(_contactStates);
            sut.PersistChanges();
        }

        public InMemoryDatabaseBuilder WithProjectState(Guid guid, string name)
        {
            var state = new ProjectStateBuilder().WithGuid(guid).WithName(name).Build();
            _projectStates.Add(state);
            return this;
        }
    }
    public class EntityStateBuilder<T> where T : IEntityState, new()
    {
        private Guid _guid;
        private string _name;
        public Guid Guid { get { return _guid; } }
        public string Name { get { return _name; } }
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

        public virtual T Build()
        {
            EnsureGuid();
            var state = new T();
            state.Guid = _guid;
            state.Name = _name;
            return state;
        }

        public void EnsureGuid()
        {
            if (_guid == null || _guid == Guid.Empty)
            {
                _guid = Guid.NewGuid();
            }
        }
    }



}
