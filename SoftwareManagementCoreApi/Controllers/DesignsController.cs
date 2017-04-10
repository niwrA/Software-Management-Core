using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DesignsShared;

// For more information on enabling Web API for empty designs, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SoftwareManagementCoreApi.Controllers
{
    public class DesignDto
    {
        private IDesignState _state;
        public DesignDto(IDesignState state)
        {
            _state = state;
        }
        public Guid Guid { get { return _state.Guid; } }
        public string Name { get { return _state.Name; } }
        public string Description {  get { return _state.Description; } }
        public string CreatedOn { get { return _state.CreatedOn.ToString("yyyy-MM-dd"); } }
        public string UpdatedOn { get { return _state.UpdatedOn.ToString("yyyy-MM-dd"); } }

    }

    [Route("api/[controller]")]
    public class DesignsController : Controller
    {
        private IDesignStateRepository _designStateRepository;

        public DesignsController(IDesignStateRepository designStateRepository)
        {
            _designStateRepository = designStateRepository;
        }
        // GET: api/designs
        [HttpGet]
        public IEnumerable<DesignDto> Get()
        {
            var states = _designStateRepository.GetDesignStates();
            var dtos = states.Select(s => new DesignDto(s)).ToList();
            return dtos;
        }

        // GET api/designs/5
        [HttpGet("{guid}")]
        public DesignDto Get(Guid guid)
        {
            var state = _designStateRepository.GetDesignState(guid);
            if (state != null)
            {
                var dto = new DesignDto(state);
                return dto;
            }
            return null;
        }
    }
}
