using CommandsShared;
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
        public DateTime Created { get { return _state.Created; } }

        public void Rename(string name)
        {
            _state.Name = name;

        }
    }
    public class Products
    {
        private IProductStateRepository _repo;
        public Products(IProductStateRepository repo)
        {
            _repo = repo;
        }
        public Product CreateProduct(Guid guid)
        {
            var state = _repo.CreateProductState(guid);
            return new Product(state);
        }
        public Product GetProduct(Guid guid)
        {
            var state = _repo.GetProductState(guid);
            return new Product(state);
        }

    }
    public class ProductBuilder
    {
        private Products _products;
        private Guid _guid;
        private string _name;

        public ProductBuilder(Products products)
        {
            _products = products;
        }

        public Product Build()
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
