using CompaniesShared;
using ContactsShared;
using ProductsShared;
using ProjectsShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoftwareManagementEventSourceRepository
{
  public class ContactState : IContactState
  {
    public DateTime? BirthDate { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public Guid Guid { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public Guid? AvatarFileGuid { get; set; }
    public string AvatarUrl { get; set; }
  }
  public class CompanyState : ICompanyState
  {
    public ICollection<ICompanyRoleState> CompanyRoleStates { get; set; }
    public string Name { get; set; }
    public Guid Guid { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public ICollection<ICompanyEnvironmentState> CompanyEnvironmentStates { get; set; }
    public string ExternalId { get; set; }
    public string Code { get; set; }
  }
  public class CompanyRoleState : ICompanyRoleState
  {
    public string Name { get; set; }
    public Guid Guid { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
  }
  public class CompanyEnvironmentState : ICompanyEnvironmentState
  {
    public string Name { get; set; }
    public Guid Guid { get; set; }
    public Guid CompanyGuid { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public string Url { get; set; }
    public ICollection<ICompanyEnvironmentHardwareState> HardwareStates { get; set; }
    public ICollection<ICompanyEnvironmentAccountState> AccountStates { get; set; }
    public ICollection<ICompanyEnvironmentDatabaseState> DatabaseStates { get; set; }
  }
  public class ProjectState : IProjectState
  {
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ICollection<IProjectRoleState> ProjectRoleStates { get; set; }
    public string Name { get; set; }
    public Guid Guid { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
  }
  public class ProjectRoleState : IProjectRoleState
  {
    public string Name { get; set; }
    public Guid Guid { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
  }
  public class ProductState : IProductState
  {
    public string Description { get; set; }
    public string BusinessCase { get; set; }
    public string Name { get; set; }
    public Guid Guid { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public ICollection<IProductVersionState> ProductVersionStates { get; set; }
    public ICollection<IProductFeatureState> ProductFeatureStates { get; set; }
    public ICollection<IProductIssueState> ProductIssueStates { get; set; }
  }
  public class EventSourcedMainRepository : IContactStateRepository, ICompanyStateRepository, IProjectStateRepository, IProductStateRepository
  {
    private Dictionary<Guid, IContactState> _contactDictionary;
    private Dictionary<Guid, IProjectState> _projectDictionary;
    private Dictionary<Guid, ICompanyState> _companyDictionary;
    private Dictionary<Guid, IProductState> _productDictionary;
    public EventSourcedMainRepository()
    {
      // works as a nice cache too
      _contactDictionary = new Dictionary<Guid, IContactState>();
      _projectDictionary = new Dictionary<Guid, IProjectState>();
      _companyDictionary = new Dictionary<Guid, ICompanyState>();
      _productDictionary = new Dictionary<Guid, IProductState>();
    }

    public ICompanyEnvironmentState AddEnvironmentToCompanyState(Guid guid, Guid environmentGuid, string name)
    {
      var state = GetCompanyState(guid);
      var companyEnvironmentState = new CompanyEnvironmentState { Guid = environmentGuid, Name = name };
      state.CompanyEnvironmentStates.Add(companyEnvironmentState);
      return companyEnvironmentState;
    }

    public ICompanyRoleState AddRoleToCompanyState(Guid guid, Guid roleGuid, string name)
    {
      var state = GetCompanyState(guid);
      var companyRoleState = new CompanyRoleState { Guid = roleGuid, Name = name };
      state.CompanyRoleStates.Add(companyRoleState);
      return companyRoleState;
    }

    public void AddRoleToProjectState(Guid guid, Guid roleGuid, string name)
    {
      var state = GetProjectState(guid);
      state.ProjectRoleStates.Add(new ProjectRoleState { Guid = roleGuid, Name = name });
    }

    public ICompanyState CreateCompanyState(Guid guid, string name)
    {
      if (_companyDictionary.TryGetValue(guid, out ICompanyState state))
      {
        state.Name = name; // todo: do we need concurrency check?
      }
      else
      {
        state = new CompanyState { Guid = guid, Name = name };
        _companyDictionary.Add(guid, state);
      }
      return state;
    }

    public IContactState CreateContactState(Guid guid, string name)
    {
      // in eventsourcing, state may already exist
      if (_contactDictionary.TryGetValue(guid, out IContactState state))
      {
        state.Name = name; // todo: do we need concurrency check?
      }
      else
      {
        state = new ContactState { Guid = guid, Name = name };
        _contactDictionary.Add(guid, state);
      }
      return state;
    }

    public IProductFeatureState CreateProductFeatureState(Guid guid, Guid productFeatureGuid, string name)
    {
      throw new NotImplementedException();
    }

    public IProductState CreateProductState(Guid guid, string name)
    {
      if (_productDictionary.TryGetValue(guid, out IProductState state))
      {
        state.Name = name; // todo: do we need concurrency check?
      }
      else
      {
        state = new ProductState { Guid = guid, Name = name };
        _productDictionary.Add(guid, state);
      }
      return state;
    }

    public IProductVersionState CreateProductVersionState(Guid guid, string name)
    {
      throw new NotImplementedException();
    }

    public IProductVersionState CreateProductVersionState(Guid guid, Guid productVersionGuid, string name)
    {
      throw new NotImplementedException();
    }

    public IProjectState CreateProjectState(Guid guid, string name)
    {
      if (_projectDictionary.TryGetValue(guid, out IProjectState state))
      {
        state.Name = name; // todo: do we need concurrency check?
      }
      else
      {
        state = new ProjectState { Guid = guid, Name = name };
        _projectDictionary.Add(guid, state);
      }
      return state;
    }

    public void DeleteCompanyState(Guid guid)
    {
      //            throw new NotImplementedException();
    }

    public void DeleteContactState(Guid guid)
    {
      // ignore in eventsourcing
    }

    public void DeleteProductFeatureState(Guid productGuid, Guid guid)
    {
      // ignore in eventsourcing
    }
    public void DeleteProductVersionState(Guid productGuid, Guid guid)
    {
      // ignore in eventsourcing
    }

    public void DeleteProductState(Guid guid)
    {
      // ignore in eventsourcing
    }

    public void DeleteProjectState(Guid guid)
    {
      //throw new NotImplementedException();
    }

    public ICompanyState GetCompanyState(Guid guid)
    {
      if (_companyDictionary.TryGetValue(guid, out ICompanyState state))
      {
        return state;
      }
      // in eventsourcing, we'll just create a nameless state
      state = CreateCompanyState(guid, "");
      return state;
    }

    public IEnumerable<ICompanyState> GetCompanyStates()
    {
      throw new NotImplementedException();
    }

    public IContactState GetContactState(Guid guid)
    {
      if (_contactDictionary.TryGetValue(guid, out IContactState state))
      {
        return state;
      }
      // in eventsourcing, we'll just create a nameless state
      state = CreateContactState(guid, "");
      return state;
    }

    public IEnumerable<IContactState> GetContactStates()
    {
      // we may need this at some point, but probably not
      throw new NotImplementedException();
    }

    public ICompanyEnvironmentState GetEnvironmentState(Guid companyGuid, Guid environmentGuid)
    {
      throw new NotImplementedException();
    }

    public IProductState GetProductState(Guid guid)
    {
      if (_productDictionary.TryGetValue(guid, out IProductState state))
      {
        return state;
      }
      // in eventsourcing, we'll just create a nameless state
      state = CreateProductState(guid, "");
      return state;
    }

    public IEnumerable<IProductState> GetProductStates()
    {
      throw new NotImplementedException();
    }

    public IProjectState GetProjectState(Guid guid)
    {
      if (_projectDictionary.TryGetValue(guid, out IProjectState state))
      {
        return state;
      }
      // in eventsourcing, we'll just create a nameless state
      state = CreateProjectState(guid, "");
      return state;
    }

    public IEnumerable<IProjectState> GetProjectStates()
    {
      throw new NotImplementedException();
    }

    public void PersistChanges()
    {
      // we don't persist state - we're always rebuilding from persisted events instead
    }

    public Task PersistChangesAsync()
    {
      // we don't persist state - we're always rebuilding from persisted events instead
      return Task.CompletedTask;
    }

    public void RemoveEnvironmentFromCompanyState(Guid guid, Guid environmentGuid)
    {
      throw new NotImplementedException();
    }

    public void RemoveRoleFromCompanyState(Guid guid, Guid roleGuid)
    {
      var state = GetCompanyState(guid);
      var roleState = state.CompanyRoleStates.Single(s => s.Guid == guid);
      state.CompanyRoleStates.Remove(roleState);
    }

    public void RemoveRoleFromProjectState(Guid guid, Guid roleGuid)
    {
      var state = GetProjectState(guid);
      var roleState = state.ProjectRoleStates.Single(s => s.Guid == guid);
      state.ProjectRoleStates.Remove(roleState);
    }

    public IProductIssueState CreateProductIssueState(Guid productGuid, Guid guid, string name)
    {
      throw new NotImplementedException();
    }

    public void DeleteProductIssueState(Guid productGuid, Guid guid)
    {
      // not in eventsourcing
    }

    public ICompanyEnvironmentHardwareState AddHardwareToEnvironmentState(ICompanyEnvironmentState state, Guid hardwareGuid, string hardwareName)
    {
      throw new NotImplementedException();
    }

    public void RemoveHardwareFromEnvironmentState(ICompanyEnvironmentState state, Guid hardwareGuid)
    {
      throw new NotImplementedException();
    }

    public ICompanyEnvironmentHardwareState GetHardwareForEnvironmentState(ICompanyEnvironmentState state, Guid hardwareGuid)
    {
      throw new NotImplementedException();
    }

    public ICompanyEnvironmentAccountState AddAccountToEnvironmentState(ICompanyEnvironmentState state, Guid accountGuid, string accountName)
    {
      throw new NotImplementedException();
    }

    public void RemoveAccountFromEnvironmentState(ICompanyEnvironmentState state, Guid accountGuid)
    {
      throw new NotImplementedException();
    }

    public ICompanyEnvironmentAccountState GetAccountForEnvironmentState(ICompanyEnvironmentState state, Guid accountGuid)
    {
      throw new NotImplementedException();
    }

    public ICompanyEnvironmentDatabaseState AddDatabaseToEnvironmentState(ICompanyEnvironmentState state, Guid databaseGuid, string databaseName)
    {
      throw new NotImplementedException();
    }

    public void RemoveDatabaseFromEnvironmentState(ICompanyEnvironmentState state, Guid databaseGuid)
    {
      throw new NotImplementedException();
    }

    public ICompanyEnvironmentDatabaseState GetDatabaseForEnvironmentState(ICompanyEnvironmentState state, Guid databaseGuid)
    {
      throw new NotImplementedException();
    }
  }
}
