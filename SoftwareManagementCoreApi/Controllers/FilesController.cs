using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FilesShared;

// For more information on enabling Web API for empty products, visit https://go.microsoft.com/fwlink/?FileID=397860

namespace SoftwareManagementCoreApi.Controllers
{
    public class FileDto
    {
        private IFileState _state;
        public FileDto(IFileState state)
        {
            _state = state;
        }
        public Guid Guid { get { return _state.Guid; } }
        public Guid ForGuid { get { return _state.ForGuid; } }
        public string Name { get { return _state.Name; } }
        public string Url { get { return _state.FolderName; } }

        public string Description { get { return _state.Description; } }
        public Guid EntityGuid { get { return _state.EntityGuid; } }
        public string ImageUrl { get { return _state.ImageUrl; } }
        public string CreatedOn { get { return _state.CreatedOn.ToString("yyyy-MM-dd"); } }
        public string UpdatedOn { get { return _state.UpdatedOn.ToString("yyyy-MM-dd"); } }
        public string Path { get { return _state.Path; } }
public string Type
{
    get;
    set;
}    }

    [Route("api/[controller]")]
    public class FilesController : Controller
    {
        private IFileStateRepository _linkStateRepository;

        public FilesController(IFileStateRepository productStateRepository)
        {
            _linkStateRepository = productStateRepository;
        }
        // GET: api/products
        [HttpGet]
        public IEnumerable<FileDto> Get()
        {
            var states = _linkStateRepository.GetFileStates();
            var dtos = states.Select(s => new FileDto(s)).ToList();
            return dtos;
        }

        // GET api/links/5
        [HttpGet("{guid}")]
        public FileDto Get(Guid guid)
        {
            var state = _linkStateRepository.GetFileState(guid);
            return new FileDto(state);
        }

        // GET api/links/forguid/5
        [HttpGet("forGuid/{forGuid}")]
        public IEnumerable<FileDto> GetForGuid(Guid forGuid)
        {
            var states = _linkStateRepository.GetFileStatesForGuid(forGuid);
            var dtos = states.Select(s => new FileDto(s)).ToList();
            return dtos;
        }
    }
}
