using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CommandsShared
{

    // define the interface for holding a command's state for instance for storage in a repository
    public interface ICommandState
    {
        Guid Guid { get; set; }
        Guid EntityGuid { get; set; }
        string CommandTypeId { get; set; }
        string ParametersJson { get; set; }
        long? ExecutedOn { get; set; }
        long? ReceivedOn { get; set; }
        string UserName { get; set; }
    }
    // defines the contract for a Command Repository implementation
    public interface ICommandRepository
    {
        void PersistChanges();
        ICommandState Create();
        void Add(ICommandState state);
        void SetProcessed(ICommandState state);
        IEnumerable<ICommandState> GetAllProcessed();
        IEnumerable<ICommandState> GetAllNew();
        //IList<ICommandState> GetUpdates();
        //IList<ICommandState> GetCreates();
        IList<ICommandState> GetUpdatesSinceLast(long lastReceivedStamp);
        bool Exists(Guid guid);
    }
    public interface IEntityState
    {
        Guid Guid { get; set; }
        string Name { get; set; }
        DateTime Created { get; set; }
    }
    // defines the contract for an Entity Repository implementation
    public interface IEntityRepository
    {
        void PersistChanges();
        Task PersistChangesAsync();
    }
    // defines the contract for actual commands that will be executed against an entity and a specified repository
    public interface ICommand
    {
        Guid Guid { get; set; }
        Guid EntityGuid { get; set; }
        string CommandTypeId { get; set; }
        string ParametersJson { get; set; }
        DateTime? ExecutedOn { get; set; }
        ICommandRepository CommandRepository { get; set; }
        IEntityRepository EntityRepository { get; set; }
        void Execute();
        void Post();
        void SetState(ICommandState state);
    }
    // defines the contract for entities compatible with commanding
    public interface ICommandableEntity
    {
        Guid Guid { get; }
    }
    // contains the shared logic for all commands
    public class CommandBase : ICommand, INotifyPropertyChanged
    {
        private ICommandState _state;
        private ICommandRepository _repository;
        public CommandBase()
        {
        }

        private void InitState()
        {
            if (_state == null)
            {
                this._state = _repository.Create();
            }
            this._state.CommandTypeId = this.GetType().Name;
        }

        public CommandBase(ICommandRepository repo) : this()
        {
            _repository = repo;
            InitState();
        }

        public CommandBase(ICommandRepository repo, ICommandState state) : this(repo)
        {
            this._state = state;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        internal static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        internal static object DeserializeObject(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
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
        public Guid Guid
        {
            get { return _state.Guid; }
            set
            {
                _state.Guid = value; OnPropertyChanged();
            }
        }
        public Guid EntityGuid
        {
            get { return _state.EntityGuid; }
            set
            {
                _state.EntityGuid = value; OnPropertyChanged();
            }
        }
        public virtual void Execute() { _state.ExecutedOn = DateTime.UtcNow.Ticks; _repository.SetProcessed(_state); }
        private DateTime? _executedOn { get; set; }
        public DateTime? ExecutedOn
        {
            get { return _state.ExecutedOn.HasValue ? new DateTime(_state.ExecutedOn.Value) as DateTime? : null; }
            set
            {
                if (value == null)
                {
                    _state.ExecutedOn = null;
                }
                else
                {
                    _state.ExecutedOn = value.Value.Ticks;

                }
                OnPropertyChanged();
            }
        }

        public string CommandTypeId
        {
            get
            {
                return _state.CommandTypeId;
            }

            set
            {
                _state.CommandTypeId = value;
                OnPropertyChanged();
            }
        }
        private IEntityRepository _entityRepository;
        public virtual IEntityRepository EntityRepository
        {
            get { return _entityRepository; }
            set
            {
                _entityRepository = value;

            }
        }
        public ICommandRepository CommandRepository { get { return _repository; } set { _repository = value; InitState(); } }
        public ICommand CreateCommand<T>(ICommandRepository commandRepository, ICommandableEntity entity) where T : ICommand, new()
        {
            var createdCommand = new T()
            {
                CommandRepository = commandRepository
            };
            return createdCommand;
        }

        public void Post()
        {
            this.ParametersJson = this.Serialize();
            _repository.Add(this._state);
        }

        public void SetState(ICommandState state)
        {
            _state = state;
        }
    }
    // the domain class for Commands 
    public class Commands : ICommands
    {
        public class Injections
        {
            public ICommandRepository CommandRepository { get; set; }
        }

        private ICommandRepository _commandRepository;
        public ObservableCollection<ICommand> PostedCommands { get; set; }
        public Commands(ICommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
            PostedCommands = new ObservableCollection<ICommand>();
        }
        public ICommandRepository Repository { get { return _commandRepository; } }
        public void PostCommand(ICommand command)
        {
            command.Post();
            PostedCommands.Add(command);
        }
        //todo: get the commands from the repository and restore them properly using the CommandTypeId ... ?
        //this would only be possible on the Aggregate root or something like that? (needs references/dlls)
        public void ProcessCommands(IList<ICommand> commands)
        {
            foreach (var command in commands.Where(w => w.ExecutedOn == null))
            {
                command.Execute();
                PostedCommands.Add(command);
            }
            Repository.PersistChanges();
        }

        public IEnumerable<ICommand> GetProcessedCommands()
        {
            var list = new List<ICommand>();
            foreach (var state in _commandRepository.GetAllProcessed())
            {
                list.Add(new CommandBase(_commandRepository, state));
            }
            return list;
        }

    }
}
