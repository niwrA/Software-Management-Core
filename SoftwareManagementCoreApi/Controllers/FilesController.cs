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
        public string ForType { get { return _state.ForType; } }
        public string Name { get { return _state.Name; } }
        public string FileName { get { return _state.FileName; } }
        public string FolderName { get { return _state.FolderName; } }

        public string Description { get { return _state.Description; } }
        public Guid EntityGuid { get { return _state.EntityGuid; } }
        public string CreatedOn { get { return _state.CreatedOn.ToString("yyyy-MM-dd"); } }
        public string UpdatedOn { get { return _state.UpdatedOn.ToString("yyyy-MM-dd"); } }
        public string Type { get { return _state.Type; } }
        public string ContentType { get { return _state.ContentType; } }
        public long Size { get { return _state.Size; } }
    }

    [Route("api/[controller]")]
    public class FilesController : Controller
    {
        private IFileStateRepository _fileStateRepository;

        public FilesController(IFileStateRepository stateRepository)
        {
            _fileStateRepository = stateRepository;
        }
        // GET: api/products
        [HttpGet]
        public IEnumerable<FileDto> Get()
        {
            var states = _fileStateRepository.GetFileStates();
            var dtos = states.Select(s => new FileDto(s)).ToList();
            return dtos;
        }

        // GET api/links/5
        [HttpGet("{guid}")]
        public FileDto Get(Guid guid)
        {
            var state = _fileStateRepository.GetFileState(guid);
            return new FileDto(state);
        }

        // GET api/links/forguid/5
        [HttpGet("forGuid/{forGuid}")]
        public IEnumerable<FileDto> GetForGuid(Guid forGuid)
        {
            var states = _fileStateRepository.GetFileStatesForGuid(forGuid);
            var dtos = states.Select(s => new FileDto(s)).ToList();
            return dtos;
        }
    }
}
