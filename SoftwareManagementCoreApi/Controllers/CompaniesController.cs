using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CompaniesShared;

// For more information on enabling Web API for empty products, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SoftwareManagementCoreApi.Controllers
{
    public class CompanyDto
    {
        private ICompanyState _state;
        public CompanyDto(ICompanyState state)
        {
            _state = state;
        }
        public Guid Guid { get { return _state.Guid; } }
        public string Name { get { return _state.Name; } }
    }

    [Route("api/[controller]")]
    public class CompaniesController : Controller
    {
        private ICompanyStateRepository _productStateRepository;

        public CompaniesController(ICompanyStateRepository productStateRepository)
        {
            _productStateRepository = productStateRepository;
        }
        // GET: api/products
        [HttpGet]
        public IEnumerable<CompanyDto> Get()
        {
            var states = _productStateRepository.GetCompanyStates();
            var dtos = states.Select(s => new CompanyDto(s)).ToList();
            return dtos;
        }

        // GET api/products/5
        [HttpGet("{guid}")]
        public CompanyDto Get(Guid guid)
        {
            var state = _productStateRepository.GetCompanyState(guid);
            return new CompanyDto(state);
        }
    }
}
