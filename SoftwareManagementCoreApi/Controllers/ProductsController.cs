using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductsShared;

// For more information on enabling Web API for empty products, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SoftwareManagementCoreApi.Controllers
{
    public class ProductDto
    {
        private IProductState _state;
        public ProductDto(IProductState state)
        {
            _state = state;
        }
        public Guid Guid { get { return _state.Guid; } }
        public string Name { get { return _state.Name; } }
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
            return new ProductDto(state);
        }
    }
}
