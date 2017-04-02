using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectRoleAssignmentsShared;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projectroleassignments, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SoftwareManagementCoreApi.Controllers
{
    public class ProjectRoleAssignmentDto
    {
        private IProjectRoleAssignmentState _state;
        public ProjectRoleAssignmentDto(IProjectRoleAssignmentState state)
        {
            _state = state;
        }
        public Guid Guid { get { return _state.Guid; } }
        public Guid ContactGuid { get { return _state.ContactGuid; } }
        public Guid ProjectRoleGuid { get { return _state.ProjectRoleGuid; } }
        public string StartDate { get { return _state.StartDate.HasValue ? _state.StartDate.Value.ToString("yyyy-MM-dd") : ""; } }
        public string EndDate { get { return _state.EndDate.HasValue ? _state.EndDate.Value.ToString("yyyy-MM-dd") : ""; } }
        public string ContactName { get { return _state.ContactName;  } }
    }

    [Route("api/[controller]")]
    public class ProjectRoleAssignmentsController : Controller
    {
        private IProjectRoleAssignmentStateRepository _projectroleassignmentStateRepository;

        public ProjectRoleAssignmentsController(IProjectRoleAssignmentStateRepository projectroleassignmentStateRepository)
        {
            _projectroleassignmentStateRepository = projectroleassignmentStateRepository;
        }
        // GET: api/projectroleassignments
        [HttpGet]
        public IEnumerable<ProjectRoleAssignmentDto> Get()
        {
            var states = _projectroleassignmentStateRepository.GetProjectRoleAssignmentStates();
            var dtos = states.Select(s => new ProjectRoleAssignmentDto(s)).ToList();
            return dtos;
        }

        // todo: this one can probably be deleted again when the projectroleassignments contains the ContactName
        // [Route("projectrole/{guid}/contacts")]
        [Route("getcontactsbyprojectroleid/{guid}")]
        public IEnumerable<ContactDto> GetContactsByProjectRoleId(Guid guid)
        {
            var states = _projectroleassignmentStateRepository.GetContactsByProjectRoleGuid(guid);
            var dtos = states.Select(s => new ContactDto(s)).ToList();
            return dtos;
        }

        // GET: api/projectroleassignments/getbyprojectroleid/5
        // [Route("projectrole/{guid}/contacts")]
        [Route("getprojectroleassignmentsbyprojectroleid/{guid}")]
        public IEnumerable<ProjectRoleAssignmentDto> GetProjectRoleAssignmentsByProjectRoleId(Guid guid)
        {
            var states = _projectroleassignmentStateRepository.GetProjectRoleAssignmentsByProjectRoleGuid(guid);
            var dtos = states.Select(s => new ProjectRoleAssignmentDto(s)).ToList();
            return dtos;
        }

        // GET: api/projectroleassignments/getbycontactid/5
        [HttpGet("/getbycontactid/{guid}")]
        public IEnumerable<ProjectRoleAssignmentDto> GetByContactId(Guid guid)
        {
            var states = _projectroleassignmentStateRepository.GetProjectRoleAssignmentsByContactGuid(guid);
            var dtos = states.Select(s => new ProjectRoleAssignmentDto(s)).ToList();
            return dtos;
        }

        // GET api/projectroleassignments/5
        [HttpGet("{guid}")]
        public ProjectRoleAssignmentDto Get(Guid guid)
        {
            var state = _projectroleassignmentStateRepository.GetProjectRoleAssignmentState(guid);
            return new ProjectRoleAssignmentDto(state);
        }
    }
}
