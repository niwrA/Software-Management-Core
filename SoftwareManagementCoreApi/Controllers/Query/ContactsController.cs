using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContactsShared;

// For more information on enabling Web API for empty products, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SoftwareManagementCoreApi.Controllers
{
    public class ContactDto
    {
        private IContactState _state;
        public ContactDto(IContactState state)
        {
            _state = state;
        }
        public Guid Guid { get { return _state.Guid; } }
        public string Name { get { return _state.Name; } }
        public string Email { get { return _state.Email; } }
        public string BirthDate { get { return _state.BirthDate.HasValue ? _state.BirthDate.Value.ToString("yyyy-MM-dd") : ""; } }
        public string AvatarUrl {  get { return _state.AvatarUrl; } }
        public Guid? AvatarFileGuid { get { return _state.AvatarFileGuid; } }
    }

    [Route("api/[controller]")]
    public class ContactsController : Controller
    {
        private IContactStateReadOnlyRepository _contactStateRepository;

        public ContactsController(IContactStateReadOnlyRepository productStateRepository)
        {
            _contactStateRepository = productStateRepository;
        }
        // GET: api/products
        [HttpGet]
        public IEnumerable<ContactDto> Get()
        {
            var states = _contactStateRepository.GetContactStates();
            var dtos = states.Select(s => new ContactDto(s)).ToList();
            return dtos;
        }

        // GET api/products/5
        [HttpGet("{guid}")]
        public ContactDto Get(Guid guid)
        {
            var state = _contactStateRepository.GetContactState(guid);
            return new ContactDto(state);
        }
    }
}
