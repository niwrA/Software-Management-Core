using DesignsShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareManagementCoreApiTests.Fakes
{
  public class StateBase
  {
    public Guid Guid { get; set; } = Guid.NewGuid();
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public DateTime UpdatedOn { get; set; } = DateTime.Now;
  }

  public class PropertyElementState : StateBase, IPropertyElementState
  {
    public string Description { get; set; } = "Property element Description";
    public Guid DesignGuid { get; set; }
    public Guid EpicElementGuid { get; set; }
    public Guid EntityElementGuid { get; set; }
    public string Name { get; set; } = "Property element Name";
    public string DataType { get; set; }
  }
  public class CommandElementState : StateBase, ICommandElementState
  {
    public string Description { get; set; } = "Command element Description";
    public Guid DesignGuid { get; set; }
    public Guid EpicElementGuid { get; set; }
    public Guid EntityElementGuid { get; set; }
    public string Name { get; set; } = "Command element Name";
  }
  public class EntityElementState : StateBase, IEntityElementState
  {
    public EntityElementState()
    {
      this.PropertyElementStates = new List<IPropertyElementState>();
      var propertyState = new PropertyElementState();
      PropertyElementStates.Add(propertyState);
      this.CommandElementStates = new List<ICommandElementState>();
      var commandState = new CommandElementState();
      CommandElementStates.Add(commandState);
    }
    public string Description { get; set; } = "Entity element description";
    public Guid DesignGuid { get; set; }
    public Guid EpicElementGuid { get; set; }
    public string Name { get; set; } = "Entity element Name";
    public bool IsCollection { get; set; }
    public string PluralName { get; set; }
    public Guid? ParentGuid { get; set; }
    public ICollection<IPropertyElementState> PropertyElementStates { get; set; }
    public ICollection<ICommandElementState> CommandElementStates { get; set; }
  }
  public class EpicElementState : StateBase, IEpicElementState
  {
    public EpicElementState()
    {
      this.EntityElementStates = new List<IEntityElementState>();
      EntityElementState entityElementState = new EntityElementState();
      this.EntityElementStates.Add(entityElementState);
    }
    public ICollection<IEntityElementState> EntityElementStates { get; set; }
    public string Description { get; set; }
    public Guid DesignGuid { get; set; }
    public string Name { get; set; }
  }
  public class DesignState : StateBase, IDesignState
  {
    public DesignState()
    {
      Name = "fake design Name";
      Description = "fake design Description";
      EpicElementStates = new List<IEpicElementState>();
      var epicState = new EpicElementState();
      EpicElementStates.Add(epicState);
    }
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<IEpicElementState> EpicElementStates { get; set; }
  }
}