using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using ProductsShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareManagementMongoDbCoreRepository
{
  [BsonIgnoreExtraElements]
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
    public ICollection<IProductFeatureConfigOptionState> ProductFeatureConfigOptionStates { get; set; } = new List<IProductFeatureConfigOptionState>();
  }
  public class ProductIssueState : NamedEntityState, IProductIssueState
  {
    public Guid ProductGuid { get; set; }
    public string Description { get; set; }
    public Guid FirstVersionGuid { get; set; }
  }

  [BsonIgnoreExtraElements]
  public class ProductState : NamedEntityState, IProductState
  {
    public ProductState()
    {
      ProductVersionStates = new List<IProductVersionState>() as ICollection<IProductVersionState>;
      ProductFeatureStates = new List<IProductFeatureState>() as ICollection<IProductFeatureState>;
    }

    public string Description { get; set; }
    public string BusinessCase { get; set; }
    public ICollection<IProductVersionState> ProductVersionStates { get; set; }
    public ICollection<IProductFeatureState> ProductFeatureStates { get; set; }
    public ICollection<IProductIssueState> ProductIssueStates { get; set; }
  }

  public class ProductStateRepository : IProductStateRepository
  {
    private const string ProductStatesCollection = "ProductStates";

    private IMongoClient _client;
    private IMongoDatabase _database;

    private Dictionary<Guid, IProductState> _productStates;
    private List<Guid> _deletedProductStates;
    private Dictionary<Guid, IProductState> _updatedProductStates;

    public ProductStateRepository(IMongoClient client)
    {
      _client = client;
      _database = _client.GetDatabase("SoftwareManagement");

      _productStates = new Dictionary<Guid, IProductState>();
      _deletedProductStates = new List<Guid>();
      _updatedProductStates = new Dictionary<Guid, IProductState>();

    }


    public IProductState CreateProductState(Guid guid, string name)
    {
      var state = new ProductState()
      {
        Guid = guid
      };
      _productStates.Add(state.Guid, state);
      return state;
    }
    public void DeleteProductState(Guid guid)
    {
      _deletedProductStates.Add(guid);
    }

    public IProductVersionState CreateProductVersionState(Guid guid, Guid versionGuid, string name)
    {
      var state = GetProductState(guid);
      var versionState = new ProductVersionState()
      {
        Guid = versionGuid,
        Name = name
      };
      state.ProductVersionStates.Add(versionState);
      return versionState;
    }

    public IProductFeatureState CreateProductFeatureState(Guid guid, Guid featureGuid, string name)
    {
      var state = GetProductState(guid);
      var featureState = new ProductFeatureState()
      {
        Guid = featureGuid,
        Name = name,
        ProductGuid = guid
      };
      state.ProductFeatureStates.Add(featureState);
      return featureState;
    }
    // todo: make upgrading more robust by creating collections if necessary?
    // it is necessary to check for a null collection now if the product was created
    // before issues was supported
    public IProductIssueState CreateProductIssueState(Guid guid, Guid issueGuid, string name)
    {
      var state = GetProductState(guid);
      var issueState = new ProductIssueState()
      {
        Guid = issueGuid,
        Name = name,
        ProductGuid = guid
      };
      if (state.ProductIssueStates == null)
      {
        state.ProductIssueStates = new List<IProductIssueState>();
      }
      state.ProductIssueStates.Add(issueState);
      return issueState;
    }


    private void TrackProductState(IProductState state)
    {
      if (state != null && !_updatedProductStates.ContainsKey(state.Guid))
      {
        _updatedProductStates.Add(state.Guid, state);
      }
    }

    // todo: specify if read-only? it will likely barely matter in most cases,
    // as long as for write mode persistchanges does the work
    public IProductState GetProductState(Guid guid)
    {
      if (!_productStates.TryGetValue(guid, out IProductState state))
      {
        if (!_updatedProductStates.TryGetValue(guid, out state))
        {
          var collection = _database.GetCollection<ProductState>(ProductStatesCollection);
          var filter = Builders<ProductState>.Filter.Eq("Guid", guid);

          state = collection.Find(filter).FirstOrDefault();

          TrackProductState(state);
        }
      }
      return state;
    }

    // readonly by default. Should we enhance the interface? Or create a separate read-only repo?
    public IEnumerable<IProductState> GetProductStates()
    {
      var collection = _database.GetCollection<ProductState>(ProductStatesCollection);
      var filter = new BsonDocument();
      var states = collection.Find(filter);

      return states?.ToList();
    }

    private void PersistProducts()
    {
      var productCollection = _database.GetCollection<ProductState>(ProductStatesCollection);
      // inserts
      if (_productStates.Values.Any())
      {
        var products = _productStates.Values.Select(s => s as ProductState).ToList();
        productCollection.InsertMany(products);
        _productStates.Clear();
      }

      // todo: can these be batched?
      // updates
      if (_updatedProductStates.Values.Any())
      {
        var products = _updatedProductStates.Values.Select(s => s as ProductState).ToList();
        foreach (var state in products)
        {
          var filter = Builders<ProductState>.Filter.Eq("Guid", state.Guid);
          productCollection.ReplaceOne(filter, state);
        }
        _updatedProductStates.Clear();
      }

      // deletes
      if (_deletedProductStates.Any())
      {
        var collection = _database.GetCollection<ProductState>(ProductStatesCollection);
        foreach (var guid in _deletedProductStates)
        {
          var filter = Builders<ProductState>.Filter.Eq("Guid", guid);
          collection.DeleteOne(filter);
        }
        _deletedProductStates.Clear();
      }
    }

    public void PersistChanges()
    {
      PersistProducts();
    }

    public Task PersistChangesAsync()
    {
      throw new NotImplementedException();
    }

    public void DeleteProductFeatureState(Guid productGuid, Guid guid)
    {
      var productState = GetProductState(productGuid);
      var featureState = productState.ProductFeatureStates.FirstOrDefault(s => s.Guid == guid);
      if (featureState != null)
      {
        productState.ProductFeatureStates.Remove(featureState);
      }
    }
    public void DeleteProductVersionState(Guid productGuid, Guid guid)
    {
      var productState = GetProductState(productGuid);
      var versionState = productState.ProductVersionStates.FirstOrDefault(s => s.Guid == guid);
      if (versionState != null)
      {
        productState.ProductVersionStates.Remove(versionState);
      }
    }
    public void DeleteProductIssueState(Guid productGuid, Guid guid)
    {
      var productState = GetProductState(productGuid);
      var issueState = productState.ProductIssueStates.FirstOrDefault(s => s.Guid == guid);
      if (issueState != null)
      {
        productState.ProductIssueStates.Remove(issueState);
      }
    }

    public IProductFeatureConfigOptionState CreateProductFeatureConfigOptionState(IProductFeatureState state, Guid guid, string name)
    {
      throw new NotImplementedException();
    }

    public IProductFeatureConfigOptionState GetProductFeatureConfigOptionState(IProductFeatureState state, Guid guid)
    {
      throw new NotImplementedException();
    }

    public void DeleteProductFeatureConfigOptionState(IProductFeatureState state, Guid guid)
    {
      throw new NotImplementedException();
    }

    public void MoveProductFeatureConfigOption(IProductFeatureConfigOptionState state, Guid parentGuid)
    {
      state.ParentGuid = parentGuid;
    }

    public void MakeDefaultFeatureConfigOptionState(IProductFeatureConfigOptionState state)
    {
      throw new NotImplementedException();
    }
  }
}
