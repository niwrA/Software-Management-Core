using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ProductsShared;
using DateTimeShared;

namespace CommandsShared
{

  // define the interface for holding a command's state for instance for storage in a repository
  public interface ICommand
  {
    Guid Guid { get; set; }
    string Entity { get; set; }
    Guid EntityGuid { get; set; }
    string CommandTypeId { get; set; }
    string ParametersJson { get; set; }
    DateTime? ExecutedOn { get; set; }
    DateTime? ReceivedOn { get; set; }
    DateTime CreatedOn { get; set; }
    string UserName { get; set; }

    ICommandStateRepository CommandRepository { get; set; }
    ICommandProcessor CommandProcessor { get; set; }

    void Execute();
  }

  public interface ICommandState
  {
    Guid Guid { get; set; }
    Guid EntityGuid { get; set; }
    string CommandTypeId { get; set; }
    string ParametersJson { get; set; }
    DateTime? ExecutedOn { get; set; }
    DateTime? ReceivedOn { get; set; }
    DateTime CreatedOn { get; set; }
    string UserName { get; set; }
    string Entity { get; set; }
  }
  // defines the contract for a Command Repository implementation
  public interface ICommandStateRepository
  {
    void PersistChanges();
    ICommandState CreateCommandState();
    IEnumerable<ICommandState> GetCommandStates();
    IEnumerable<ICommandState> GetCommandStates(Guid entityGuid);
    IEnumerable<ICommandState> GetUnprocessedCommandStates();
  }

  public interface IEntityState
  {
    Guid Guid { get; set; }
    DateTime CreatedOn { get; set; }
    DateTime UpdatedOn { get; set; }
  }
  public interface INamedEntity
  {
    DateTime CreatedOn { get; }
    DateTime UpdatedOn { get; }
    Guid Guid { get; }
    string Name { get; }
  }

  public interface INamedEntityState : IEntityState
  {
    string Name { get; set; }
  }
  // defines the contract for an Entity Repository implementation
  public interface IEntityRepository
  {
    void PersistChanges();
    Task PersistChangesAsync();
  }
  // defines the contract for entities compatible with commanding
  public interface ICommandableEntity
  {
    Guid Guid { get; }
  }
  // contains the shared logic for all commands
  public class CommandBase : ICommand
  {
    private ICommandState _state;
    private ICommandStateRepository _repository;
    public CommandBase()
    {
    }
    public CommandBase(ICommandStateRepository repo) : this()
    {
      _repository = repo;
      InitState();
    }

    public CommandBase(ICommandStateRepository repo, ICommandState state) : this(repo)
    {
      this._state = state;
    }
    public DateTime CreatedOn { get { return _state.CreatedOn; } set { _state.CreatedOn = value; } }
    public DateTime? ReceivedOn { get { return _state.ReceivedOn; } set { _state.ReceivedOn = value; } }
    public string UserName { get { return _state.UserName; } set { _state.UserName = value; } }

    private void InitState()
    {
      if (_state == null && _repository != null)
      {
        this._state = _repository.CreateCommandState();
      }
      if (_state != null)
      {
        this._state.CommandTypeId = this.GetType().Name;
        if (_state.Guid == null || _state.Guid == Guid.Empty)
        {
          _state.Guid = Guid.NewGuid();
        }
      }
    }

    public virtual string ParametersJson
    {
      get
      {
        return _state.ParametersJson;
      }
      set
      {
        _state.ParametersJson = value;
      }
    }
    public Guid Guid { get { return _state.Guid; } set { _state.Guid = value; } }
    public string Entity { get { return _state.Entity; } set { _state.Entity = value; } }
    public Guid EntityGuid { get { return _state.EntityGuid; } set { _state.EntityGuid = value; } }
    public virtual void Execute() { }
    public DateTime? ExecutedOn { get { return _state.ExecutedOn; } set { _state.ExecutedOn = value; } }

    public string CommandTypeId { get { return _state.CommandTypeId; } set { _state.CommandTypeId = value; } }
    public ICommandState State { get { return _state; } set { _state = value; } }

    public ICommandStateRepository CommandRepository { get { return _repository; } set { _repository = value; InitState(); } }

