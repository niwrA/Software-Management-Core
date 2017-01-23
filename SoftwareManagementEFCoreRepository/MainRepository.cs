using CommandsShared;
using Microsoft.EntityFrameworkCore;
using ProductsShared;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ProjectsShared;

namespace SoftwareManagementEFCoreRepository
{
    public class MainContext : DbContext
    {
        public MainContext()
        {

        }
        public MainContext(DbContextOptions<MainContext> options) : base(options)
        {
        }
        public DbSet<ProductState> ProductStates { get; set; }
        public DbSet<ProjectState> ProjectStates { get; set; }
        public DbSet<CommandState> CommandStates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=localhost;Database=SoftwareManagement;Trusted_Connection=True;");
                //               optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");
            }
        }
    }
    public abstract class NamedEntityState: IEntityState
    {
        [Key]
        public Guid Guid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string Name { get; set; }
    }

    public class ProductState : NamedEntityState, IProductState { }
    public class ProjectState : NamedEntityState, IProjectState
    {
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
    }

    public class CommandState : ICommandState
    {
        [Key]
        public Guid Guid { get; set; }
        public Guid EntityGuid { get; set; }
        public string CommandTypeId { get; set; }
        public DateTime? ExecutedOn { get; set; }
        public string ParametersJson { get; set; }
        public DateTime? ReceivedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UserName { get; set; }
    }
    public interface IMainRepository : IProductStateRepository, IProjectStateRepository, ICommandRepository { };
    public class MainRepository : IMainRepository
    {
        private MainContext _context;
        public MainRepository(MainContext context)
        {
            _context = context;
            //if (_context.Database.GetPendingMigrations().Any())
            //{
            //    _context.Database.Migrate();
            //}
        }

        public void Add(ICommandState state)
        {
            throw new NotImplementedException();
        }

        public ICommandState Create()
        {
            var state = new CommandState()
            {
                Guid = Guid.NewGuid()
            };
            _context.CommandStates.Add(state);
            return state;
        }

        public void DeleteProjectState(Guid guid)
        {
            var state = GetProjectState(guid) as ProjectState;
            _context.ProjectStates.Remove(state);
        }

        public IProductState CreateProductState(Guid guid)
        {
            var state = new ProductState()
            {
                Guid = guid
            };
            _context.ProductStates.Add(state);
            return state;
        }

        public IProjectState CreateProjectState(Guid guid)
        {
            var state = new ProjectState()
            {
                Guid = guid
            };
            _context.ProjectStates.Add(state);
            return state;
        }

        public bool Exists(Guid guid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICommandState> GetAllNew()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICommandState> GetAllProcessed()
        {
            throw new NotImplementedException();
        }

        public IProductState GetProductState(Guid guid)
        {
            return _context.ProductStates.Find(guid);
        }

        public IProjectState GetProjectState(Guid guid)
        {
            return _context.ProjectStates.Find(guid);
        }

        public IEnumerable<IProjectState> GetProjectStates()
        {
            // todo: make a separate readonly repo for the query part of CQRS
            return _context.ProjectStates.AsNoTracking().ToList(); 
        }

        public IList<ICommandState> GetUpdatesSinceLast(long lastReceivedStamp)
        {
            throw new NotImplementedException();
        }

        public void PersistChanges()
        {
            _context.SaveChanges();
        }

        public Task PersistChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void SetProcessed(ICommandState state)
        {
//            state.UserName = 
            //throw new NotImplementedException();
        }
    }
}
