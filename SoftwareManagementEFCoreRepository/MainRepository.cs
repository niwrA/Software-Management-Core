using Microsoft.EntityFrameworkCore;
using ProductsShared;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;");
                //               optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");
            }
        }
    }

    public class ProductState : IProductState
    {
        public ProductState()
        {

        }
        public ProductState(Guid guid)
        {
            Guid = guid;
        }
        public DateTime Created { get; set; }
        [Key]
        public Guid Guid { get; set; }
        public string Name { get; set; }
    }
    public class MainRepository : IProductStateRepository
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
        public IProductState CreateProductState(Guid guid)
        {
            var state = new ProductState(guid);
            _context.ProductStates.Add(state);
            return state;
        }

        public IProductState GetProductState(Guid guid)
        {
            return _context.ProductStates.Find(guid);
        }

        public void PersistChanges()
        {
            _context.SaveChanges();
        }

        public Task PersistChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
