using CommandsShared;
using DateTimeShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductsShared
{
    public interface IProductState : INamedEntityState
    {
        string Description { get; set; }
        string BusinessCase { get; set; }
        ICollection<IProductVersionState> ProductVersionStates { get; set; }

    }
    public interface IProductVersionState : INamedEntityState
    {
        int Major { get; set; }
        int Minor { get; set; }
        int Revision { get; set; }
        int Build { get; set; }
        Guid ProductGuid { get; set; }
    }
    public interface IProductStateRepository : IEntityRepository
    {
        IProductState CreateProductState(Guid guid, string name);
        IProductState GetProductState(Guid guid);
        IEnumerable<IProductState> GetProductStates();
        void DeleteProductState(Guid guid);
        IProductVersionState CreateProductVersionState(Guid guid, Guid productVersionGuid, string name);
    }
    public interface IProductVersion
    {
        DateTime CreatedOn { get; }
        Guid Guid { get; }
        string Name { get; }
        int Major { get; }
        int Minor { get; }
        int Revision { get; }
        int Build { get; }
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
        IReadOnlyCollection<IProductVersion> ProductVersions { get; }
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

        public IReadOnlyCollection<IProductVersion> ProductVersions => throw new NotImplementedException();

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
    }

    public class ProductVersion : IProductVersion
    {
        private IProductVersionState _state;
        public ProductVersion(IProductVersionState state)
        {
            _state = state;
        }
        public DateTime CreatedOn { get { return _state.CreatedOn; } }

        public Guid Guid { get { return _state.Guid; } }

        public string Name { get { return _state.Name; } }

        public int Major { get { return _state.Major; } }

        public int Minor { get { return _state.Minor; } }

        public int Revision { get { return _state.Revision; } }

        public int Build { get { return _state.Build; } }
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
