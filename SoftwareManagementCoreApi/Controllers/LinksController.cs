using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LinksShared;

// For more information on enabling Web API for empty products, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SoftwareManagementCoreApi.Controllers
{
    public class LinkDto
    {
        private ILinkState _state;
        public LinkDto(ILinkState state)
        {
            _state = state;
        }
        public Guid Guid { get { return _state.Guid; } }
        public Guid LinkForGuid { get { return _state.ForGuid; } }
        public string Name { get { return _state.Name; } }
        public string Url { get { return _state.Url; } }

        public string CreatedOn { get { return _state.CreatedOn.ToString("yyyy-MM-dd"); } }
        public string Description { get { return _state.Description; } }
        public Guid EntityGuid { get { return _state.EntityGuid; } }
        public string ImageUrl { get { return _state.ImageUrl; } }
        public string UpdatedOn { get { return _state.UpdatedOn.ToString("yyyy-MM-dd"); } }
    }

    [Route("api/[controller]")]
    public class LinksController : Controller
    {
        private ILinkStateRepository _linkStateRepository;

        public LinksController(ILinkStateRepository productStateRepository)
        {
            _linkStateRepository = productStateRepository;
        }
        // GET: api/products
        [HttpGet]
        public IEnumerable<LinkDto> Get()
        {
            var states = _linkStateRepository.GetLinkStates();
            var dtos = states.Select(s => new LinkDto(s)).ToList();
            return dtos;
        }

        // GET api/links/5
        [HttpGet("{guid}")]
        public LinkDto Get(Guid guid)
        {
            var state = _linkStateRepository.GetLinkState(guid);
            return new LinkDto(state);
        }

        // GET api/links/forguid/5
        [HttpGet("forGuid/{forGuid}")]
        public IEnumerable<LinkDto> GetForGuid(Guid forGuid)
        {
            var states = _linkStateRepository.GetLinkStatesForGuid(forGuid);
            var dtos = states.Select(s => new LinkDto(s)).ToList();
            return dtos;
        }
    }
}
