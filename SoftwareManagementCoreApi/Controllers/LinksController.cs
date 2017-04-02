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
        public Guid LinkForGuid { get { return _state.LinkForGuid; } }
        public string Name { get { return _state.Name; } }
        public string Url { get { return _state.Url; } }
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

        // GET api/products/5
        [HttpGet("{guid}")]
        public LinkDto Get(Guid guid)
        {
            var state = _linkStateRepository.GetLinkState(guid);
            return new LinkDto(state);
        }
    }
}