    private ICommandProcessor _commandProcessor;
    public virtual ICommandProcessor CommandProcessor { get { return _commandProcessor; } set { _commandProcessor = value; } }

  }
  // the domain class for Commands
  public interface ICommandProcessor
  {
  }
  public interface ICommandConfig
  {
    string Key { get; }
    string Name { get; set; }
    ICommand GetCommand(string parametersJson);
    ICommandProcessor Processor { get; set; }
    string ProcessorName { get; set; }
    string NameSpace { get; set; }
    string Assembly { get; set; }
  }

  public interface IProcessorConfig
  {
    ICommandProcessor Processor { get; set; }
    string Entity { get; set; }
    string NameSpace { get; set; }
    string Assembly { get; set; }
    ICommand GetCommand(string name, string entity, string parametersJson);

  }

  public interface ICommandService
  {
    void AddConfig(ICommandConfig config);
    void AddConfig(IProcessorConfig config);
    ICommand ProcessCommand(CommandDto command);
    ICommand ProcessCommand(ICommand command);
    ICommand ProcessCommand(CommandDto command, ICommandState state);
    void PersistChanges();
    void MergeCommands(IEnumerable<CommandDto> commands);
    ICommand CreateCommand<T>() where T : ICommand, new();
    IEnumerable<CommandDto> GetUnprocessedCommands();
  }

  public class ProcessorConfig : IProcessorConfig
  {
    public ICommandProcessor Processor { get; set; }
    public string Entity { get; set; }
    public string NameSpace { get; set; }
    public string Assembly { get; set; }
    public ICommand GetCommand(string name, string entity, string parametersJson)
    {
      var commandConfig = new CommandConfig()
      {
        ProcessorName = entity,
        Name = name,
        NameSpace = this.NameSpace,
        Assembly = this.Assembly,
        Processor = this.Processor
      };
      return commandConfig.GetCommand(parametersJson);
    }
  }
  public class CommandConfig : ICommandConfig
  {
    public string Key { get { return Name + ProcessorName + "Command"; } }
    public string Name { get; set; }
    public string ProcessorName { get; set; }
    public string NameSpace { get; set; }
    public string Assembly { get; set; }
    public ICommandProcessor Processor { get; set; }
    public ICommand GetCommand(string json)
    {
      ICommand command;
      Type type = Type.GetType(NameSpace + "." + Key + ", " + Assembly);
      if (type == null)
      {
        throw new TypeNotFoundException($"{Key} not found in {NameSpace} of {Assembly}");
      }

      if (!string.IsNullOrWhiteSpace(json))
      {
        var insert = @"'$type': '" + NameSpace + "." + Key + @", " + Assembly + "', ";
        json = json.Trim().Insert(1, insert);
        command = JsonConvert.DeserializeObject<ICommand>(json, new JsonSerializerSettings
        {
          TypeNameHandling = TypeNameHandling.Auto
        });
      }
      else
      {
        command = Activator.CreateInstance(type) as ICommand;
      }
      return command;
    }
  }

  // todo: move out CommandDto c.s. to separate project
  public class CommandDto
  {
    private ICommandState _state;
    public CommandDto()
    {
    }
    public CommandDto(ICommandState state)
    {
      this.Guid = state.Guid;
      this.EntityGuid = state.EntityGuid;
      this.Entity = state.Entity; // todo: add to state and store in database
      this.ExecutedOn = state.ExecutedOn;
      this.Name = state.CommandTypeId?.Replace(state.Entity + "Command", ""); // we already have the proper name, so perhaps this can be done more cleanly,
      this.Username = state.UserName;
      // or we should save the CommandTypeId differently into the CommandState Table, ie. without EntityCommand suffix
      this.ParametersJson = state.ParametersJson;
      _state = state;
    }
    // todo: this is perhaps a bit ugly
    public ICommandState State { get { return _state; } }

