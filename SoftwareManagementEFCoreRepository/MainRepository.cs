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
using ProductInstallationsShared;

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
    public DbSet<CompanyEnvironmentAccountState> CompanyEnvironmentAccountStates { get; set; }
    public DbSet<CompanyEnvironmentDatabaseState> CompanyEnvironmentDatabaseStates { get; set; }
    public DbSet<ProjectRoleAssignmentState> ProjectRoleAssignmentStates { get; set; }
    public DbSet<LinkState> LinkStates { get; set; }
    public DbSet<FileState> FileStates { get; set; }
    public DbSet<ProductIssueState> ProductIssueStates { get; set; }
    public DbSet<DesignState> DesignStates { get; set; }
    public DbSet<DesignElementState> DesignElementStates { get; set; }
    public DbSet<EpicElementState> EpicElementStates { get; set; }
    public DbSet<EntityElementState> EntityElementStates { get; set; }
    public DbSet<PropertyElementState> PropertyElementStates { get; set; }
    public DbSet<CommandElementState> CommandElementStates { get; set; }
    public DbSet<ProductConfigOptionState> ProductConfigOptionStates { get; set; }
    public DbSet<ProductInstallationState> ProductInstallationStates{ get; set; }

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

      modelBuilder.Entity<ProductState>()
          .HasMany(h => (ICollection<ProductConfigOptionState>)h.ProductConfigOptionStates)
          .WithOne()
          .HasForeignKey(p => p.ProductGuid);

      modelBuilder.Entity<CompanyEnvironmentState>()
        .HasMany(h => (ICollection<CompanyEnvironmentHardwareState>)h.HardwareStates)
        .WithOne()
        .HasForeignKey(p => p.EnvironmentGuid);

      modelBuilder.Entity<CompanyEnvironmentState>()
        .HasMany(h => (ICollection<CompanyEnvironmentAccountState>)h.AccountStates)
        .WithOne()
        .HasForeignKey(p => p.EnvironmentGuid);

      modelBuilder.Entity<CompanyEnvironmentState>()
        .HasMany(h => (ICollection<CompanyEnvironmentDatabaseState>)h.DatabaseStates)
        .WithOne()
        .HasForeignKey(p => p.EnvironmentGuid);

      modelBuilder.Entity<LinkState>()
        .HasIndex(i => i.ForGuid);

      modelBuilder.Entity<FileState>()
        .HasIndex(i => i.ForGuid);

      modelBuilder.Entity<DesignState>()
       .HasMany(h => (ICollection<EpicElementState>)h.EpicElementStates)
       .WithOne()
       .HasForeignKey(p => p.DesignGuid);
      modelBuilder.Entity<EpicElementState>()
       .HasMany(h => (ICollection<EntityElementState>)h.EntityElementStates)
       .WithOne()
       .HasForeignKey(p => p.EpicElementGuid);
      modelBuilder.Entity<EntityElementState>()
       .HasMany(h => (ICollection<PropertyElementState>)h.PropertyElementStates)
       .WithOne()
       .HasForeignKey(p => p.EntityElementGuid);
      modelBuilder.Entity<EntityElementState>()
       .HasMany(h => (ICollection<CommandElementState>)h.CommandElementStates)
       .WithOne()
       .HasForeignKey(p => p.EntityElementGuid);

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
  public abstract class NamedEntityState : TimeStampedEntityState, INamedEntityState
  {
    public string Name { get; set; }
  }
  public abstract class TimeStampedEntityState: ITimeStampedEntityState
  {
    [Key]
    public Guid Guid { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }

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
  public class ProductConfigOptionState : NamedEntityState, IProductConfigOptionState
  {
    public Guid ProductGuid { get; set; }
    public Guid? ProductFeatureGuid { get; set; }
    public Guid? ParentGuid { get; set; }
    public string Path { get; set; }
    public string DefaultValue { get; set; }
    public string Description { get; set; }
    public bool IsDefaultOption { get; set; }
    public bool IsOptionForParent { get; set; }
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
    public ICollection<IProductConfigOptionState> ProductConfigOptionStates { get; set; }
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
    public string ExternalId { get; set; }
    public string Code { get; set; }
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
    public ICollection<ICompanyEnvironmentAccountState> AccountStates { get; set; }
    public ICollection<ICompanyEnvironmentDatabaseState> DatabaseStates { get; set; }
  }
  public class CompanyEnvironmentHardwareState : NamedEntityState, ICompanyEnvironmentHardwareState
  {
    public Guid EnvironmentGuid { get; set; }
    public Guid CompanyGuid { get; set; }
    public string IpAddress { get; set; }
  }
  public class CompanyEnvironmentAccountState : NamedEntityState, ICompanyEnvironmentAccountState
  {
    public Guid EnvironmentGuid { get; set; }
    public Guid CompanyGuid { get; set; }
  }
  public class CompanyEnvironmentDatabaseState : NamedEntityState, ICompanyEnvironmentDatabaseState
  {
    public Guid EnvironmentGuid { get; set; }
    public Guid CompanyGuid { get; set; }
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

  public class ProjectRoleAssignmentState : TimeStampedEntityState, IProjectRoleAssignmentState
  {
    public Guid ContactGuid { get; set; }
    public Guid ProjectGuid { get; set; }
    public Guid ProjectRoleGuid { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string ContactName { get; set; }
  }

  public class ProductInstallationState : TimeStampedEntityState, IProductInstallationState
  {
    public Guid CompanyGuid { get; set; }
    public Guid ProductGuid { get; set; }
    public Guid? CompanyEnvironmentGuid { get; set; }
    public Guid? ProductVersionGuid { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string ExternalId { get; set; }
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
  public class DesignState : IDesignState
  {
    [Key]
    public Guid Guid { get; set; }
    public string Description { get; set; }
    public ICollection<IEpicElementState> EpicElementStates { get; set; } = new List<IEpicElementState>();
    public string Name { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
  }
  public class DesignElementState : IDesignElementState
  {
    [Key]
    public Guid Guid { get; set; }
    public string Description { get; set; }
    public Guid DesignGuid { get; set; }
    public string Name { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
  }
  public class EpicElementState : IEpicElementState
  {
    [Key]
    public Guid Guid { get; set; }
    public ICollection<IEntityElementState> EntityElementStates { get; set; }
    public string Description { get; set; }
    public Guid DesignGuid { get; set; }
    public string Name { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
  }
  public class PropertyElementState : IPropertyElementState
  {
    [Key]
    public Guid Guid { get; set; }
    public Guid EpicElementGuid { get; set; }
    public Guid EntityElementGuid { get; set; }
    public string Description { get; set; }
    public Guid DesignGuid { get; set; }
    public string Name { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
  }
  public class EntityElementState : IEntityElementState
  {
    [Key]
    public Guid Guid { get; set; }
    public ICollection<IPropertyElementState> PropertyElementStates { get; set; }
    public ICollection<ICommandElementState> CommandElementStates { get; set; }
    public Guid EpicElementGuid { get; set; }
    public string Description { get; set; }
    public Guid DesignGuid { get; set; }
    public string Name { get; set; }
    public Guid? ParentGuid { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
  }
  public class CommandElementState : ICommandElementState
  {
    [Key]
    public Guid Guid { get; set; }
    public Guid EpicElementGuid { get; set; }
    public Guid EntityElementGuid { get; set; }
    public string Description { get; set; }
    public Guid DesignGuid { get; set; }
    public string Name { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
  }
  public class CommandState : ICommandState
  {
    [Key]
    public Guid Guid { get; set; }
    public Guid EntityGuid { get; set; }
    public string Entity { get; set; }
    public string CommandTypeId { get; set; }
    public DateTime? ExecutedOn { get; set; }
    public string ParametersJson { get; set; }
    public DateTime? ReceivedOn { get; set; }
    public DateTime CreatedOn { get; set; }
    public string UserName { get; set; }
  }
  // just because the repositories can each be separate, doesn't mean we always want to
  public interface IMainRepository : IProductStateRepository, IContactStateRepository,
      IProjectStateRepository, ICompanyStateRepository, ICommandStateRepository,
    IEmploymentStateRepository, IDesignStateRepository, ILinkStateRepository,
    IFileStateRepository, IProjectRoleAssignmentStateRepository, IProductInstallationStateRepository
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
      return _context.ProductStates
        .Include(s => s.ProductFeatureStates)
        .Include(i => i.ProductIssueStates)
        .Include(i => i.ProductVersionStates)
        .Include(i => i.ProductConfigOptionStates)
        .SingleOrDefault(s => s.Guid == guid);
    }

    public IProjectState GetProjectState(Guid guid)
    {
      return _context.ProjectStates
        .Include(s => s.ProjectRoleStates)
        .SingleOrDefault(s => s.Guid == guid);
    }
    // todo: separate read-only repository
    public IProjectState GetProjectStateReadOnly(Guid guid)
    {
      return _context.ProjectStates.Include(s => s.ProjectRoleStates).AsNoTracking().SingleOrDefault(s => s.Guid == guid);
    }


    public IEnumerable<IProductState> GetProductStates()
    {
      // todo: make a separate readonly repo for the query part of CQRS
      return _context.ProductStates
        .Include(s => s.ProductVersionStates)
        .Include(s => s.ProductIssueStates)
        .Include(s => s.ProductFeatureStates)
        .Include(i => i.ProductConfigOptionStates)
        .AsNoTracking().ToList();
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
      var state = _context.CompanyStates.Include(i => i.CompanyRoleStates)
        .Include(i => i.CompanyEnvironmentStates).ThenInclude(ti => ti.HardwareStates)
        .Include(ti => ti.CompanyEnvironmentStates).ThenInclude(ti => ti.DatabaseStates)
        .Include(ti => ti.CompanyEnvironmentStates).ThenInclude(ti => ti.AccountStates)
        .SingleOrDefault(s => s.Guid == guid);
      return state;
    }

    public IEnumerable<ICompanyState> GetCompanyStates()
    {
      return _context.CompanyStates.Include(i => i.CompanyRoleStates)
        .Include(i => i.CompanyEnvironmentStates).ThenInclude(ti => ti.HardwareStates)
        .Include(ti => ti.CompanyEnvironmentStates).ThenInclude(ti => ti.DatabaseStates)
        .Include(ti => ti.CompanyEnvironmentStates).ThenInclude(ti => ti.AccountStates)
        .AsNoTracking().OrderBy(o => o.Name).ToList();
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
      return states;
    }

    public IEnumerable<IEmploymentState> GetEmploymentsByContactGuid(Guid contactGuid)
    {
      var states = _context.EmploymentStates.AsNoTracking().Where(w => w.ContactGuid == contactGuid).ToList();
      return states;
    }

    public IEnumerable<IEmploymentState> GetEmploymentStates()
    {
      var states = _context.EmploymentStates.AsNoTracking().ToList();
      return states;
    }
    public IEnumerable<ICommandState> GetCommandStates()
    {
      // todo: consider which date I want to use. Ideally the created on reflects the time the user created the command correctly. Ideally ...
      var states = _context.CommandStates.AsNoTracking().OrderByDescending(o => o.ExecutedOn).ThenByDescending(o => o.CreatedOn).ToList();
      return states;
    }

    public IEnumerable<ICommandState> GetCommandStates(Guid guid)
    {
      // todo: consider which date I want to use. Ideally the created on reflects the time the user created the command correctly. Ideally ...
      var states = _context.CommandStates.Where(w => w.EntityGuid == guid).OrderByDescending(o => o.ExecutedOn).ThenByDescending(o => o.CreatedOn).AsNoTracking().ToList();
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
      var state = new ProductFeatureState { Guid = productFeatureGuid, ProductGuid = guid, Name = name };
      _context.ProductFeatureStates.Add(state);
      return state;
    }
    private ProductFeatureState GetProductFeatureState(Guid guid)
    {
      return _context.ProductFeatureStates.SingleOrDefault(s => s.Guid == guid);
    }

    public void DeleteProductFeatureState(Guid productGuid, Guid guid)
    {
      var state = GetProductFeatureState(guid);
      if (state != null)
      {
        _context.ProductFeatureStates.Remove(state);
      }
    }
    private ProductVersionState GetProductVersionState(Guid guid)
    {
      return _context.ProductVersionStates.SingleOrDefault(s => s.Guid == guid);
    }

    public void DeleteProductVersionState(Guid productGuid, Guid guid)
    {
      var state = GetProductVersionState(guid);
      if (state != null)
      {
        _context.ProductVersionStates.Remove(state);
      }
    }
    private ProductIssueState GetProductIssueState(Guid guid)
    {
      return _context.ProductIssueStates.SingleOrDefault(s => s.Guid == guid);
    }
    public IProductIssueState CreateProductIssueState(Guid productGuid, Guid guid, string name)
    {
      var state = new ProductIssueState { Guid = guid, ProductGuid = productGuid, Name = name };
      _context.ProductIssueStates.Add(state);
      return state;
    }

    // todo: this needs to delete from the selected feature state as well?
    public void DeleteProductIssueState(Guid productGuid, Guid guid)
    {
      var state = GetProductIssueState(guid);
      if (state != null)
      {
        _context.ProductIssueStates.Remove(state);
      }
    }
    public ICompanyEnvironmentHardwareState CreateCompanyEnvironmentHardwareState(Guid companyGuid, Guid environmentGUid, Guid guid, string name)
    {
      var newState = new CompanyEnvironmentHardwareState { CompanyGuid = companyGuid, EnvironmentGuid = environmentGUid, Guid = guid, Name = name };
      _context.CompanyEnvironmentHardwareStates.Add(newState);
      return newState;
    }

    public ICompanyEnvironmentHardwareState AddHardwareToEnvironmentState(ICompanyEnvironmentState companyEnvironmentState, Guid hardwareGuid, string hardwareName)
    {
      var state = GetHardwareForEnvironmentState(companyEnvironmentState, hardwareGuid);
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
      var newState = new DesignState { Guid = guid, Name = name };
      _context.DesignStates.Add(newState);
      return newState;
    }

    public IEpicElementState CreateEpicElementState(Guid designGuid, Guid guid, string name)
    {
      var newState = new EpicElementState { Guid = guid, DesignGuid = designGuid, Name = name };
      _context.EpicElementStates.Add(newState);
      return newState;
    }

    public IDesignState GetDesignState(Guid guid)
    {
      return _context.DesignStates
        .Include(i => i.EpicElementStates).ThenInclude(ti => ti.EntityElementStates).ThenInclude(ti => ti.PropertyElementStates)
        .Include(i => i.EpicElementStates).ThenInclude(ti => ti.EntityElementStates).ThenInclude(ti => ti.CommandElementStates)
        .SingleOrDefault(s => s.Guid == guid);
    }

    public IEnumerable<IDesignState> GetDesignStates()
    {
      return _context.DesignStates
        .Include(i => i.EpicElementStates).ThenInclude(ti => ti.EntityElementStates).ThenInclude(ti => ti.PropertyElementStates)
        .Include(i => i.EpicElementStates).ThenInclude(ti => ti.EntityElementStates).ThenInclude(ti => ti.CommandElementStates)
        .AsNoTracking().ToList();
    }
    // todo: does this need to enable cascaded delete to remove all children?
    public void DeleteDesignState(Guid guid)
    {
      var state = _context.DesignStates.SingleOrDefault(s => s.Guid == guid);
      if (state != null)
      {
        _context.DesignStates.Remove(state);
      }
    }

    public IEntityElementState CreateEntityElementState(Guid designGuid, Guid epicGuid, Guid entityGuid, string name, Guid? parentGuid)
    {
      var newState = new EntityElementState { Guid = entityGuid, EpicElementGuid = epicGuid, DesignGuid = designGuid, Name = name, ParentGuid = parentGuid };
      _context.EntityElementStates.Add(newState);
      return newState;
    }

    public IPropertyElementState CreatePropertyElementState(Guid designGuid, Guid epicGuid, Guid entityGuid, Guid guid, string name)
    {
      var newState = new PropertyElementState { Guid = guid, EntityElementGuid = entityGuid, EpicElementGuid = epicGuid, DesignGuid = designGuid, Name = name };
      _context.PropertyElementStates.Add(newState);
      return newState;
    }

    public ICommandElementState CreateCommandElementState(Guid designGuid, Guid epicGuid, Guid entityGuid, Guid guid, string name)
    {
      var newState = new CommandElementState { Guid = guid, EntityElementGuid = entityGuid, EpicElementGuid = epicGuid, DesignGuid = designGuid, Name = name };
      _context.CommandElementStates.Add(newState);
      return newState;
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

    public IProjectRoleAssignmentState CreateProjectRoleAssignmentState(Guid guid, Guid contactGuid, Guid projectGuid, Guid projectRoleGuid)
    {
      var state = new ProjectRoleAssignmentState { Guid = guid, ContactGuid = contactGuid, ProjectGuid = projectGuid, ProjectRoleGuid = projectRoleGuid };
      _context.ProjectRoleAssignmentStates.Add(state);
      return state;
    }

    public IProjectRoleAssignmentState GetProjectRoleAssignmentState(Guid guid)
    {
      return _context.ProjectRoleAssignmentStates.SingleOrDefault(s => s.Guid == guid);
    }

    public IEnumerable<IProjectRoleAssignmentState> GetProjectRoleAssignmentsByProjectRoleGuid(Guid projectRoleGuid)
    {
      return _context.ProjectRoleAssignmentStates.Where(s => s.ProjectRoleGuid == projectRoleGuid).AsNoTracking().ToList();
    }

    public IEnumerable<IProjectRoleAssignmentState> GetProjectRoleAssignmentsByContactGuid(Guid contactGuid)
    {
      return _context.ProjectRoleAssignmentStates.Where(s => s.ContactGuid == contactGuid).AsNoTracking().ToList();
    }

    public IEnumerable<IContactState> GetContactsByProjectRoleGuid(Guid projectRoleGuid)
    {
      var contactGuids = _context.ProjectRoleAssignmentStates.Where(s => s.ProjectRoleGuid == projectRoleGuid).AsNoTracking().Select(s => s.ContactGuid).ToList();
      return _context.ContactStates.Where(s => contactGuids.Contains(s.Guid)).AsNoTracking().ToList();
    }

    public void DeleteProjectRoleAssignmentState(Guid entityGuid)
    {
      var state = GetProjectRoleAssignmentState(entityGuid);
      if (state != null)
      {
        _context.ProjectRoleAssignmentStates.Remove((ProjectRoleAssignmentState)state);
      }
    }

    public IEnumerable<IProjectRoleAssignmentState> GetProjectRoleAssignmentStates()
    {
      return _context.ProjectRoleAssignmentStates.AsNoTracking().ToList();
    }

    public IEnumerable<IContactState> GetContactsByProjectGuid(Guid guid)
    {
      var contactGuids = _context.ProjectRoleAssignmentStates.Where(s => s.ProjectGuid == guid).AsNoTracking().Select(s => s.ContactGuid).ToList();
      return _context.ContactStates.Where(s => contactGuids.Contains(s.Guid)).AsNoTracking().ToList();
    }
    public ICompanyEnvironmentAccountState CreateCompanyEnvironmentAccountState(Guid companyGuid, Guid environmentGUid, Guid guid, string name)
    {
      var newState = new CompanyEnvironmentAccountState { CompanyGuid = companyGuid, EnvironmentGuid = environmentGUid, Guid = guid, Name = name };
      _context.CompanyEnvironmentAccountStates.Add(newState);
      return newState;
    }

    public ICompanyEnvironmentAccountState AddAccountToEnvironmentState(ICompanyEnvironmentState companyEnvironmentState, Guid accountGuid, string accountName)
    {
      var state = GetAccountForEnvironmentState(companyEnvironmentState, accountGuid);
      if (state == null)
      {
        state = CreateCompanyEnvironmentAccountState(companyEnvironmentState.CompanyGuid, companyEnvironmentState.Guid, accountGuid, accountName);
      }
      return state;
    }

    public void RemoveAccountFromEnvironmentState(ICompanyEnvironmentState companyEnvironmentState, Guid accountGuid)
    {
      var state = GetAccountForEnvironmentState(companyEnvironmentState, accountGuid);
      if (state != null)
      {
        companyEnvironmentState.AccountStates.Remove(state);
        _context.CompanyEnvironmentAccountStates.Remove((CompanyEnvironmentAccountState)state);
      }
    }

    public ICompanyEnvironmentAccountState GetAccountForEnvironmentState(ICompanyEnvironmentState state, Guid accountGuid)
    {
      return state.AccountStates?.SingleOrDefault(s => s.Guid == accountGuid);
    }
    public ICompanyEnvironmentDatabaseState CreateCompanyEnvironmentDatabaseState(Guid companyGuid, Guid environmentGUid, Guid guid, string name)
    {
      var newState = new CompanyEnvironmentDatabaseState { CompanyGuid = companyGuid, EnvironmentGuid = environmentGUid, Guid = guid, Name = name };
      _context.CompanyEnvironmentDatabaseStates.Add(newState);
      return newState;
    }

    public ICompanyEnvironmentDatabaseState AddDatabaseToEnvironmentState(ICompanyEnvironmentState companyEnvironmentState, Guid databaseGuid, string databaseName)
    {
      var state = GetDatabaseForEnvironmentState(companyEnvironmentState, databaseGuid);
      if (state == null)
      {
        state = CreateCompanyEnvironmentDatabaseState(companyEnvironmentState.CompanyGuid, companyEnvironmentState.Guid, databaseGuid, databaseName);
      }
      return state;
    }

    public void RemoveDatabaseFromEnvironmentState(ICompanyEnvironmentState companyEnvironmentState, Guid databaseGuid)
    {
      var state = GetDatabaseForEnvironmentState(companyEnvironmentState, databaseGuid);
      if (state != null)
      {
        companyEnvironmentState.DatabaseStates.Remove(state);
        _context.CompanyEnvironmentDatabaseStates.Remove((CompanyEnvironmentDatabaseState)state);
      }
    }

    public ICompanyEnvironmentDatabaseState GetDatabaseForEnvironmentState(ICompanyEnvironmentState state, Guid databaseGuid)
    {
      return state.DatabaseStates?.SingleOrDefault(s => s.Guid == databaseGuid);
    }

    public IEnumerable<ICommandState> GetUnprocessedCommandStates()
    {
      var states = _context.CommandStates.Where(w => w.ExecutedOn == null).OrderByDescending(o => o.ReceivedOn);
      return states;
    }

    public IProductConfigOptionState CreateProductConfigOptionState(IProductState productState, Guid? featureGuid, Guid? parentGuid, Guid guid, string name)
    {
      var newState = new ProductConfigOptionState { Guid = guid, Name = name, ProductGuid = productState.Guid };
      newState.ProductFeatureGuid = featureGuid;
      newState.ParentGuid = parentGuid;
      this._context.ProductConfigOptionStates.Add(newState);
      return newState;
    }

    public IProductConfigOptionState GetProductConfigOptionState(IProductState state, Guid guid)
    {
      return state.ProductConfigOptionStates.SingleOrDefault(s => s.Guid == guid);
    }

    public void DeleteProductConfigOptionState(IProductState state, Guid guid)
    {
      var stateToDelete = GetProductConfigOptionState(state, guid);
      state.ProductConfigOptionStates.Remove(stateToDelete);
      // todo: remove from product?
      _context.ProductConfigOptionStates.Remove((ProductConfigOptionState)stateToDelete);
    }

    // todo: is this enough? May cause problems when validating before object is refreshed,
    // should we expose the Parent state object here? (we don't currently so no problem yet)
    public void MoveProductConfigOption(IProductConfigOptionState state, Guid parentGuid)
    {
      state.ParentGuid = parentGuid;
    }

    public void MakeDefaultConfigOptionState(IProductConfigOptionState state)
    {
      if (state.ParentGuid != null)
      {
        var options = _context.ProductConfigOptionStates.Where(s => s.ParentGuid == state.ParentGuid);
        foreach (var option in options)
        {
          option.IsDefaultOption = (state.Guid == option.Guid);
        }
      }
      else
      {
        // not possible to make default option. Exception?
      }
    }

    public IProductInstallationState CreateProductInstallationState(Guid guid, Guid companyGuid, Guid productGuid)
    {
      var state = new ProductInstallationState { Guid = guid, CompanyGuid = companyGuid, ProductGuid = productGuid };
      _context.ProductInstallationStates.Add(state);
      return state;
    }

    public IProductInstallationState GetProductInstallationState(Guid guid)
    {
      return _context.ProductInstallationStates.SingleOrDefault(s => s.Guid == guid);
    }

    public IEnumerable<IProductInstallationState> GetProductInstallationsByCompanyGuid(Guid companyGuid)
    {
      return _context.ProductInstallationStates.Where(s => s.CompanyGuid == companyGuid).AsNoTracking().ToList();
    }

    public IEnumerable<IProductInstallationState> GetProductInstallationsByProductGuid(Guid productGuid)
    {
      return _context.ProductInstallationStates.Where(s => s.ProductGuid == productGuid).AsNoTracking().ToList();
    }

    public IEnumerable<IProductInstallationState> GetProductInstallationStates()
    {
      return _context.ProductInstallationStates.AsNoTracking().ToList();
    }

    public void DeleteProductInstallationState(Guid entityGuid)
    {
      var state = GetProductInstallationState(entityGuid);
      if (state != null)
      {
        _context.ProductInstallationStates.Remove((ProductInstallationState)state);
      }
    }
  }
}
