using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductInstallationsShared;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty ProductInstallations, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SoftwareManagementCoreApi.Controllers
{
  public class ProductInstallationDto
  {
    private IProductInstallationState _state;
    public ProductInstallationDto(IProductInstallationState state)
    {
      _state = state;
    }
    public Guid Guid { get { return _state.Guid; } }
    public Guid CompanyGuid { get { return _state.CompanyGuid; } }
    public Guid ProductGuid { get { return _state.ProductGuid; } }
    public Guid? CompanyEnvironmentGuid { get { return _state.CompanyEnvironmentGuid; } }
    public Guid? ProductVersionGuid { get { return _state.CompanyEnvironmentGuid; } }
    public string StartDate { get { return _state.StartDate.HasValue ? _state.StartDate.Value.ToString("yyyy-MM-dd") : ""; } }
    public string EndDate { get { return _state.EndDate.HasValue ? _state.EndDate.Value.ToString("yyyy-MM-dd") : ""; } }
    public string ExternalId { get { return _state.ExternalId; } }
  }

  [Route("api/[controller]")]
  public class ProductInstallationsController : Controller
  {
    private IProductInstallationStateRepository _ProductInstallationStateRepository;

    public ProductInstallationsController(IProductInstallationStateRepository ProductInstallationStateRepository)
    {
      _ProductInstallationStateRepository = ProductInstallationStateRepository;
    }
    // GET: api/ProductInstallations
    [HttpGet]
    public IEnumerable<ProductInstallationDto> Get()
    {
      var states = _ProductInstallationStateRepository.GetProductInstallationStates();
      var dtos = states.Select(s => new ProductInstallationDto(s)).ToList();
      return dtos;
    }

    [Route("getproductinstallationsbycompanyid/{guid}")]
    public IEnumerable<ProductInstallationDto> GetProductInstallationsByCompanyGuid(Guid guid)
    {
      var states = _ProductInstallationStateRepository.GetProductInstallationsByCompanyGuid(guid);
      var dtos = states.Select(s => new ProductInstallationDto(s)).ToList();
      return dtos;
    }

    [Route("getproductinstallationsbyproductid/{guid}")]
    public IEnumerable<ProductInstallationDto> GetProductInstallationsByProductGuid(Guid guid)
    {
      var states = _ProductInstallationStateRepository.GetProductInstallationsByProductGuid(guid);
      var dtos = states.Select(s => new ProductInstallationDto(s)).ToList();
      return dtos;
    }

    // GET api/ProductInstallations/5
    [HttpGet("{guid}")]
    public ProductInstallationDto Get(Guid guid)
    {
      var state = _ProductInstallationStateRepository.GetProductInstallationState(guid);
      return new ProductInstallationDto(state);
    }
  }
}
