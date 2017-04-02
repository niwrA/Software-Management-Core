using CommandsShared;
using ContactsShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectRoleAssignmentsShared
{
    public interface IProjectRoleAssignmentState: IEntityState
    {
        Guid ContactGuid { get; set; }
        Guid ProjectRoleGuid { get; set; }
        string ContactName { get; set; }
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
    }
    public interface IProjectRoleAssignment
    {
        Guid Guid { get; }
        Guid ContactGuid { get; }
        Guid ProjectRoleGuid { get; }
        DateTime? StartDate { get; }
        DateTime? EndDate { get; }

        void ChangeStartDate(DateTime? startDate, DateTime? originalStartDate);
        void ChangeEndDate(DateTime? endDate, DateTime? originalEndDate);
        void Terminate(DateTime endDate);
    }
    public interface IProjectRoleAssignmentStateRepository
    {
        IProjectRoleAssignmentState CreateProjectRoleAssignmentState(Guid guid, Guid contactGuid, Guid companyRoleGuid);
        IProjectRoleAssignmentState GetProjectRoleAssignmentState(Guid guid);
        IEnumerable<IProjectRoleAssignmentState> GetProjectRoleAssignmentsByProjectRoleGuid(Guid companyRoleGuid);
        IEnumerable<IProjectRoleAssignmentState> GetProjectRoleAssignmentsByContactGuid(Guid contactGuid);
        IEnumerable<IContactState> GetContactsByProjectRoleGuid(Guid companyRoleGuid);
        void DeleteProjectRoleAssignmentState(Guid entityGuid);
        IEnumerable<IProjectRoleAssignmentState> GetProjectRoleAssignmentStates();
        void PersistChanges();
    }
    public class ProjectRoleAssignment : IProjectRoleAssignment
    {
        IProjectRoleAssignmentState _state;
        IProjectRoleAssignmentStateRepository _repo;

        public Guid Guid { get { return _state.Guid; } }
        public Guid ContactGuid { get { return _state.ContactGuid; } }
        public Guid ProjectRoleGuid { get { return _state.ProjectRoleGuid; } }
        public DateTime? StartDate { get { return _state.StartDate; } }
        public DateTime? EndDate { get { return _state.EndDate; } }
        public string ContactName { get { return _state.ContactName; } }

        public ProjectRoleAssignment(IProjectRoleAssignmentState state)
        {
            _state = state;
        }
        public ProjectRoleAssignment(IProjectRoleAssignmentState state, IProjectRoleAssignmentStateRepository repo) : this(state)
        {
            _repo = repo;
        }

        public void ChangeStartDate(DateTime? startDate, DateTime? originalStartDate)
        {
            if (_state.EndDate == originalStartDate)
            {
                _state.EndDate = startDate;
            }
        }

        public void ChangeEndDate(DateTime? endDate, DateTime? originalEndDate)
        {
            if (_state.EndDate == originalEndDate)
            {
                _state.EndDate = endDate;
            }
        }

        public void RenameContact(string name, string originalName)
        {
            if(_state.ContactName == originalName)
            {
                _state.ContactName = name;
            }
        }

        public void Terminate(DateTime endDate)
        {
            _state.EndDate = endDate;
        }
    }

    public interface IProjectRoleAssignmentService : ICommandProcessor
    {
        IProjectRoleAssignment CreateProjectRoleAssignment(Guid guid, Guid contactGuid, Guid companyGuid, DateTime? startDate, DateTime? endDate, string contactName);
        void DeleteProjectRoleAssignment(Guid entityGuid);
        IProjectRoleAssignment GetProjectRoleAssignment(Guid guid);
        void PersistChanges();
    }

    public class ProjectRoleAssignmentService : IProjectRoleAssignmentService
    {
        private IProjectRoleAssignmentStateRepository _repo;
        public ProjectRoleAssignmentService(IProjectRoleAssignmentStateRepository repo)
        {
            _repo = repo;
        }
        public IProjectRoleAssignment CreateProjectRoleAssignment(Guid guid, Guid contactGuid, Guid companyRoleGuid, DateTime? startDate, DateTime? endDate, string contactName)
        {
            var state = _repo.CreateProjectRoleAssignmentState(guid, contactGuid, companyRoleGuid);
            state.StartDate = startDate;
            state.EndDate = endDate;
            state.ContactName = contactName;
            return new ProjectRoleAssignment(state, _repo);
        }

        public void DeleteProjectRoleAssignment(Guid entityGuid)
        {
            _repo.DeleteProjectRoleAssignmentState(entityGuid);
        }

        public IProjectRoleAssignment GetProjectRoleAssignment(Guid guid)
        {
            var state = _repo.GetProjectRoleAssignmentState(guid);
            return new ProjectRoleAssignment(state, _repo);
        }
        public void PersistChanges()
        {
            _repo.PersistChanges();
        }
    }
}
