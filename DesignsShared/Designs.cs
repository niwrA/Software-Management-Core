using CommandsShared;
using DateTimeShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesignsShared
{
    public interface IDesignState : INamedEntityState
    {
        string Description { get; set; }
    }
    public interface IDesignStateRepository : IEntityRepository
    {
        IDesignState CreateDesignState(Guid guid, string name);
        IDesignState GetDesignState(Guid guid);
        IEnumerable<IDesignState> GetDesignStates();
        void DeleteDesignState(Guid guid);
    }
    public interface IDesign
    {
        DateTime CreatedOn { get; }
        Guid Guid { get; }
        string Name { get; }

        void Rename(string name, string original);
        void ChangeDescription(string description);
    }
    public class Design : IDesign
    {
        private IDesignState _state;
        private IDesignStateRepository _repo;

        public Design(IDesignState state, IDesignStateRepository repo) 
        {
            _state = state;
            _repo = repo;
        }

        public Guid Guid { get { return _state.Guid; } }
        public string Name { get { return _state.Name; } }
        public string Description { get { return _state.Description; } }
        public DateTime CreatedOn { get { return _state.CreatedOn; } }


        public void Rename(string name, string originalName)
        {
            if (_state.Name == originalName)
            {
                _state.Name = name;
            }
            else
            {
                // todo: concurrency policy implementation
            }
        }
        // todo: rework to textfragment?
        public void ChangeDescription(string description)
        {
            _state.Description = description;
        }
    }

    public interface IDesignService : ICommandProcessor
    {
        IDesign CreateDesign(Guid guid, string name);
        IDesign GetDesign(Guid guid);
        void DeleteDesign(Guid entityGuid);
        void PersistChanges();
    }
    public class DesignService : IDesignService
    {
        private IDateTimeProvider _dateTimeProvider;
        private IDesignStateRepository _repo;
        public DesignService(IDesignStateRepository repo, IDateTimeProvider dateTimeProvider)
        {
            _repo = repo;
            _dateTimeProvider = dateTimeProvider;
        }
        public IDesign CreateDesign(Guid guid, string name)
        {
            var state = _repo.CreateDesignState(guid, name);
            state.Name = name;
            state.CreatedOn = _dateTimeProvider.GetUtcDateTime();
            state.UpdatedOn = _dateTimeProvider.GetUtcDateTime();
            return new Design(state, _repo) as IDesign;
        }
        public IDesign GetDesign(Guid guid)
        {
            var state = _repo.GetDesignState(guid);
            return new Design(state, _repo) as IDesign;
        }
        public void DeleteDesign(Guid guid)
        {
            _repo.DeleteDesignState(guid);
        }

        public void PersistChanges()
        {
            _repo.PersistChanges();
        }
    }
    public class DesignBuilder
    {
        private DesignService _designs;
        private Guid _guid;
        private string _name;

        public DesignBuilder(DesignService designs)
        {
            _designs = designs;
        }

        public IDesign Build()
        {
            EnsureGuid();
            var design = _designs.CreateDesign(_guid, _name);
            return design;
        }

        private void EnsureGuid()
        {
            if (_guid == null || _guid == Guid.Empty)
            {
                _guid = Guid.NewGuid();
            }
        }

        public DesignBuilder WithGuid(Guid guid)
        {
            _guid = guid;
            return this;
        }

        public DesignBuilder WithName(string name)
        {
            _name = name;
            return this;
        }
    }
}
