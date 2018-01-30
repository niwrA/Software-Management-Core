using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectsShared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SoftwareManagementCoreApi.Controllers
{
    public class ProjectRoleDto
    {
        private IProjectRoleState _state;
        public ProjectRoleDto(IProjectRoleState state)
        {
            _state = state;
        }
        public Guid Guid { get { return _state.Guid; } }
        public string Name { get { return _state.Name; } }
    }
    public class ProjectDto
    {
        private IProjectState _state;
        private List<ProjectRoleDto> _projectRoleStates;
        public ProjectDto(IProjectState state)
        {
            _state = state;
            _projectRoleStates = _state.ProjectRoleStates.Select(s => new ProjectRoleDto(s)).ToList();
        }
        public Guid Guid { get { return _state.Guid; } }
        public string Name { get { return _state.Name; } }
        public string StartDate { get { return _state.StartDate.HasValue ? _state.StartDate.Value.ToString("yyyy-MM-dd") : ""; } }
        public string EndDate { get { return _state.EndDate.HasValue ? _state.EndDate.Value.ToString("yyyy-MM-dd") : ""; } }
        public List<ProjectRoleDto> ProjectRoles { get { return _projectRoleStates; } }
    }

    [Route("api/[controller]")]
    public class ProjectsController : Controller
    {
        private IProjectStateRepository _projectStateRepository;

        public ProjectsController(IProjectStateRepository projectStateRepository)
        {
            _projectStateRepository = projectStateRepository;
        }
        // GET: api/projects
        [HttpGet]
        public IEnumerable<ProjectDto> Get()
        {
            var states = _projectStateRepository.GetProjectStates();
            var dtos = states.Select(s => new ProjectDto(s)).ToList();
            return dtos;
        }

        // GET api/projects/5
        [HttpGet("{guid}")]
        public ProjectDto Get(Guid guid)
        {
            var state = _projectStateRepository.GetProjectState(guid);
            return new ProjectDto(state);
        }
    }
}
