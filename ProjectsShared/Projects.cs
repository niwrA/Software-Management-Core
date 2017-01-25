using CommandsShared;
using DateTimeShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectsShared
{
    public interface IProjectService : ICommandProcessor
    {
        IProject CreateProject(Guid guid, string name);
        IProject GetProject(Guid guid);
        void DeleteProject(Guid guid);
    }

    public interface IProjectState : IEntityState
    {
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
    }

    public interface IProjectStateRepository : IEntityRepository
    {
        IProjectState CreateProjectState(Guid guid, string name);
        IProjectState GetProjectState(Guid guid);
        IEnumerable<IProjectState> GetProjectStates();
        void DeleteProjectState(Guid guid);
    }
    public interface IProject
    {
        Guid Guid { get; }
        string Name { get; }
        DateTime CreatedOn { get; }
        void Rename(string name, string originalName);
        void ChangeStartDate(DateTime? startDate, DateTime? originalStartDate);
        void ChangeEndDate(DateTime? endDate, DateTime? originalEndDate);
    }
    public class Project : IProject
    {
        private IProjectState _state;
        public Project(IProjectState state)
        {
            _state = state;
        }

        public Guid Guid { get { return _state.Guid; } }
        public string Name { get { return _state.Name; } }
        public DateTime CreatedOn { get { return _state.CreatedOn; } }

        public void Rename(string name, string originalName)
        {
            if (_state.Name == originalName)
            {
                _state.Name = name;
            }
            else
            {
                // todo: implement concurrency policy
            }
        }

        // todo: implement concurrency policy
        public void ChangeStartDate(DateTime? startDate, DateTime? originalStartDate)
        {
            if(_state.StartDate == originalStartDate)
            {
                _state.StartDate = startDate;
            }
        }

        // todo: implement concurrency policy
        public void ChangeEndDate(DateTime? endDate, DateTime? originalEndDate)
        {
            if (_state.EndDate == originalEndDate)
            {
                _state.EndDate = endDate;
            }
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
        public IProject CreateProject(Guid guid, string name)
        {
            var state = _repo.CreateProjectState(guid, name);
            state.CreatedOn = _dateTimeProvider.GetUtcDateTime();
            state.UpdatedOn = _dateTimeProvider.GetUtcDateTime();
            state.Name = name;
            return new Project(state);
        }
        public IProject GetProject(Guid guid)
        {
            var state = _repo.GetProjectState(guid);
            return new Project(state);
        }
        public void DeleteProject(Guid guid)
        {
            _repo.DeleteProjectState(guid);
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

        public IProject Build(string name)
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
