using CommandsShared;
using DateTimeShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductsShared
{
    public interface IProductState: IEntityState
    {
    }
    public interface IProductStateRepository : IEntityRepository
    {
        IProductState CreateProductState(Guid guid);
        IProductState GetProductState(Guid guid);
        IEnumerable<IProductState> GetProductStates();
        void DeleteProductState(Guid guid);
    }
    public interface IProduct
    {
        DateTime CreatedOn { get; }
        Guid Guid { get; }
        string Name { get; }

        void Rename(string name);
    }
    public class Product
    {
        private IProductState _state;
        public Product(IProductState state)
        {
            _state = state;
        }

        public Guid Guid { get { return _state.Guid; } }
        public string Name { get { return _state.Name; } }
        public DateTime CreatedOn { get { return _state.CreatedOn; } }

        public void Rename(string name)
        {
            _state.Name = name;
        }
    }
    public interface IProductService : ICommandProcessor
    {
        IProduct CreateProduct(Guid guid);
        IProduct GetProduct(Guid guid);
        void DeleteProduct(Guid entityGuid);
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
        public IProduct CreateProduct(Guid guid)
        {
            var state = _repo.CreateProductState(guid);
            state.CreatedOn = _dateTimeProvider.GetUtcDateTime();
            state.UpdatedOn = _dateTimeProvider.GetUtcDateTime();
            return new Product(state) as IProduct;
        }
        public IProduct GetProduct(Guid guid)
        {
            var state = _repo.GetProductState(guid);
            return new Product(state) as IProduct;
        }
        public void DeleteProduct(Guid guid)
        {
            _repo.DeleteProductState(guid);
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
            var product = _products.CreateProduct(_guid);
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
