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
            }

    public interface IProductStateRepository : IEntityRepository
    {
        IProductState CreateProductState(Guid guid, string name);
        IProductState GetProductState(Guid guid);
        IEnumerable<IProductState> GetProductStates();
        void DeleteProductState(Guid guid);
        IProductVersionState CreateProductVersionState(Guid guid, Guid productVersionGuid, string name);
        IProductFeatureState CreateProductFeatureState(Guid guid, Guid productFeatureGuid, string name);
        void DeleteProductFeatureState(Guid productGuid, Guid guid);
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
        IProductFeature GetFeature(Guid featureGuid);
        IProductFeature RequestFeature(Guid productFeatureGuid, string name, Guid requestedForVersionGuid);
        void DeleteFeature(Guid guid);
    }
    public class Product : IProduct
    {
        private IProductState _state;
        private IProductStateRepository _repo;

        public Product(IProductState state, IProductStateRepository repo)
        {
            _state = state;
            _repo = repo;
        }

        public Guid Guid { get { return _state.Guid; } }
        public string Name { get { return _state.Name; } }
        public string Description { get { return _state.Description; } }
        public string BusinessCase { get { return _state.BusinessCase; } }
        public DateTime CreatedOn { get { return _state.CreatedOn; } }
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
            var productVersion = new ProductVersion(state);
            return productVersion;
        }
        public IProductFeature AddFeature(Guid guid, string name, Guid firstVersionGuid)
        {
            var state = _repo.CreateProductFeatureState(Guid, guid, name);
            state.FirstVersionGuid = firstVersionGuid;
            var productFeature = new ProductFeature(state);
            return productFeature;
        }

        public IProductFeature RequestFeature(Guid guid, string name, Guid requestedForVersionGuid)
        {
            var state = _repo.CreateProductFeatureState(Guid, guid, name);
            state.RequestedForVersionGuid = requestedForVersionGuid;
            state.IsRequest = true;
            var productFeature = new ProductFeature(state);
            return productFeature;
        }

        public IProductFeature GetFeature(Guid featureGuid)
        {
            var state = _state.ProductFeatureStates.FirstOrDefault(s => s.Guid == featureGuid);
            if (state != null)
            {
                return new ProductFeature(state);
            }
            return null;
        }

        // todo: hide in repository?
        public void DeleteFeature(Guid guid)
        {
            _repo.DeleteProductFeatureState(this.Guid, guid);
        }
    }

    public class ProductVersion : IProductVersion
    {
        private IProductVersionState _state;
        public ProductVersion(IProductVersionState state)
        {
            _state = state;
        }
        public DateTime CreatedOn { get { return _state.CreatedOn; } }
        public DateTime UpdatedOn { get { return _state.UpdatedOn; } }

        public Guid Guid { get { return _state.Guid; } }

        public string Name { get { return _state.Name; } }

        public int Major { get { return _state.Major; } }

        public int Minor { get { return _state.Minor; } }

        public int Revision { get { return _state.Revision; } }

        public int Build { get { return _state.Build; } }
    }
    public class ProductFeature : IProductFeature
    {
        private IProductFeatureState _state;
        public ProductFeature(IProductFeatureState state)
        {
            _state = state;
        }
        public DateTime CreatedOn { get { return _state.CreatedOn; } }
        public DateTime UpdatedOn { get { return _state.UpdatedOn; } }

        public Guid Guid { get { return _state.Guid; } }
        public Guid? FirstVersionGuid { get { return _state.FirstVersionGuid; } }

        public string Name { get { return _state.Name; } }

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
