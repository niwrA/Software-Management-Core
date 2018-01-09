using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductsShared;

// For more information on enabling Web API for empty products, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SoftwareManagementCoreApi.Controllers
{
  public class ProductVersionDto
  {
    private IProductVersionState _state;
    public ProductVersionDto(IProductVersionState state)
    {
      _state = state;
    }
    public Guid Guid { get { return _state.Guid; } }
    public string Name { get { return _state.Name; } }
    public int Major { get { return _state.Major; } }
    public int Minor { get { return _state.Minor; } }
    public int Revision { get { return _state.Revision; } }
    public int Build { get { return _state.Build; } }
    public Guid ProductGuid { get { return _state.ProductGuid; } }
  }
  public class ProductConfigOptionDto
  {
    private IProductConfigOptionState _state;

    public ProductConfigOptionDto(IProductConfigOptionState state)
    {
      _state = state;
    }
    public Guid Guid { get { return _state.Guid; } }
    public string Name { get { return _state.Name; } }
    public string DefaultValue { get { return _state.DefaultValue; } }
    public string Description { get { return _state.Description; } }
    public bool IsDefaultOption { get { return _state.IsDefaultOption; } }
    public bool IsOptionForParent { get { return _state.IsOptionForParent; } }
    public Guid? ParentGuid { get { return _state.ParentGuid; } }
    public string Path { get { return _state.Path; } }
    public Guid ProductGuid { get { return _state.ProductGuid; } }
    public Guid? ProductFeatureGuid { get { return _state.ProductFeatureGuid; } }
  }
  public class ProductFeatureDto
  {
    private IProductFeatureState _state;
    public ProductFeatureDto(IProductFeatureState state)
    {
      _state = state;
    }
    public Guid Guid { get { return _state.Guid; } }
    public string Name { get { return _state.Name; } }
    public string Description { get { return _state.Description; } }
    public bool IsRequest { get { return _state.IsRequest; } }
    public Guid ProductGuid { get { return _state.ProductGuid; } }
    public Guid? FirstVersionGuid { get { return _state.FirstVersionGuid; } }
    public Guid? RequestedForVersionGuid { get { return _state.RequestedForVersionGuid; } }
  }
  public class ProductIssueDto
  {
    private IProductIssueState _state;
    public ProductIssueDto(IProductIssueState state)
    {
      _state = state;
    }
    public Guid Guid { get { return _state.Guid; } }
    public string Name { get { return _state.Name; } }
    public string Description { get { return _state.Description; } }
    public Guid ProductGuid { get { return _state.ProductGuid; } }
    public Guid FirstVersionGuid { get { return _state.FirstVersionGuid; } }
  }


  public class ProductDto
  {
    private IProductState _state;
    public ProductDto(IProductState state)
    {
      _state = state;
    }
    public Guid Guid { get { return _state.Guid; } }
    public string Name { get { return _state.Name; } }
    public string Description { get { return _state.Description; } }
    public string BusinessCase { get { return _state.BusinessCase; } }
    public ICollection<ProductVersionDto> Versions { get { return _state.ProductVersionStates?.Select(s => new ProductVersionDto(s)).ToList(); } }
    public ICollection<ProductFeatureDto> Features { get { return _state.ProductFeatureStates?.Select(s => new ProductFeatureDto(s)).ToList(); } }
    public ICollection<ProductIssueDto> Issues { get { return _state.ProductIssueStates?.Select(s => new ProductIssueDto(s)).ToList(); } }
    public ICollection<ProductConfigOptionDto> ConfigOptions { get { return _state.ProductConfigOptionStates?.Select(s => new ProductConfigOptionDto(s)).ToList(); } }
  }

  [Route("api/[controller]")]
  public class ProductsController : Controller
  {
    private IProductStateRepository _productStateRepository;

    public ProductsController(IProductStateRepository productStateRepository)
    {
      _productStateRepository = productStateRepository;
    }
    // GET: api/products
    [HttpGet]
    public IEnumerable<ProductDto> Get()
    {
      var states = _productStateRepository.GetProductStates();
      var dtos = states.Select(s => new ProductDto(s)).ToList();
      return dtos;
    }

    // GET api/products/5
    [HttpGet("{guid}")]
    public ProductDto Get(Guid guid)
    {
      var state = _productStateRepository.GetProductState(guid);
      if (state != null)
      {
        var dto = new ProductDto(state);
        return dto;
      }
      return null;
    }
  }
}
