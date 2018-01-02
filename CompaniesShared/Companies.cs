using CommandsShared;
using DateTimeShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompaniesShared
{
  public interface ICompanyService : ICommandProcessor
  {
    ICompany CreateCompany(Guid guid, string name);
    ICompany GetCompany(Guid guid);
    void DeleteCompany(Guid guid);
    void PersistChanges();
  }

  public interface ICompanyState : INamedEntityState
  {
    ICollection<ICompanyRoleState> CompanyRoleStates { get; set; }
    ICollection<ICompanyEnvironmentState> CompanyEnvironmentStates { get; set; }
  }
  public interface ICompanyRoleState : INamedEntityState
  {
  }
  public interface ICompanyEnvironmentState : INamedEntityState
  {
    Guid CompanyGuid { get; set; }
    string Url { get; set; }
    ICollection<ICompanyEnvironmentHardwareState> HardwareStates { get; set; }
  }
  public interface ICompanyEnvironmentHardwareState : INamedEntityState
  {
    Guid EnvironmentGuid { get; set; }
    Guid CompanyGuid { get; set; }
    string IpAddress { get; set; }
  }
  public interface ICompanyStateRepository : IEntityRepository
  {
    ICompanyState CreateCompanyState(Guid guid, string name);
    ICompanyState GetCompanyState(Guid guid);
    IEnumerable<ICompanyState> GetCompanyStates();
    void DeleteCompanyState(Guid guid);
    ICompanyRoleState AddRoleToCompanyState(Guid projectGuid, Guid roleGuid, string name);
    void RemoveRoleFromCompanyState(Guid guid, Guid roleGuid);
    ICompanyEnvironmentState AddEnvironmentToCompanyState(Guid guid, Guid environmentGuid, string environmentName);
    void RemoveEnvironmentFromCompanyState(Guid guid, Guid environmentGuid);
    ICompanyEnvironmentState GetEnvironmentState(Guid companyGuid, Guid environmentGuid);
    ICompanyEnvironmentHardwareState AddHardwareToEnvironmentState(ICompanyEnvironmentState state, Guid hardwareGuid, string hardwareName);
    void RemoveHardwareFromEnvironmentState(ICompanyEnvironmentState state, Guid hardwareGuid);
    ICompanyEnvironmentHardwareState GetHardwareForEnvironmentState(ICompanyEnvironmentState state, Guid hardwareGuid);
  }
  public interface IEntity
  {
    Guid Guid { get; }
    string Name { get; }
    DateTime CreatedOn { get; }
    void Rename(string name, string originalName);
  }
  public interface ICompany : IEntity
  {
    void AddRoleToCompany(Guid roleGuid, string roleName);
    void RemoveRoleFromCompany(Guid roleGuid);
    void AddEnvironmentToCompany(Guid environmentGuid, string environmentName);
    void RemoveEnvironmentFromCompany(Guid environmentGuid);
    ICompanyEnvironment GetEnvironment(Guid guid);
  }
  public interface ICompanyEnvironment : IEntity
  {
    void AddHardware(Guid hardwareGuid, string hardwareName);
    void ChangeUrl(string url, string originalUrl);
    ICompanyEnvironmentHardware GetHardware(Guid hardwareGuid);
    void RemoveHardware(Guid hardwareGuid);
  }
  public interface ICompanyEnvironmentHardware : IEntity
  {
    Guid EnvironmentGuid { get; }
    Guid CompanyGuid { get; }

    void ChangeIpAddress(string ipAddress, string originalIpAddress);
  }

  public class Company : ICompany
  {
    private ICompanyState _state;
    private ICompanyStateRepository _repo;

    public Company(ICompanyState state)
    {
      _state = state;
    }
    public Company(ICompanyState state, ICompanyStateRepository repo) : this(state)
    {
      _repo = repo;
    }

    public Guid Guid { get { return _state.Guid; } }
    public string Name { get { return _state.Name; } }
    public DateTime CreatedOn { get { return _state.CreatedOn; } }

    public void Rename(string name, string originalName)
    {
      if (_state.Name == originalName)
      {
        _state.Name = name;
      }
      else
      {
        // todo: implement concurrency policy
      }
    }
    public void AddRoleToCompany(Guid roleGuid, string name)
    {
      _repo.AddRoleToCompanyState(this.Guid, roleGuid, name);
    }

    public void RemoveRoleFromCompany(Guid roleGuid)
    {
      _repo.RemoveRoleFromCompanyState(this.Guid, roleGuid);
    }

    public void AddEnvironmentToCompany(Guid environmentGuid, string environmentName)
    {
      _repo.AddEnvironmentToCompanyState(this.Guid, environmentGuid, environmentName);
    }

    public void RemoveEnvironmentFromCompany(Guid environmentGuid)
    {
      _repo.RemoveEnvironmentFromCompanyState(this.Guid, environmentGuid);
    }

    public ICompanyEnvironment GetEnvironment(Guid guid)
    {
      var state = _repo.GetEnvironmentState(this.Guid, guid);
      return new CompanyEnvironment(state, _repo);
    }
  }
  public class CompanyEnvironment : ICompanyEnvironment
  {
    private ICompanyEnvironmentState _state;
    private ICompanyStateRepository _repo;

    public CompanyEnvironment(ICompanyEnvironmentState state)
    {
      _state = state;
    }
    public CompanyEnvironment(ICompanyEnvironmentState state, ICompanyStateRepository repo) : this(state)
    {
      _repo = repo;
    }

    public Guid Guid { get { return _state.Guid; } }
    public string Name { get { return _state.Name; } }
    public DateTime CreatedOn { get { return _state.CreatedOn; } }


    public void AddHardware(Guid hardwareGuid, string hardwareName)
    {
      _repo.AddHardwareToEnvironmentState(_state, hardwareGuid, hardwareName);
    }

    public void ChangeUrl(string url, string originalUrl)
    {
      if (_state.Url == originalUrl)
      {
        _state.Url = url;
      }
      else
      {
        // todo: implement concurrency policy
      }
    }

    public ICompanyEnvironmentHardware GetHardware(Guid hardwareGuid)
    {
      var state = _repo.GetHardwareForEnvironmentState(_state, hardwareGuid);
      return new CompanyEnvironmentHardware(state);
    }

    public void RemoveHardware(Guid hardwareGuid)
    {
      _repo.RemoveHardwareFromEnvironmentState(_state, hardwareGuid);
    }

    public void Rename(string name, string originalName)
    {
      if (_state.Name == originalName)
      {
        _state.Name = name;
      }
      else
      {
        // todo: implement concurrency policy
      }
    }


    public void Url(string url, string originalUrl)
    {
      if (_state.Url == originalUrl)
      {
        _state.Url = url;
      }
      else
      {
        // todo: implement concurrency policy
      }
    }
  }
  public class CompanyEnvironmentHardware : ICompanyEnvironmentHardware
  {
    private ICompanyEnvironmentHardwareState _state;
    public CompanyEnvironmentHardware(ICompanyEnvironmentHardwareState state)
    {
      _state = state;
    }
    public Guid EnvironmentGuid { get { return _state.EnvironmentGuid; } }

    public Guid CompanyGuid { get { return _state.CompanyGuid; } }

    public Guid Guid { get { return _state.Guid; } }

    public string Name { get { return _state.Name; } }

    public DateTime CreatedOn { get { return _state.CreatedOn; } }

    public void Rename(string name, string originalName)
    {
      if (_state.Name == originalName)
      {
        _state.Name = name;
      }
      else
      {
        // todo: implement concurrency policy
      }
    }
    public void ChangeIpAddress(string ipAddress, string originalIpAddress)
    {
      if (_state.IpAddress == originalIpAddress)
      {
        _state.IpAddress = ipAddress;
      }
      else
      {
        // todo: implement concurrency policy
      }
    }
  }
  public class CompanyService : ICompanyService
  {
    private IDateTimeProvider _dateTimeProvider;
    private ICompanyStateRepository _repo;
    public CompanyService(ICompanyStateRepository repo, IDateTimeProvider dateTimeProvider)
    {
      _repo = repo;
      _dateTimeProvider = dateTimeProvider;
    }
    public ICompany CreateCompany(Guid guid, string name)
    {
      var state = _repo.CreateCompanyState(guid, name);
      state.CreatedOn = _dateTimeProvider.GetUtcDateTime();
      state.UpdatedOn = _dateTimeProvider.GetUtcDateTime();
      state.Name = name;
      return new Company(state);
    }
    public ICompany GetCompany(Guid guid)
    {
      var state = _repo.GetCompanyState(guid);
      return new Company(state, _repo);
    }
    public void DeleteCompany(Guid guid)
    {
      _repo.DeleteCompanyState(guid);
    }

    public void PersistChanges()
    {
      _repo.PersistChanges();
    }
  }
  public class CompanyBuilder
  {
    private CompanyService _companies;
    private Guid _guid;
    private string _name;

    public CompanyBuilder(CompanyService companies)
    {
      _companies = companies;
    }

    public ICompany Build(string name)
    {
      EnsureGuid();
      var company = _companies.CreateCompany(_guid, name);
      return company;
    }

    private void EnsureGuid()
    {
      if (_guid == null || _guid == Guid.Empty)
      {
        _guid = Guid.NewGuid();
      }
    }

    public CompanyBuilder WithGuid(Guid guid)
    {
      _guid = guid;
      return this;
    }

    public CompanyBuilder WithName(string name)
    {
      _name = name;
      return this;
    }
  }
}
