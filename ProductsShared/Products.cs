using CommandsShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductsShared
{
    public interface IProductState
    {
        Guid Guid { get; set; }
        string Name { get; set; }
        DateTime Created { get; set; }
    }
    public interface IProductStateRepository: IEntityRepository
    {
        IProductState CreateProductState(Guid guid);
    }
    public class Product
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
        public DateTime Created { get { return _state.Created;  } }

        public void RenameProduct(string name)
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
            return new Product(state, _repo);
        }
    }
}
