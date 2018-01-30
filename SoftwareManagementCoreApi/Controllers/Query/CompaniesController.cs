using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CompaniesShared;

// For more information on enabling Web API for empty products, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SoftwareManagementCoreApi.Controllers
{
  public class CompanyRoleDto
  {
    private ICompanyRoleState _state;
    public CompanyRoleDto(ICompanyRoleState state)
    {
      _state = state;
    }
    public Guid Guid { get { return _state.Guid; } }
    public string Name { get { return _state.Name; } }
  }
  public class CompanyEnvironmentDto
  {
    private ICompanyEnvironmentState _state;
    public CompanyEnvironmentDto(ICompanyEnvironmentState state)
    {
      _state = state;
    }
    public Guid Guid { get { return _state.Guid; } }
    public string Name { get { return _state.Name; } }
    public string Url { get { return _state.Url; } }
    public Guid CompanyGuid { get { return _state.CompanyGuid; } }
    public List<CompanyEnvironmentHardwareDto> Hardware { get { return _state.HardwareStates?.Select(s => new CompanyEnvironmentHardwareDto(s)).ToList(); } }
    public List<CompanyEnvironmentAccountDto> Accounts { get { return _state.AccountStates?.Select(s => new CompanyEnvironmentAccountDto(s)).ToList(); } }
    public List<CompanyEnvironmentDatabaseDto> Databases { get { return _state.DatabaseStates?.Select(s => new CompanyEnvironmentDatabaseDto(s)).ToList(); } }
  }
  public class CompanyEnvironmentHardwareDto
  {
    private ICompanyEnvironmentHardwareState _state;
    public CompanyEnvironmentHardwareDto(ICompanyEnvironmentHardwareState state)
    {
      _state = state;
    }
    public Guid Guid { get { return _state.Guid; } }
    public string Name { get { return _state.Name; } }
    public string IpAddress { get { return _state.IpAddress; } }
    public Guid CompanyGuid { get { return _state.CompanyGuid; } }
    public Guid EnvironmentGuid { get { return _state.EnvironmentGuid; } }
  }
  public class CompanyEnvironmentAccountDto
  {
    private ICompanyEnvironmentAccountState _state;
    public CompanyEnvironmentAccountDto(ICompanyEnvironmentAccountState state)
    {
      _state = state;
    }
    public Guid Guid { get { return _state.Guid; } }
    public string Name { get { return _state.Name; } }
    public Guid CompanyGuid { get { return _state.CompanyGuid; } }
    public Guid EnvironmentGuid { get { return _state.EnvironmentGuid; } }
  }
  public class CompanyEnvironmentDatabaseDto
  {
    private ICompanyEnvironmentDatabaseState _state;
    public CompanyEnvironmentDatabaseDto(ICompanyEnvironmentDatabaseState state)
    {
      _state = state;
    }
    public Guid Guid { get { return _state.Guid; } }
    public string Name { get { return _state.Name; } }
    public Guid CompanyGuid { get { return _state.CompanyGuid; } }
    public Guid EnvironmentGuid { get { return _state.EnvironmentGuid; } }
  }

  public class CompanyDto
  {
    private ICompanyState _state;
    private List<CompanyRoleDto> _companyRoles;
    private List<CompanyEnvironmentDto> _companyEnvironments;

    public CompanyDto(ICompanyState state)
    {
      _state = state;
      _companyRoles = _state.CompanyRoleStates.Select(s => new CompanyRoleDto(s)).ToList();
      _companyEnvironments = _state.CompanyEnvironmentStates.Select(s => new CompanyEnvironmentDto(s)).ToList();
    }
    public Guid Guid { get { return _state.Guid; } }
    public string Name { get { return _state.Name; } }
    public List<CompanyRoleDto> Roles { get { return _companyRoles; } }
    public List<CompanyEnvironmentDto> Environments { get { return _companyEnvironments; } }
    public string ExternalId { get { return _state.ExternalId; } }
    public string Code { get { return _state.Code; } }
  }

    [Route("api/[controller]")]
    public class CompaniesController : Controller
    {
      private ICompanyStateReadOnlyRepository _companyStateRepository;

      public CompaniesController(ICompanyStateReadOnlyRepository companyStateRepository)
      {
        _companyStateRepository = companyStateRepository;
      }
      // GET: api/products
      [HttpGet]
      public IEnumerable<CompanyDto> Get()
      {
        var states = _companyStateRepository.GetCompanyStates();
        var dtos = states.Select(s => new CompanyDto(s)).ToList();
        return dtos;
      }

      // GET api/products/5
      [HttpGet("{guid}")]
      public CompanyDto Get(Guid guid)
      {
        var state = _companyStateRepository.GetCompanyState(guid);
        return new CompanyDto(state);
      }
    }
  }
