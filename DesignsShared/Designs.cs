using CommandsShared;
using DateTimeShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesignsShared
{
  public interface IDesignState : INamedEntityState
  {
    string Description { get; set; }
    ICollection<IEpicElementState> EpicElementStates { get; set; }

  }
  public interface IDesignElementState : INamedEntityState
  {
    string Description { get; set; }
    Guid DesignGuid { get; set; }
  }
  public interface IEpicElementState : IDesignElementState
  {
    ICollection<IEntityElementState> EntityElementStates { get; set; }
  }
  public interface IEntityElementState : IDesignElementState
  {
    ICollection<IPropertyElementState> PropertyElementStates { get; set; }
    ICollection<ICommandElementState> CommandElementStates { get; set; }
    Guid EpicElementGuid { get; set; }
    Guid? ParentGuid { get; set; }
  }
  public interface IPropertyElementState : IDesignElementState
  {
    Guid EpicElementGuid { get; set; }
    Guid EntityElementGuid { get; set; }
  }
  public interface ICommandElementState : IDesignElementState
  {
    Guid EpicElementGuid { get; set; }
    Guid EntityElementGuid { get; set; }
  }
  public interface IQueryElementState : IDesignElementState
  {

  }

  public interface IDesignStateRepository : IEntityRepository
  {
    IDesignState CreateDesignState(Guid guid, string name);
    IEpicElementState CreateEpicElementState(Guid designGuid, Guid guid, string name);
    //IDesignState CreateEntityElementState(Guid parentGuid, Guid guid, string name);
    //IDesignState CreatePropertyElementState(Guid parentGuid, Guid guid, string name);
    //IDesignState CreateCommandElementState(Guid parentGuid, Guid guid, string name);
    IDesignState GetDesignState(Guid guid);
    IEnumerable<IDesignState> GetDesignStates();
    void DeleteDesignState(Guid guid);
    IEntityElementState CreateEntityElementState(Guid designGuid, Guid epicGuid, Guid entityGuid, string name, Guid? parentGuid);
    IPropertyElementState CreatePropertyElementState(Guid designGuid, Guid epicGuid, Guid entityGuid, Guid guid, string name);
    ICommandElementState CreateCommandElementState(Guid designGuid, Guid epicGuid, Guid entityGuid, Guid guid, string name);
  }
  public interface IElement
  {
    Guid Guid { get; }
    string Name { get; }
    Guid DesignGuid { get; }
    void Rename(string name, string originalName);
    void ChangeDescription(string description);
  }
  public interface IPropertyElement : IElement
  {
    Guid EpicGuid { get; }
    Guid EntityGuid { get; }
  }
  public interface ICommandElement : IElement
  {
    Guid EpicGuid { get; }
    Guid EntityGuid { get; }
  }
  public interface IEntityElement : IElement
  {
    Guid EpicGuid { get; }
    ICommandElement AddCommandElement(Guid guid, string name);
    IPropertyElement AddPropertyElement(Guid guid, string name);
    IPropertyElement GetPropertyElement(Guid entityGuid);
    void DeleteCommandElement(Guid entityGuid);
    void DeletePropertyElement(Guid entityGuid);
    ICommandElement GetCommandElement(Guid entityGuid);
  }
  public interface IEpicElement : IElement
  {
    IEntityElement AddEntityElement(Guid entityGuid, string name, Guid? parentGuid);
    IEntityElement GetEntityElement(Guid entityGuid);
    void DeleteEntityElement(Guid entityGuid);
  }
  public interface IDesign
  {
    DateTime CreatedOn { get; }
    Guid Guid { get; }
    string Name { get; }
    void Rename(string name, string original);
    void ChangeDescription(string description);
    IEpicElement AddEpicElement(Guid guid, string name);
    void DeleteEpicElement(Guid entityGuid);
    IEpicElement GetEpicElement(Guid entityGuid);
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

    public IEpicElement AddEpicElement(Guid guid, string name)
    {
      var state = _repo.CreateEpicElementState(this.Guid, guid, name);
      state.DesignGuid = this.Guid;
      return new EpicElement(state, _repo);
    }

    public void DeleteEpicElement(Guid entityGuid)
    {
      throw new NotImplementedException();
    }

    public IEpicElement GetEpicElement(Guid entityGuid)
    {
      var state = _state.EpicElementStates.Single(s => s.Guid == entityGuid);
      return new EpicElement(state, _repo);
    }
  }
  public class ElementBase : IElement
  {
    private IDesignElementState _state;
    public ElementBase(IDesignElementState state)
    {
      _state = state;

    }
    public Guid DesignGuid { get { return _state.DesignGuid; } }
    public Guid Guid { get { return _state.Guid; } }
    public string Name { get { return _state.Name; } }
    public DateTime CreatedOn { get { return _state.CreatedOn; } }
    public DateTime UpdatedOn { get { return _state.CreatedOn; } }
    public void ChangeDescription(string description)
    {
      _state.Description = description;
    }

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

  }
  public class EpicElement : ElementBase, IEpicElement
  {
    private IEpicElementState _state;
    private IDesignStateRepository _repo;

    public EpicElement(IEpicElementState state, IDesignStateRepository repo) : base(state)
    {
      _state = state;
      _repo = repo;
    }

    public IEntityElement AddEntityElement(Guid guid, string name, Guid? parentGuid)
    {
      var state = _repo.CreateEntityElementState(DesignGuid, Guid, guid, name, parentGuid);
      var entityElement = new EntityElement(state, _repo);
      return entityElement;
    }

    // todo: this works for MongoDb but should be hidden in the repository
    public void DeleteEntityElement(Guid entityGuid)
    {
      var state = _state.EntityElementStates.Single(s => s.Guid == entityGuid);
      _state.EntityElementStates.Remove(state);
    }

    public IEntityElement GetEntityElement(Guid entityGuid)
    {
      var state = _state.EntityElementStates.Single(s => s.Guid == entityGuid);
      return new EntityElement(state, _repo);
    }

  }

  public class EntityElement : ElementBase, IEntityElement
  {
    private IEntityElementState _state;
    private IDesignStateRepository _repo;
    public Guid EpicGuid { get { return _state.EpicElementGuid; } }

    public EntityElement(IEntityElementState state, IDesignStateRepository repo) : base(state)
    {
      _state = state;
      _repo = repo;
    }

    public IPropertyElement AddPropertyElement(Guid guid, string name)
    {
      var state = _repo.CreatePropertyElementState(DesignGuid, EpicGuid, Guid, guid, name);
      var propertyElement = new PropertyElement(state, _repo);
      return propertyElement;
    }
    public ICommandElement AddCommandElement(Guid guid, string name)
    {
      var state = _repo.CreateCommandElementState(DesignGuid, EpicGuid, Guid, guid, name);
      var commandElement = new CommandElement(state, _repo);
      return commandElement;
    }

    public IPropertyElement GetPropertyElement(Guid entityGuid)
    {
      var state = _state.PropertyElementStates.Single(s => s.Guid == entityGuid);
      return new PropertyElement(state, _repo);
    }

    public void DeletePropertyElement(Guid entityGuid)
    {
      var state = _state.PropertyElementStates.Single(s => s.Guid == entityGuid);
      _state.PropertyElementStates.Remove(state);
    }

    public ICommandElement GetCommandElement(Guid entityGuid)
    {
      var state = _state.CommandElementStates.Single(s => s.Guid == entityGuid);
      return new CommandElement(state, _repo);
    }

    public void DeleteCommandElement(Guid entityGuid)
    {
      var state = _state.CommandElementStates.Single(s => s.Guid == entityGuid);
      _state.CommandElementStates.Remove(state);
    }
  }
  public class PropertyElement : ElementBase, IPropertyElement
  {
    private IPropertyElementState _state;
    private IDesignStateRepository _repo;
    public Guid EpicGuid { get { return _state.EpicElementGuid; } }
    public Guid EntityGuid { get { return _state.EntityElementGuid; } }

    public PropertyElement(IPropertyElementState state, IDesignStateRepository repo) : base(state)
    {
      _state = state;
      _repo = repo;
    }
  }
  public class CommandElement : ElementBase, ICommandElement
  {
    private ICommandElementState _state;
    private IDesignStateRepository _repo;
    public Guid EpicGuid { get { return _state.EpicElementGuid; } }
    public Guid EntityGuid { get { return _state.EntityElementGuid; } }

    public CommandElement(ICommandElementState state, IDesignStateRepository repo) : base(state)
    {
      _state = state;
      _repo = repo;
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
