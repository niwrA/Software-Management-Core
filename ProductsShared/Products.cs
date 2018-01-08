using CommandsShared;
using DateTimeShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProductsShared
{
  public interface IProductState : INamedEntityState
  {
    string Description { get; set; }
    string BusinessCase { get; set; }
    ICollection<IProductVersionState> ProductVersionStates { get; set; }
    ICollection<IProductFeatureState> ProductFeatureStates { get; set; }
    ICollection<IProductIssueState> ProductIssueStates { get; set; }
  }
  public interface IProductVersionState : INamedEntityState
  {
    int Major { get; set; }
    int Minor { get; set; }
    int Revision { get; set; }
    int Build { get; set; }
    Guid ProductGuid { get; set; }
  }
  public interface IProductFeatureState : INamedEntityState
  {
    Guid ProductGuid { get; set; }
    string Description { get; set; }
    bool IsRequest { get; set; }
    Guid? FirstVersionGuid { get; set; }
    Guid? RequestedForVersionGuid { get; set; }
    ICollection<IProductFeatureConfigOptionState> ProductFeatureConfigOptionStates { get; set; }
  }
  public interface IProductIssueState : INamedEntityState
  {
    string Description { get; set; }
    Guid FirstVersionGuid { get; set; }
    Guid ProductGuid { get; set; }
  }
  public interface IProductFeatureConfigOptionState : INamedEntityState
  {
    Guid ProductFeatureGuid { get; set; }
    Guid? ParentGuid { get; set; }
    string Path { get; set; }
    string Description { get; set; }
    bool IsDefaultOption { get; set; }
    bool IsOptionForParent { get; set; }
    string DefaultValue { get; set; }
  }

  public interface IProductStateRepository : IEntityRepository
  {
    IProductState CreateProductState(Guid guid, string name);
    IProductState GetProductState(Guid guid);
    IEnumerable<IProductState> GetProductStates();
    void DeleteProductState(Guid guid);
    IProductVersionState CreateProductVersionState(Guid guid, Guid productVersionGuid, string name);
    IProductFeatureState CreateProductFeatureState(Guid guid, Guid productFeatureGuid, string name);
    void MakeDefaultFeatureConfigOptionState(IProductFeatureConfigOptionState state);
    void DeleteProductFeatureState(Guid productGuid, Guid guid);
    void DeleteProductVersionState(Guid productGuid, Guid guid);
    IProductIssueState CreateProductIssueState(Guid productGuid, Guid guid, string name);
    void DeleteProductIssueState(Guid productGuid, Guid guid);
    IProductFeatureConfigOptionState CreateProductFeatureConfigOptionState(IProductFeatureState state, Guid guid, string name);
    IProductFeatureConfigOptionState GetProductFeatureConfigOptionState(IProductFeatureState state, Guid guid);
    void DeleteProductFeatureConfigOptionState(IProductFeatureState state, Guid guid);
    void MoveProductFeatureConfigOption(IProductFeatureConfigOptionState state, Guid parentGuid);
  }
  public interface IProductVersion : INamedEntity
  {
    int Major { get; }
    int Minor { get; }
    int Revision { get; }
    int Build { get; }
  }
  public interface IProductFeature : INamedEntity
  {
    void Rename(string name, string originalName);
    void ChangeDescription(string description);
    Guid? FirstVersionGuid { get; }
    IProductFeatureConfigOption AddConfigOption(Guid guid, string name);
    IProductFeatureConfigOption GetConfigOption(Guid guid);
    void DeleteConfigOption(Guid guid);

  }
  public interface IProductFeatureConfigOption : INamedEntity
  {
    Guid? ParentGuid { get; }
    Guid ProductFeatureGuid { get; }
    string Path { get; }
    string DefaultValue { get; }
    bool IsOptionForParent { get; }
    bool IsDefaultOption { get; }
    void Rename(string name, string originalName);
    void ChangeDescription(string description);
    void ChangeDefaultValue(string value, string originalValue);
    void MakeDefaultOption();
    void MoveToParent(Guid parentGuid, Guid originalParentGuid);
    void ChangePath(string value, string originalValue);
  }

  public interface IProductIssue : INamedEntity
  {
    void Rename(string name, string originalName);
    void ChangeDescription(string description);
    Guid FirstVersionGuid { get; }
  }
  public interface IProduct
  {
    DateTime CreatedOn { get; }
    Guid Guid { get; }
    string Name { get; }
    void Rename(string name, string original);
    void ChangeDescription(string description);
    void ChangeBusinessCase(string businessCase);
    IProductVersion AddVersion(Guid guid, string name, int major, int minor, int revision, int build);
    IProductFeature AddFeature(Guid guid, string name, Guid firstVersionGuid);
    IProductIssue AddIssue(Guid productIssueGuid, string name, Guid firstVersionGuid);
    IProductFeature GetFeature(Guid guid);
    IProductIssue GetIssue(Guid guid);
    IProductFeature RequestFeature(Guid productFeatureGuid, string name, Guid requestedForVersionGuid);
    void DeleteFeature(Guid guid);
    void DeleteVersion(Guid guid);
    void DeleteIssue(Guid guid);
  }
  public class NamedEntity : INamedEntity
  {
    private INamedEntityState _state;
    public NamedEntity(INamedEntityState state)
    {
      _state = state;
    }

    public Guid Guid { get { return _state.Guid; } }
    public string Name { get { return _state.Name; } }
    public DateTime CreatedOn { get { return _state.CreatedOn; } }
    public DateTime UpdatedOn { get { return _state.UpdatedOn; } }
  }
  public class Product : NamedEntity, IProduct
  {
    private IProductState _state;
    private IProductStateRepository _repo;

    public Product(IProductState state, IProductStateRepository repo) : base(state)
    {
      _state = state;
      _repo = repo;
    }

    public string Description { get { return _state.Description; } }
    public string BusinessCase { get { return _state.BusinessCase; } }
    public void Rename(string name, string originalName)
    {
      if (_state.Name == originalName)
      {
        _state.Name = name;
      }
      else
      {
        // todo: concurrency policy implementation
      }
    }
    // todo: rework to textfragment?
    public void ChangeDescription(string description)
    {
      _state.Description = description;
    }
    public void ChangeBusinessCase(string businessCase)
    {
      _state.BusinessCase = businessCase;
    }

    public IProductVersion AddVersion(Guid guid, string name, int major, int minor, int revision, int build)
    {
      var state = _repo.CreateProductVersionState(Guid, guid, name);
      state.Major = major;
      state.Minor = minor;
      state.Revision = revision;
      state.Build = build;
      state.ProductGuid = Guid;
      var productVersion = new ProductVersion(state, _repo);
      return productVersion;
    }
    public IProductFeature AddFeature(Guid guid, string name, Guid firstVersionGuid)
    {
      var state = _repo.CreateProductFeatureState(Guid, guid, name);
      state.FirstVersionGuid = firstVersionGuid;
      var productFeature = new ProductFeature(state, _repo);
      return productFeature;
    }
    public IProductIssue AddIssue(Guid guid, string name, Guid firstVersionGuid)
    {
      var state = _repo.CreateProductIssueState(Guid, guid, name);
      state.FirstVersionGuid = firstVersionGuid;
      var productIssue = new ProductIssue(state);
      return productIssue;
    }

    public IProductFeature RequestFeature(Guid guid, string name, Guid requestedForVersionGuid)
    {
      var state = _repo.CreateProductFeatureState(Guid, guid, name);
      state.RequestedForVersionGuid = requestedForVersionGuid;
      state.IsRequest = true;
      var productFeature = new ProductFeature(state, _repo);
      return productFeature;
    }

    public IProductFeature GetFeature(Guid guid)
    {
      var state = _state.ProductFeatureStates.FirstOrDefault(s => s.Guid == guid);
      if (state != null)
      {
        return new ProductFeature(state, _repo);
      }
      return null;
    }
    public IProductIssue GetIssue(Guid guid)
    {
      var state = _state.ProductIssueStates.FirstOrDefault(s => s.Guid == guid);
      if (state != null)
      {
        return new ProductIssue(state);
      }
      return null;
    }

    public void DeleteFeature(Guid guid)
    {
      _repo.DeleteProductFeatureState(this.Guid, guid);
    }
    public void DeleteVersion(Guid guid)
    {
      _repo.DeleteProductVersionState(this.Guid, guid);
    }
    public void DeleteIssue(Guid guid)
    {
      _repo.DeleteProductIssueState(this.Guid, guid);
    }

  }

  public class ProductVersion : NamedEntity, IProductVersion
  {
    private IProductVersionState _state;
    private IProductStateRepository _repo;
    public ProductVersion(IProductVersionState state, IProductStateRepository repo) : base(state)
    {
      _state = state;
      _repo = repo;
    }

    public int Major { get { return _state.Major; } }

    public int Minor { get { return _state.Minor; } }

    public int Revision { get { return _state.Revision; } }

    public int Build { get { return _state.Build; } }
  }
  public class ProductFeature : NamedEntity, IProductFeature
  {
    private IProductFeatureState _state;
    private IProductStateRepository _repo;

    public ProductFeature(IProductFeatureState state, IProductStateRepository repo) : base(state)
    {
      _state = state;
      _repo = repo;
    }
    public Guid? FirstVersionGuid { get { return _state.FirstVersionGuid; } }

    public void Rename(string name, string originalName)
    {
      if (_state.Name == originalName)
      {
        _state.Name = name;
      }
      // todo: implement concurrency policy
    }
    public void ChangeDescription(string description)
    {
      _state.Description = description;
    }

    public IProductFeatureConfigOption AddConfigOption(Guid guid, string name)
    {
      var state = _repo.CreateProductFeatureConfigOptionState(this._state, guid, name);
      var productFeature = new ProductFeatureConfigOption(state, _repo);
      return productFeature;
    }

    public IProductFeatureConfigOption GetConfigOption(Guid guid)
    {
      var state = _repo.GetProductFeatureConfigOptionState(this._state, guid);
      var productFeature = new ProductFeatureConfigOption(state, _repo);
      return productFeature;
    }

    public void DeleteConfigOption(Guid guid)
    {
      _repo.DeleteProductFeatureConfigOptionState(this._state, guid);
    }
  }
  // todo: move rename to base class?
  public class ProductIssue : NamedEntity, IProductIssue
  {
    private IProductIssueState _state;
    public ProductIssue(IProductIssueState state) : base(state)
    {
      _state = state;
    }
    public Guid FirstVersionGuid { get { return _state.FirstVersionGuid; } }

    public void Rename(string name, string originalName)
    {
      if (_state.Name == originalName)
      {
        _state.Name = name;
      }
      // todo: implement concurrency policy
    }
    public void ChangeDescription(string description)
    {
      _state.Description = description;
    }
  }
  public class ProductFeatureConfigOption : NamedEntity, IProductFeatureConfigOption
  {
    private IProductFeatureConfigOptionState _state;
    private IProductStateRepository _repo;

    public ProductFeatureConfigOption(IProductFeatureConfigOptionState state, IProductStateRepository repo) : base(state)
    {
      _state = state;
      _repo = repo;
    }
    public Guid? ParentGuid { get { return _state.ParentGuid; } }
    public Guid ProductFeatureGuid { get { return _state.ProductFeatureGuid; } }

    public string Path { get { return _state.Path; } }

    public string DefaultValue { get { return _state.DefaultValue; } }

    public bool IsOptionForParent { get { return _state.IsOptionForParent; } }

    public bool IsDefaultOption { get { return _state.IsDefaultOption; } }

    public void ChangeDefaultValue(string value, string originalValue)
    {
      if (_state.DefaultValue == originalValue)
      {
        _state.DefaultValue = value;
      }
    }

    public void ChangeDescription(string description)
    {
      _state.Description = description;
    }
    public void ChangePath(string value, string originalValue)
    {
      if (_state.Path == originalValue)
      {
        _state.Path = value;
      }
    }

    // todo: make other options false. Do this in command? Or call repository from here?
    public void MakeDefaultOption()
    {
      _repo.MakeDefaultFeatureConfigOptionState(_state);
    }

    // todo: may need to tell the repository?
    public void MoveToParent(Guid parentGuid, Guid originalParentGuid)
    {
      if (_state.ParentGuid == originalParentGuid)
      {
        _repo.MoveProductFeatureConfigOption(_state, parentGuid);
      }
    }

    public void Rename(string name, string originalName)
    {
      if (_state.Name == originalName)
      {
        _state.Name = name;
      }
    }
  }

  public interface IProductService : ICommandProcessor
  {
    IProduct CreateProduct(Guid guid, string name);
    IProduct GetProduct(Guid guid);
    void DeleteProduct(Guid entityGuid);
    void PersistChanges();
  }
  public class ProductService : IProductService
  {
    private IDateTimeProvider _dateTimeProvider;
    private IProductStateRepository _repo;
    public ProductService(IProductStateRepository repo, IDateTimeProvider dateTimeProvider)
    {
      _repo = repo;
      _dateTimeProvider = dateTimeProvider;
    }
    public IProduct CreateProduct(Guid guid, string name)
    {
      var state = _repo.CreateProductState(guid, name);
      state.Name = name;
      state.CreatedOn = _dateTimeProvider.GetUtcDateTime();
      state.UpdatedOn = _dateTimeProvider.GetUtcDateTime();
      return new Product(state, _repo) as IProduct;
    }
    public IProduct GetProduct(Guid guid)
    {
      var state = _repo.GetProductState(guid);
      return new Product(state, _repo) as IProduct;
    }
    public void DeleteProduct(Guid guid)
    {
      _repo.DeleteProductState(guid);
    }

    public void PersistChanges()
    {
      _repo.PersistChanges();
    }
  }
  public class ProductBuilder
  {
    private ProductService _products;
    private Guid _guid;
    private string _name;

    public ProductBuilder(ProductService products)
    {
      _products = products;
    }

    public IProduct Build()
    {
      EnsureGuid();
      var product = _products.CreateProduct(_guid, _name);
      return product;
    }

    private void EnsureGuid()
    {
      if (_guid == null || _guid == Guid.Empty)
      {
        _guid = Guid.NewGuid();
      }
    }

    public ProductBuilder WithGuid(Guid guid)
    {
      _guid = guid;
      return this;
    }

    public ProductBuilder WithName(string name)
    {
      _name = name;
      return this;
    }
  }
}
