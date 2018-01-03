using CommandsShared;
using Microsoft.EntityFrameworkCore;
using ProductsShared;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ProjectsShared;
using ContactsShared;
using CompaniesShared;
using EmploymentsShared;
using ProjectRoleAssignmentsShared;
using DesignsShared;
using LinksShared;
using FilesShared;

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
    public DbSet<ProductVersionState> ProductVersionStates { get; set; }
    public DbSet<ProductFeatureState> ProductFeatureStates { get; set; }
    public DbSet<ProjectState> ProjectStates { get; set; }
    public DbSet<ProjectRoleState> ProjectRoleStates { get; set; }
    public DbSet<CommandState> CommandStates { get; set; }
    public DbSet<ContactState> ContactStates { get; set; }
    public DbSet<CompanyState> CompanyStates { get; set; }
    public DbSet<CompanyRoleState> CompanyRoleStates { get; set; }
    public DbSet<EmploymentState> EmploymentStates { get; set; }
    public DbSet<CompanyEnvironmentState> CompanyEnvironmentStates { get; set; }
    public DbSet<CompanyEnvironmentHardwareState> CompanyEnvironmentHardwareStates { get; set; }
    public DbSet<ProjectRoleAssignmentState> ProjectRoleAssignmentStates { get; set; }
    public DbSet<LinkState> LinkStates { get; set; }
    public DbSet<FileState> FileStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<ProjectState>()
          .HasMany(h => (ICollection<ProjectRoleState>)h.ProjectRoleStates)
          .WithOne()
          .HasForeignKey(p => p.ProjectGuid);

      modelBuilder.Entity<CompanyState>()
          .HasMany(h => (ICollection<CompanyRoleState>)h.CompanyRoleStates)
          .WithOne()
          .HasForeignKey(p => p.CompanyGuid);

      modelBuilder.Entity<CompanyState>()
          .HasMany(h => (ICollection<CompanyEnvironmentState>)h.CompanyEnvironmentStates)
          .WithOne()
          .HasForeignKey(p => p.CompanyGuid);

      modelBuilder.Entity<ProductState>()
          .HasMany(h => (ICollection<ProductVersionState>)h.ProductVersionStates)
          .WithOne()
          .HasForeignKey(p => p.ProductGuid);

      modelBuilder.Entity<ProductState>()
          .HasMany(h => (ICollection<ProductFeatureState>)h.ProductFeatureStates)
          .WithOne()
          .HasForeignKey(p => p.ProductGuid);

      modelBuilder.Entity<ProductState>()
          .HasMany(h => (ICollection<ProductIssueState>)h.ProductIssueStates)
          .WithOne()
          .HasForeignKey(p => p.ProductGuid);

      modelBuilder.Entity<CompanyEnvironmentState>()
        .HasMany(h => (ICollection<CompanyEnvironmentHardwareState>)h.HardwareStates)
        .WithOne()
        .HasForeignKey(p => p.EnvironmentGuid);

      modelBuilder.Entity<LinkState>()
        .HasIndex(i => i.ForGuid);

      modelBuilder.Entity<FileState>()
        .HasIndex(i => i.ForGuid);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (!optionsBuilder.IsConfigured)
      {
        optionsBuilder.UseSqlServer(@"Server=localhost;Database=SoftwareManagement;Trusted_Connection=True;");
        //               optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");
      }
    }
  }
  public abstract class NamedEntityState : INamedEntityState
  {
    [Key]
    public Guid Guid { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public string Name { get; set; }
  }

  public class ProductVersionState : NamedEntityState, IProductVersionState
  {
    public int Major { get; set; }
    public int Minor { get; set; }
    public int Revision { get; set; }
    public int Build { get; set; }
    public Guid ProductGuid { get; set; }
  }
  public class ProductFeatureState : NamedEntityState, IProductFeatureState
  {
    public Guid ProductGuid { get; set; }
    public string Description { get; set; }
    public bool IsRequest { get; set; }
    public Guid? FirstVersionGuid { get; set; }
    public Guid? RequestedForVersionGuid { get; set; }
  }
  public class ProductIssueState : NamedEntityState, IProductIssueState
  {
    public Guid ProductGuid { get; set; }
    public string Description { get; set; }
    public Guid FirstVersionGuid { get; set; }
  }

  public class ProductState : NamedEntityState, IProductState
  {
    public string Description { get; set; }
    public string BusinessCase { get; set; }
    public ICollection<IProductVersionState> ProductVersionStates { get; set; }
    public ICollection<IProductFeatureState> ProductFeatureStates { get; set; }
    public ICollection<IProductIssueState> ProductIssueStates { get; set; }
  }

  public class ProjectRoleState : NamedEntityState, IProjectRoleState
  {
    public Guid ProjectGuid { get; set; }
  }

  public class ContactState : NamedEntityState, IContactState
  {
    public DateTime? BirthDate { get; set; }
    public string Email { get; set; }
    public Guid? AvatarFileGuid { get; set; }
    public string AvatarUrl { get; set; }
  }

  public class CompanyState : NamedEntityState, ICompanyState
  {
    public CompanyState()
    {
      CompanyRoleStates = new List<ICompanyRoleState>() as ICollection<ICompanyRoleState>;
      CompanyEnvironmentStates = new List<ICompanyEnvironmentState>() as ICollection<ICompanyEnvironmentState>;
    }
    public ICollection<ICompanyRoleState> CompanyRoleStates { get; set; }
    public ICollection<ICompanyEnvironmentState> CompanyEnvironmentStates { get; set; }
  }

  public class CompanyRoleState : NamedEntityState, ICompanyRoleState
  {
    public Guid CompanyGuid { get; set; }
  }
  public class CompanyEnvironmentState : NamedEntityState, ICompanyEnvironmentState
  {
    public Guid CompanyGuid { get; set; }
    public string Url { get; set; }
    public ICollection<ICompanyEnvironmentHardwareState> HardwareStates { get; set; }
  }
  public class CompanyEnvironmentHardwareState : NamedEntityState, ICompanyEnvironmentHardwareState
  {
    public Guid EnvironmentGuid { get; set; }
    public Guid CompanyGuid { get; set; }
    public string IpAddress { get; set; }
  }
  public class ProjectState : NamedEntityState, IProjectState
  {
    public ProjectState()
    {
      ProjectRoleStates = new List<IProjectRoleState>() as ICollection<IProjectRoleState>;
    }
    public DateTime? EndDate { get; set; }
    public DateTime? StartDate { get; set; }
    public ICollection<IProjectRoleState> ProjectRoleStates { get; set; }
  }

  public class EmploymentState : IEmploymentState
  {
    [Key]
    public Guid Guid { get; set; }
    public Guid ContactGuid { get; set; }
    public Guid CompanyRoleGuid { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string ContactName { get; set; }
  }

  public class ProjectRoleAssignmentState : IProjectRoleAssignmentState
  {
    [Key]
    public Guid Guid { get; set; }
    public Guid ContactGuid { get; set; }
    public Guid ProjectGuid { get; set; }
    public Guid ProjectRoleGuid { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string ContactName { get; set; }
  }

  public class LinkState : ILinkState
  {
    [Key]
    public Guid Guid { get; set; }
    public Guid EntityGuid { get; set; }
    public string Url { get; set; }
    public Guid ForGuid { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string SiteName { get; set; }
    public string Name { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
  }
  public class FileState : IFileState
  {
    [Key]
    public Guid Guid { get; set; }
    public Guid EntityGuid { get; set; } // todo: not used. Maybe fill with main entity? Otherwise remove.

    public string FolderName { get; set; }

    public Guid ForGuid { get; set; }
    public string ForType { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
    public string Name { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
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
  // just because the repositories can each be separate, doesn't mean we always want to
  public interface IMainRepository : IProductStateRepository, IContactStateRepository,
      IProjectStateRepository, ICompanyStateRepository, ICommandStateRepository, IEmploymentStateRepository, IDesignStateRepository, ILinkStateRepository, IFileStateRepository, IProjectRoleAssignmentStateRepository
  { }
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


    public ICommandState CreateCommandState()
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

    public void DeleteProductState(Guid guid)
    {
      var state = GetProductState(guid) as ProductState;
      _context.ProductStates.Remove(state);
    }

    public IProductState CreateProductState(Guid guid, string name)
    {
      var state = new ProductState()
      {
        Guid = guid,
        Name = name
      };
      _context.ProductStates.Add(state);
      return state;
    }

    public IProjectState CreateProjectState(Guid guid, string name)
    {
      var state = new ProjectState()
      {
        Guid = guid,
        Name = name
      };
      _context.ProjectStates.Add(state);
      return state;
    }

    public IProductState GetProductState(Guid guid)
    {
      return _context.ProductStates.Find(guid);
    }

    public IProjectState GetProjectState(Guid guid)
    {
      return _context.ProjectStates.Include(s => s.ProjectRoleStates).SingleOrDefault(s => s.Guid == guid);
    }

    public IProjectState GetProjectStateReadOnly(Guid guid)
    {
      return _context.ProjectStates.Include(s => s.ProjectRoleStates).AsNoTracking().SingleOrDefault(s => s.Guid == guid);
    }


    public IEnumerable<IProductState> GetProductStates()
    {
      // todo: make a separate readonly repo for the query part of CQRS
      return _context.ProductStates.Include(s => s.ProductVersionStates).Include(s => s.ProductIssueStates).Include(s => s.ProductFeatureStates).AsNoTracking().ToList();
    }

    public IEnumerable<IProjectState> GetProjectStates()
    {
      // todo: make a separate readonly repo for the query part of CQRS
      return _context.ProjectStates.Include(s => s.ProjectRoleStates).AsNoTracking().ToList();
    }

    public IProjectRoleState CreateProjectRoleState(Guid projectGuid, Guid guid, string name)
    {
      var newState = new ProjectRoleState { ProjectGuid = projectGuid, Guid = guid, Name = name };
      _context.ProjectRoleStates.Add(newState);
      return newState;

    }

    public void AddRoleToProjectState(Guid projectGuid, Guid projectRoleGuid, string projectRoleName)
    {
      var projectState = GetProjectState(projectGuid);

      if (projectState.ProjectRoleStates.All(w => w.Guid != projectRoleGuid))
      {
        CreateProjectRoleState(projectGuid, projectRoleGuid, projectRoleName);
      }
    }

    public void RemoveRoleFromProjectState(Guid projectGuid, Guid projectRoleGuid)
    {
      var projectState = GetProjectState(projectGuid);
      var projectRoleState = projectState.ProjectRoleStates.SingleOrDefault(s => s.Guid == projectRoleGuid);
      if (projectRoleState != null)
      {
        projectState.ProjectRoleStates.Remove(projectRoleState);
        _context.ProjectRoleStates.Remove((ProjectRoleState)projectRoleState);
      }
    }

    public ICompanyRoleState CreateCompanyRoleState(Guid companyGuid, Guid guid, string name)
    {
      var newState = new CompanyRoleState { CompanyGuid = companyGuid, Guid = guid, Name = name };
      _context.CompanyRoleStates.Add(newState);
      return newState;

    }
    public ICompanyEnvironmentState CreateCompanyEnvironmentState(Guid companyGuid, Guid guid, string name)
    {
      var newState = new CompanyEnvironmentState { CompanyGuid = companyGuid, Guid = guid, Name = name };
      _context.CompanyEnvironmentStates.Add(newState);
      return newState;

    }
    public ICompanyEnvironmentHardwareState CreateCompanyEnvironmentHardwareState(Guid companyGuid, Guid environmentGUid, Guid guid, string name)
    {
      var newState = new CompanyEnvironmentHardwareState { CompanyGuid = companyGuid, EnvironmentGuid = environmentGUid, Guid = guid, Name = name };
      _context.CompanyEnvironmentHardwareStates.Add(newState);
      return newState;

    }

    public ICompanyRoleState AddRoleToCompanyState(Guid companyGuid, Guid companyRoleGuid, string companyRoleName)
    {
      var companyState = GetCompanyState(companyGuid);
      var roleState = companyState.CompanyRoleStates.SingleOrDefault(s => s.Guid == companyRoleGuid);
      if (roleState == null)
      {
        roleState = CreateCompanyRoleState(companyGuid, companyRoleGuid, companyRoleName);
      }
      return roleState;
    }

    public void RemoveRoleFromCompanyState(Guid companyGuid, Guid companyRoleGuid)
    {
      var companyState = GetCompanyState(companyGuid);
      var companyRoleState = companyState.CompanyRoleStates.SingleOrDefault(s => s.Guid == companyRoleGuid);
      if (companyRoleState != null)
      {
        companyState.CompanyRoleStates.Remove(companyRoleState);
        _context.CompanyRoleStates.Remove((CompanyRoleState)companyRoleState);
      }
    }

    public void PersistChanges()
    {
      _context.SaveChanges();
    }

    public Task PersistChangesAsync()
    {
      return _context.SaveChangesAsync();
    }

    public IContactState CreateContactState(Guid guid, string name)
    {
      var state = new ContactState { Guid = guid, Name = name };
      _context.ContactStates.Add(state);
      return state;
    }

    public IContactState GetContactState(Guid guid)
    {
      var state = _context.ContactStates.Find(guid);
      return state;
    }

    public IEnumerable<IContactState> GetContactStates()
    {
      return _context.ContactStates.AsNoTracking().ToList();
    }

    public void DeleteContactState(Guid guid)
    {
      var state = _context.ContactStates.Find(guid);
      _context.ContactStates.Remove(state);
    }

    public ICompanyState CreateCompanyState(Guid guid, string name)
    {
      var state = new CompanyState { Guid = guid, Name = name };
      _context.CompanyStates.Add(state);
      return state;
    }

    public ICompanyState GetCompanyState(Guid guid)
    {
      var state = _context.CompanyStates.Include(i => i.CompanyRoleStates).Include(i => i.CompanyEnvironmentStates).ThenInclude(ti => ti.HardwareStates).SingleOrDefault(s => s.Guid == guid);
      return state;
    }

    public IEnumerable<ICompanyState> GetCompanyStates()
    {
      return _context.CompanyStates.Include(i => i.CompanyRoleStates).Include(i => i.CompanyEnvironmentStates).ThenInclude(ti => ti.HardwareStates).AsNoTracking().ToList();
    }

    public void DeleteCompanyState(Guid guid)
    {
      var state = _context.CompanyStates.Find(guid);
      _context.CompanyStates.Remove(state);
    }

    public IEmploymentState CreateEmploymentState(Guid guid, Guid contactGuid, Guid companyRoleGuid)
    {
      var state = new EmploymentState()
      {
        Guid = guid,
        ContactGuid = contactGuid,
        CompanyRoleGuid = companyRoleGuid
      };
      _context.EmploymentStates.Add(state);
      return state;
    }

    public IEmploymentState GetEmploymentState(Guid guid)
    {
      return _context.EmploymentStates.Find(guid);
    }

    public void DeleteEmploymentState(Guid entityGuid)
    {
      var state = GetEmploymentState(entityGuid) as EmploymentState;
      _context.EmploymentStates.Remove(state);
    }

    public IEnumerable<IEmploymentState> GetEmploymentsByCompanyRoleGuid(Guid companyRoleGuid)
    {
      var states = _context.EmploymentStates.AsNoTracking().Where(w => w.CompanyRoleGuid == companyRoleGuid).ToList();
      return states as ICollection<IEmploymentState>;
    }

    public IEnumerable<IEmploymentState> GetEmploymentsByContactGuid(Guid contactGuid)
    {
      var states = _context.EmploymentStates.AsNoTracking().Where(w => w.ContactGuid == contactGuid).ToList();
      return states as ICollection<IEmploymentState>;
    }

    public IEnumerable<IEmploymentState> GetEmploymentStates()
    {
      var states = _context.EmploymentStates.AsNoTracking().ToList();
      return states as ICollection<IEmploymentState>;
    }

    public IEnumerable<ICommandState> GetCommandStates(Guid guid)
    {
      // todo: consider which date I want to use. Ideally the created on reflects the time the user created the command correctly. Ideally ...
      var states = _context.CommandStates.Where(w => w.EntityGuid == guid).OrderByDescending(o => o.CreatedOn).AsNoTracking().ToList();
      return states;
    }

    public IEnumerable<IContactState> GetContactsByCompanyRoleGuid(Guid companyRoleGuid)
    {
      throw new NotImplementedException();
    }

    public IProductVersionState CreateProductVersionState(Guid guid, Guid productVersionGuid, string name)
    {
      var state = new ProductVersionState { Guid = productVersionGuid, ProductGuid = guid, Name = name };
      _context.ProductVersionStates.Add(state);
      return state;
    }

    public ICompanyEnvironmentState AddEnvironmentToCompanyState(Guid companyGuid, Guid companyEnvironmentGuid, string companyEnvironmentName)
    {
      var companyState = GetCompanyState(companyGuid);
      var state = companyState.CompanyEnvironmentStates.SingleOrDefault(s => s.Guid == companyEnvironmentGuid);
      if (state == null)
      {
        state = CreateCompanyEnvironmentState(companyGuid, companyEnvironmentGuid, companyEnvironmentName);
      }
      return state;
    }

    public void RemoveEnvironmentFromCompanyState(Guid companyGuid, Guid companyEnvironmentGuid)
    {
      var companyState = GetCompanyState(companyGuid);
      var companyEnvironmentState = companyState.CompanyEnvironmentStates.SingleOrDefault(s => s.Guid == companyEnvironmentGuid);
      if (companyEnvironmentState != null)
      {
        companyState.CompanyEnvironmentStates.Remove(companyEnvironmentState);
        _context.CompanyEnvironmentStates.Remove((CompanyEnvironmentState)companyEnvironmentState);
      }
    }

    public ICompanyEnvironmentState GetEnvironmentState(Guid companyGuid, Guid environmentGuid)
    {
      return _context.CompanyEnvironmentStates.Include(i => i.HardwareStates).SingleOrDefault(s => s.Guid == environmentGuid);
    }

    public IProductFeatureState CreateProductFeatureState(Guid guid, Guid productFeatureGuid, string name)
    {
      throw new NotImplementedException();
    }

    public void DeleteProductFeatureState(Guid productGuid, Guid guid)
    {
      throw new NotImplementedException();
    }

    public void DeleteProductVersionState(Guid productGuid, Guid guid)
    {
      throw new NotImplementedException();
    }

    public IProductIssueState CreateProductIssueState(Guid productGuid, Guid guid, string name)
    {
      throw new NotImplementedException();
    }

    public void DeleteProductIssueState(Guid productGuid, Guid guid)
    {
      throw new NotImplementedException();
    }

    public ICompanyEnvironmentHardwareState AddHardwareToEnvironmentState(ICompanyEnvironmentState companyEnvironmentState, Guid hardwareGuid, string hardwareName)
    {
      var state = companyEnvironmentState.HardwareStates.SingleOrDefault(s => s.Guid == hardwareGuid);
      if (state == null)
      {
        state = CreateCompanyEnvironmentHardwareState(companyEnvironmentState.CompanyGuid, companyEnvironmentState.Guid, hardwareGuid, hardwareName);
      }
      return state;
    }

    public void RemoveHardwareFromEnvironmentState(ICompanyEnvironmentState state, Guid hardwareGuid)
    {
      var hardwareState = GetHardwareForEnvironmentState(state, hardwareGuid);
      if (hardwareState != null)
      {
        state.HardwareStates.Remove(hardwareState);
        _context.CompanyEnvironmentHardwareStates.Remove((CompanyEnvironmentHardwareState)hardwareState);
      }
    }

    public ICompanyEnvironmentHardwareState GetHardwareForEnvironmentState(ICompanyEnvironmentState state, Guid hardwareGuid)
    {
      return state.HardwareStates?.SingleOrDefault(s => s.Guid == hardwareGuid);
    }

    public IDesignState CreateDesignState(Guid guid, string name)
    {
      throw new NotImplementedException();
    }

    public IEpicElementState CreateEpicElementState(Guid designGuid, Guid guid, string name)
    {
      throw new NotImplementedException();
    }

    public IDesignState GetDesignState(Guid guid)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<IDesignState> GetDesignStates()
    {
      throw new NotImplementedException();
    }

    public void DeleteDesignState(Guid guid)
    {
      throw new NotImplementedException();
    }

    public IEntityElementState CreateEntityElementState(Guid designGuid, Guid epicGuid, Guid entityGuid, string name)
    {
      throw new NotImplementedException();
    }

    public IPropertyElementState CreatePropertyElementState(Guid designGuid, Guid epicGuid, Guid entityGuid, Guid guid, string name)
    {
      throw new NotImplementedException();
    }

    public ICommandElementState CreateCommandElementState(Guid designGuid, Guid epicGuid, Guid entityGuid, Guid guid, string name)
    {
      throw new NotImplementedException();
    }

    public ILinkState CreateLinkState(Guid guid, string name)
    {
      var state = new LinkState { Guid = guid, Name = name };
      _context.LinkStates.Add(state);
      return state;
    }

    public ILinkState GetLinkState(Guid guid)
    {
      return _context.LinkStates.SingleOrDefault(s => s.Guid == guid);
    }

    public IEnumerable<ILinkState> GetLinkStates()
    {
      return _context.LinkStates.AsNoTracking().ToList();
    }

    public IEnumerable<ILinkState> GetLinkStatesForGuid(Guid forGuid)
    {
      return _context.LinkStates.Where(s => s.ForGuid == forGuid).AsNoTracking().ToList();
    }

    public void DeleteLinkState(Guid guid)
    {
      var state = GetLinkState(guid);
      if (state != null)
      {
        _context.LinkStates.Remove((LinkState)state);
      }
    }

    public IFileState CreateFileState(Guid guid, string name)
    {
      var state = new FileState { Guid = guid, Name = name };
      _context.FileStates.Add(state);
      return state;
    }

    public IFileState GetFileState(Guid guid)
    {
      return _context.FileStates.SingleOrDefault(s => s.Guid == guid);
    }

    public IEnumerable<IFileState> GetFileStates()
    {
      return _context.FileStates.AsNoTracking().ToList();
    }

    public IEnumerable<IFileState> GetFileStatesForGuid(Guid forGuid)
    {
      return _context.FileStates.Where(s => s.ForGuid == forGuid).AsNoTracking().ToList();
    }

    public void DeleteFileState(Guid guid)
    {
      var state = GetFileState(guid);
      if (state != null)
      {
        _context.FileStates.Remove((FileState)state);
      }
    }

    public IProjectRoleAssignmentState CreateProjectRoleAssignmentState(Guid guid, Guid contactGuid, Guid projectGuid, Guid companyRoleGuid)
    {
      throw new NotImplementedException();
    }

    public IProjectRoleAssignmentState GetProjectRoleAssignmentState(Guid guid)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<IProjectRoleAssignmentState> GetProjectRoleAssignmentsByProjectRoleGuid(Guid companyRoleGuid)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<IProjectRoleAssignmentState> GetProjectRoleAssignmentsByContactGuid(Guid contactGuid)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<IContactState> GetContactsByProjectRoleGuid(Guid companyRoleGuid)
    {
      throw new NotImplementedException();
    }

    public void DeleteProjectRoleAssignmentState(Guid entityGuid)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<IProjectRoleAssignmentState> GetProjectRoleAssignmentStates()
    {
      throw new NotImplementedException();
    }

    public IEnumerable<IContactState> GetContactsByProjectGuid(Guid guid)
    {
      throw new NotImplementedException();
    }
  }
}