    public Guid Guid { get; set; }
    public Guid EntityGuid { get; set; }
    public string Entity { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string ParametersJson { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ExecutedOn { get; set; }
    public DateTime? ReceivedOn { get; set; }

    //public string UserName { get; set; }
  }
  public class CommandService : ICommandService
  {
    private Dictionary<string, IProcessorConfig> _configs = new Dictionary<string, IProcessorConfig>();
    private Dictionary<string, ICommandConfig> _commandConfigs = new Dictionary<string, ICommandConfig>();
    private ICommandStateRepository _repo;
    private IDateTimeProvider _dateTimeProvider;
    public CommandService(ICommandStateRepository repo, IDateTimeProvider dateTimeProvider)
    {
      _repo = repo;
      _dateTimeProvider = dateTimeProvider;
    }
    public void AddConfig(ICommandConfig config)
    {
      _commandConfigs.Add(config.Key, config);
    }
    public void AddConfig(IProcessorConfig config)
    {
      _configs.Add(config.Entity, config);
    }

    public void PersistChanges()
    {
      _repo.PersistChanges();
    }
    public void MergeCommands(IEnumerable<CommandDto> commands)
    {
      // todo: replay should be done once for each unique entity, or we need to refresh
      var processedEntities = new HashSet<Guid>();
      // the existing entity for every command
      foreach (var command in commands)
      {
        if (!processedEntities.Contains(command.EntityGuid))
        {
          // cheap solution. Should find a way to wrap the state here as well.
          var states = _repo.GetCommandStates(command.EntityGuid);//.Select(s => new CommandDto { Guid = s.Guid, Entity = command.Entity, EntityGuid = s.EntityGuid, Name = s.CommandTypeId.Replace(command.Entity + "Command", ""), ParametersJson = s.ParametersJson, CreatedOn = s.CreatedOn });
          foreach (var state in states)
          {
            // these will always be the same Entity
            ProcessCommand(command, state);
          }
          processedEntities.Add(command.EntityGuid);
        }
        ProcessCommand(command);
      }
    }
    public ICommand ProcessCommand(CommandDto command)
    {
      return ProcessCommand(command, null);
    }
    public ICommand ProcessCommand(ICommand command)
    {
      // todo: support command configs
      if (_configs.TryGetValue(command.CommandTypeId, out IProcessorConfig value))
      {
        command.CommandProcessor = value.Processor;
        command.Execute();
        command.ExecutedOn = _dateTimeProvider.GetUtcDateTime();
      }
      return command;
    }
    public ICommand ProcessCommand(CommandDto command, ICommandState state)
    {
      ICommand typedCommand = null;
      ICommandProcessor processor = null;
      // either take existing name from state, or construct from dto
      // the replace is a bit hacky, should probably clean the commandstore
      var commandName = state == null ? command.Name : state.CommandTypeId.Replace(command.Entity + "Command", "");
      var parametersJson = state == null ? command.ParametersJson : state.ParametersJson;
      if (_commandConfigs.TryGetValue(commandName + command.Entity + "Command", out ICommandConfig commandConfig))
      {
        typedCommand = commandConfig.GetCommand(parametersJson);
        processor = commandConfig.Processor;
      }
      else if (_configs.TryGetValue(command.Entity, out IProcessorConfig config))
      {
        typedCommand = config.GetCommand(commandName, command.Entity, parametersJson);
        processor = config.Processor;
      }
      if (typedCommand != null)
      {
        CopyCommandDtoIntoCommand(command, _repo, processor, typedCommand, state);

        typedCommand.Execute();
        typedCommand.ExecutedOn = _dateTimeProvider.GetUtcDateTime();

        return typedCommand;
      }

      throw new CommandNotConfiguredException($"The command named '{command.Name}' for entity '{command.Entity}' does not have a matching configuration.");
    }

    private void CopyCommandDtoIntoCommand(CommandDto command, ICommandStateRepository commandRepository, ICommandProcessor processor, ICommand typedCommand, ICommandState state)
    {
      ((CommandBase)typedCommand).State = state;
      typedCommand.CommandRepository = commandRepository;
      typedCommand.CreatedOn = command.CreatedOn;
      typedCommand.ReceivedOn = _dateTimeProvider.GetSessionUtcDateTime();
      typedCommand.Entity = command.Entity;
      typedCommand.EntityGuid = command.EntityGuid;
      typedCommand.Guid = command.Guid;
      typedCommand.ParametersJson = command.ParametersJson;
      typedCommand.CommandProcessor = processor;
    }

    public ICommand CreateCommand<T>() where T : ICommand, new()
    {
      var command = new T()
      {
        CommandRepository = _repo
      };
      command.Guid = Guid.NewGuid();
      command.CreatedOn = DateTime.Now;
      return command;
    }

    public IEnumerable<CommandDto> GetUnprocessedCommands()
    {
      var states = _repo.GetUnprocessedCommandStates();
      var dtos = new List<CommandDto>();
      foreach (var state in states)
      {
        dtos.Add(new CommandDto(state));
      }
      return dtos;
    }
  }

}
