using CommandsShared;
using DateTimeShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectsShared
{
    public interface IProjectService : ICommandProcessor
    {
        Project CreateProject(Guid guid, string name);
        Project GetProject(Guid guid);
    }

    public interface IProjectState : IEntityState
    {
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
    }

    public interface IProjectStateRepository : IEntityRepository
    {
        IProjectState CreateProjectState(Guid guid);
        IProjectState GetProjectState(Guid guid);
        IEnumerable<IProjectState> GetProjectStates();
    }
    public class Project
    {
        private IProjectState _state;
        public Project(IProjectState state)
        {
            _state = state;
        }

        public Guid Guid { get { return _state.Guid; } }
        public string Name { get { return _state.Name; } }
        public DateTime CreatedOn { get { return _state.CreatedOn; } }

        public void Rename(string name)
        {
            _state.Name = name;
        }

        public void ChangeStartDate(DateTime? startDate)
        {
            _state.StartDate = startDate;
        }

        public void ChangeEndDate(DateTime? endDate)
        {
            _state.EndDate = endDate;
        }
    }
    public class ProjectService : IProjectService
    {
        private IDateTimeProvider _dateTimeProvider;
        private IProjectStateRepository _repo;
        public ProjectService(IProjectStateRepository repo, IDateTimeProvider dateTimeProvider)
        {
            _repo = repo;
            _dateTimeProvider = dateTimeProvider;
        }
        public Project CreateProject(Guid guid, string name)
        {
            var state = _repo.CreateProjectState(guid);
            state.CreatedOn = _dateTimeProvider.GetUtcDateTime();
            state.UpdatedOn = _dateTimeProvider.GetUtcDateTime();
            state.Name = name;
            return new Project(state);
        }
        public Project GetProject(Guid guid)
        {
            var state = _repo.GetProjectState(guid);
            return new Project(state);
        }
    }
    public class ProjectBuilder
    {
        private ProjectService _products;
        private Guid _guid;
        private string _name;

        public ProjectBuilder(ProjectService products)
        {
            _products = products;
        }

        public Project Build(string name)
        {
            EnsureGuid();
            var product = _products.CreateProject(_guid, name);
            return product;
        }

        private void EnsureGuid()
        {
            if (_guid == null || _guid == Guid.Empty)
            {
                _guid = Guid.NewGuid();
            }
        }

        public ProjectBuilder WithGuid(Guid guid)
        {
            _guid = guid;
            return this;
        }

        public ProjectBuilder WithName(string name)
        {
            _name = name;
            return this;
        }
    }
}
