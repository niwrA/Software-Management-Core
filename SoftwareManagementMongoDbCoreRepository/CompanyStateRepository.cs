using CommandsShared;
using CompaniesShared;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareManagementMongoDbCoreRepository
{
  [BsonIgnoreExtraElements]
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
  [BsonIgnoreExtraElements]
  public class CompanyRoleState : NamedEntityState, ICompanyRoleState
  {
  }

  [BsonIgnoreExtraElements]
  public class CompanyEnvironmentState : NamedEntityState, ICompanyEnvironmentState
  {
    public Guid CompanyGuid { get; set; }
    public string Url { get; set; }
    public ICollection<ICompanyEnvironmentHardwareState> HardwareStates { get; set; } = new List<ICompanyEnvironmentHardwareState>() as ICollection<ICompanyEnvironmentHardwareState>;
  }

  [BsonIgnoreExtraElements]
  public class CompanyEnvironmentHardwareState : NamedEntityState, ICompanyEnvironmentHardwareState
  {
    public Guid CompanyGuid { get; set; }
    public Guid EnvironmentGuid { get; set; }
    public string IpAddress{ get; set; }
  }

  public class CompanyStateRepository : ICompanyStateRepository
  {
    private const string CompanyStatesCollection = "CompanyStates";

    private IMongoClient _client;
    private IMongoDatabase _database;

    private Dictionary<Guid, ICompanyState> _companyStates;
    private List<Guid> _deletedCompanyStates;
    private Dictionary<Guid, ICompanyState> _updatedCompanyStates;

    private Dictionary<Guid, ICommandState> _commandStates { get; set; }

    public CompanyStateRepository(IMongoClient client)
    {
      _client = client;
      _database = _client.GetDatabase("SoftwareManagement");

      _companyStates = new Dictionary<Guid, ICompanyState>();
      _deletedCompanyStates = new List<Guid>();
      _updatedCompanyStates = new Dictionary<Guid, ICompanyState>();

    }
    public ICompanyState CreateCompanyState(Guid guid, string name)
    {
      var state = new CompanyState()
      {
        Guid = guid
      };
      _companyStates.Add(state.Guid, state);
      return state;
    }


    public void DeleteCompanyState(Guid guid)
    {
      _deletedCompanyStates.Add(guid);
    }

    public ICompanyRoleState AddRoleToCompanyState(Guid guid, Guid roleGuid, string name)
    {
      var state = GetCompanyState(guid);
      var roleState = state.CompanyRoleStates.FirstOrDefault(s => s.Guid == roleGuid); // todo: work with Single and catch errors?
      if (roleState == null)
      {
        roleState = new CompanyRoleState { Guid = roleGuid, Name = name };
        state.CompanyRoleStates.Add(roleState);
      }
      return roleState;
    }
    // todo: repository tests
    public void RemoveRoleFromCompanyState(Guid guid, Guid roleGuid)
    {
      var state = GetCompanyState(guid);
      var roleState = state.CompanyRoleStates.FirstOrDefault(s => s.Guid == roleGuid); // todo: work with Single and catch errors?
      if (roleState != null)
      {
        state.CompanyRoleStates.Remove(roleState);
      }
    }
    public ICompanyEnvironmentState AddEnvironmentToCompanyState(Guid guid, Guid environmentGuid, string name)
    {
      var state = GetCompanyState(guid);
      var environmentState = state.CompanyEnvironmentStates.FirstOrDefault(s => s.Guid == environmentGuid); // todo: work with Single and catch errors?
      if (environmentState == null)
      {
        environmentState = new CompanyEnvironmentState { Guid = environmentGuid, Name = name };
        state.CompanyEnvironmentStates.Add(environmentState);
      } // todo: else throw error? replace?
      return environmentState;
    }
    // todo: add test
    public void RemoveEnvironmentFromCompanyState(Guid guid, Guid environmentGuid)
    {
      var state = GetCompanyState(guid);
      var environmentState = state.CompanyEnvironmentStates.FirstOrDefault(s => s.Guid == environmentGuid); // todo: work with Single and catch errors?
      if (environmentState != null)
      {
        state.CompanyEnvironmentStates.Remove(environmentState);
      }
    }

    public ICompanyEnvironmentState GetEnvironmentState(Guid companyGuid, Guid environmentGuid)
    {
      var companyState = GetCompanyState(companyGuid);
      var state = companyState.CompanyEnvironmentStates.SingleOrDefault(s => s.Guid == environmentGuid);
      return state;
    }

    public ICompanyState GetCompanyState(Guid guid)
    {
      if (!_companyStates.TryGetValue(guid, out ICompanyState state))
      {
        if (!_updatedCompanyStates.TryGetValue(guid, out state))
        {
          var collection = _database.GetCollection<CompanyState>(CompanyStatesCollection);
          var filter = Builders<CompanyState>.Filter.Eq("Guid", guid);
          state = collection.Find(filter).FirstOrDefault();

          TrackCompanyState(state);

        }
      }
      return state;
    }

    public IEnumerable<ICompanyState> GetCompanyStates()
    {
      var collection = _database.GetCollection<CompanyState>(CompanyStatesCollection);
      var filter = new BsonDocument();
      var states = collection.Find(filter);

      return states?.ToList();
    }

    private void TrackCompanyState(ICompanyState state)
    {
      if (state != null && !_updatedCompanyStates.ContainsKey(state.Guid))
      {
        _updatedCompanyStates.Add(state.Guid, state);
      }
    }

    private void PersistCompanies()
    {
      var companyCollection = _database.GetCollection<CompanyState>(CompanyStatesCollection);
      // inserts
      if (_companyStates.Values.Any())
      {
        var companies = _companyStates.Values.Select(s => s as CompanyState).ToList();
        companyCollection.InsertMany(companies);
        _companyStates.Clear();
      }

      // todo: can these be batched?
      // updates
      if (_updatedCompanyStates.Values.Any())
      {
        var companies = _updatedCompanyStates.Values.Select(s => s as CompanyState).ToList();
        foreach (var state in companies)
        {
          var filter = Builders<CompanyState>.Filter.Eq("Guid", state.Guid);
          companyCollection.ReplaceOne(filter, state);
        }
        _updatedCompanyStates.Clear();
      }

      // deletes
      if (_deletedCompanyStates.Any())
      {
        var collection = _database.GetCollection<CompanyState>(CompanyStatesCollection);
        foreach (var guid in _deletedCompanyStates)
        {
          var filter = Builders<CompanyState>.Filter.Eq("Guid", guid);
          collection.DeleteOne(filter);
        }
        _deletedCompanyStates.Clear();
      }
    }

    public void PersistChanges()
    {
      PersistCompanies();
    }

    public Task PersistChangesAsync()
    {
      throw new NotImplementedException();
    }

    public ICompanyEnvironmentHardwareState AddHardwareToEnvironmentState(ICompanyEnvironmentState state, Guid hardwareGuid, string hardwareName)
    {
      var hardwareState = state.HardwareStates?.FirstOrDefault(s => s.Guid == hardwareGuid); // todo: work with Single and catch errors?
      if (hardwareState == null)
      {
        hardwareState = new CompanyEnvironmentHardwareState { Guid = hardwareGuid, EnvironmentGuid = state.Guid, Name = hardwareName };
        state.HardwareStates.Add(hardwareState);
      } // todo: else throw error? replace?
      return hardwareState;
    }

    public void RemoveHardwareFromEnvironmentState(ICompanyEnvironmentState state, Guid hardwareGuid)
    {
      var hardwareState = state.HardwareStates.FirstOrDefault(s => s.Guid == hardwareGuid); // todo: work with Single and catch errors?
      if (hardwareState != null)
      {
        state.HardwareStates.Remove(hardwareState);
      }
    }

    public ICompanyEnvironmentHardwareState GetHardwareForEnvironmentState(ICompanyEnvironmentState state, Guid hardwareGuid)
    {
      return state.HardwareStates.FirstOrDefault(s => s.Guid == hardwareGuid); // todo: work with Single and catch errors?
    }
  }
  // todo: not yet used, so use or remove
  public class CompanyStateBuilder
  {
    private List<CompanyEnvironmentState> _environments = new List<CompanyEnvironmentState>();
    public CompanyState Build()
    {
      var state = new CompanyState();
      foreach (var environmentState in _environments)
      {
        state.CompanyEnvironmentStates.Add(environmentState);
      }
      return state;
    }

    public CompanyStateBuilder WithEnvironment(string name)
    {
      var state = new CompanyEnvironmentState { Name = name };
      _environments.Add(state);
      return this;
    }
  }

}
