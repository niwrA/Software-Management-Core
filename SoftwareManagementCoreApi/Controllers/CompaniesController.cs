using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CompaniesShared;

// For more information on enabling Web API for empty products, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SoftwareManagementCoreApi.Controllers
{
    public class CompanyRoleDto
    {
        private ICompanyRoleState _state;
        public CompanyRoleDto(ICompanyRoleState state)
        {
            _state = state;
        }
        public Guid Guid { get { return _state.Guid; } }
        public string Name { get { return _state.Name; } }
    }

    public class CompanyDto
    {
        private ICompanyState _state;
        private List<CompanyRoleDto> _companyRoleStates;

        public CompanyDto(ICompanyState state)
        {
            _state = state;
            _companyRoleStates = _state.CompanyRoleStates.Select(s => new CompanyRoleDto(s)).ToList();
        }
        public Guid Guid { get { return _state.Guid; } }
        public string Name { get { return _state.Name; } }
        public List<CompanyRoleDto> CompanyRoles { get { return _companyRoleStates; } }

    }

    [Route("api/[controller]")]
    public class CompaniesController : Controller
    {
        private ICompanyStateRepository _companyStateRepository;

        public CompaniesController(ICompanyStateRepository companyStateRepository)
        {
            _companyStateRepository = companyStateRepository;
        }
        // GET: api/products
        [HttpGet]
        public IEnumerable<CompanyDto> Get()
        {
            var states = _companyStateRepository.GetCompanyStates();
            var dtos = states.Select(s => new CompanyDto(s)).ToList();
            return dtos;
        }

        // GET api/products/5
        [HttpGet("{guid}")]
        public CompanyDto Get(Guid guid)
        {
            var state = _companyStateRepository.GetCompanyState(guid);
            return new CompanyDto(state);
        }
    }
}
