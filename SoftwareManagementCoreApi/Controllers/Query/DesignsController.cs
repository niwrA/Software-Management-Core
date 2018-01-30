using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DesignsShared;

// For more information on enabling Web API for empty designs, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SoftwareManagementCoreApi.Controllers
{
  public class CommandElementDto
  {
    private ICommandElementState _state;
    public CommandElementDto(ICommandElementState state)
    {
      _state = state;
    }
    public Guid Guid { get { return _state.Guid; } }
    public Guid DesignGuid { get { return _state.DesignGuid; } }
    public Guid EpicElementGuid { get { return _state.EpicElementGuid; } }
    public Guid EntityElementGuid { get { return _state.EntityElementGuid; } }
    public string Name { get { return _state.Name; } }
    public string Description { get { return _state.Description; } }
    public string CreatedOn { get { return _state.CreatedOn.ToString("yyyy-MM-dd"); } }
    public string UpdatedOn { get { return _state.UpdatedOn.ToString("yyyy-MM-dd"); } }
  }
  public class PropertyElementDto
  {
    private IPropertyElementState _state;
    public PropertyElementDto(IPropertyElementState state)
    {
      _state = state;
    }
    public Guid Guid { get { return _state.Guid; } }
    public Guid DesignGuid { get { return _state.DesignGuid; } }
    public Guid EpicElementGuid { get { return _state.EpicElementGuid; } }
    public Guid EntityElementGuid { get { return _state.EntityElementGuid; } }
    public string Name { get { return _state.Name; } }
    public string Description { get { return _state.Description; } }
    public string DataType {  get { return _state.DataType; } }
    public string CreatedOn { get { return _state.CreatedOn.ToString("yyyy-MM-dd"); } }
    public string UpdatedOn { get { return _state.UpdatedOn.ToString("yyyy-MM-dd"); } }
  }
  public class EntityElementDto
  {
    private IEntityElementState _state;
    public EntityElementDto(IEntityElementState state)
    {
      _state = state;
      PropertyElements = state.PropertyElementStates?.Select(s => new PropertyElementDto(s)).ToList();
      CommandElements = state.CommandElementStates?.Select(s => new CommandElementDto(s)).ToList();
    }
    public Guid Guid { get { return _state.Guid; } }
    public Guid DesignGuid { get { return _state.DesignGuid; } }
    public Guid EpicElementGuid { get { return _state.EpicElementGuid; } }
    public string Name { get { return _state.Name; } }
    public Guid? ParentGuid { get { return _state.ParentGuid; } }
    public string Description { get { return _state.Description; } }
    public string CreatedOn { get { return _state.CreatedOn.ToString("yyyy-MM-dd"); } }
    public string UpdatedOn { get { return _state.UpdatedOn.ToString("yyyy-MM-dd"); } }
    public bool IsCollection { get { return _state.IsCollection; } }
    public string PluralName { get { return _state.PluralName; } }
    public ICollection<CommandElementDto> CommandElements { get; set; }
    public ICollection<PropertyElementDto> PropertyElements { get; set; }
  }

  public class EpicElementDto
  {
    private IEpicElementState _state;
    public EpicElementDto(IEpicElementState state)
    {
      _state = state;
      EntityElements = state.EntityElementStates?.Select(s => new EntityElementDto(s)).ToList();
    }
    public Guid Guid { get { return _state.Guid; } }
    public Guid DesignGuid { get { return _state.DesignGuid; } }
    public string Name { get { return _state.Name; } }
    public string Description { get { return _state.Description; } }
    public string CreatedOn { get { return _state.CreatedOn.ToString("yyyy-MM-dd"); } }
    public string UpdatedOn { get { return _state.UpdatedOn.ToString("yyyy-MM-dd"); } }

    public ICollection<EntityElementDto> EntityElements { get; set; }
  }

  public class DesignDto
  {
    private IDesignState _state;
    public DesignDto(IDesignState state)
    {
      _state = state;
      EpicElements = state.EpicElementStates?.Select(s => new EpicElementDto(s)).ToList();
    }
    public Guid Guid { get { return _state.Guid; } }
    public string Name { get { return _state.Name; } }
    public string Description { get { return _state.Description; } }
    public string CreatedOn { get { return _state.CreatedOn.ToString("yyyy-MM-dd"); } }
    public string UpdatedOn { get { return _state.UpdatedOn.ToString("yyyy-MM-dd"); } }

    public ICollection<EpicElementDto> EpicElements { get; set; }
  }

  [Route("api/[controller]")]
  public class DesignsController : Controller
  {
    private IDesignStateRepository _designStateRepository;

    public DesignsController(IDesignStateRepository designStateRepository)
    {
      _designStateRepository = designStateRepository;
    }
    // GET: api/designs
    [HttpGet]
    public IEnumerable<DesignDto> Get()
    {
      var states = _designStateRepository.GetDesignStates();
      var dtos = states.Select(s => new DesignDto(s)).ToList();
      return dtos;
    }

    // GET api/designs/5
    [HttpGet("{guid}")]
    public DesignDto Get(Guid guid)
    {
      var state = _designStateRepository.GetDesignState(guid);
      if (state != null)
      {
        var dto = new DesignDto(state);
        return dto;
      }
      return null;
    }
  }
}
