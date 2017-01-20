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

namespace CommandsShared
{

    // define the interface for holding a command's state for instance for storage in a repository
    public interface ICommand
    {
        Guid Guid { get; set; }
        Guid EntityGuid { get; set; }
        string CommandTypeId { get; set; }
        string ParametersJson { get; set; }
        DateTime? ExecutedOn { get; set; }
        DateTime? ReceivedOn { get; set; }
        DateTime CreatedOn { get; set; }
        string UserName { get; set; }

        ICommandRepository CommandRepository { get; set; }
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
        DateTime CreatedOn { get; set; }
        DateTime UpdatedOn { get; set; }
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
    public class CommandBase : ICommand, INotifyPropertyChanged
    {
        private ICommandState _state;
        private ICommandRepository _repository;
        public CommandBase()
        {
        }
        public DateTime CreatedOn { get; set; }
        public DateTime? ReceivedOn { get; set; }
        public string UserName { get; set; }

        private void InitState()
        {
            if (_state == null && _repository != null)
            {
                this._state = _repository.Create();
                if (_state.Guid == null || _state.Guid == Guid.Empty)
                {
                    _state.Guid = Guid.NewGuid();
                }
            }
            if (_state != null)
            {
                this._state.CommandTypeId = this.GetType().Name;
            }
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

        // PropertyChanged only useful for UI, so maybe remove? 
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        // todo: serialization can go from here, moved to CommandManager
        //internal static T Deserialize<T>(string json)
        //{
        //    return JsonConvert.DeserializeObject<T>(json);
        //}
        //internal static object DeserializeObject(string json, Type type)
        //{
        //    return JsonConvert.DeserializeObject(json, type);
        //}
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
        public virtual void Execute() { _repository.SetProcessed(_state); }
        public DateTime? ExecutedOn
        {
            get { return _state.ExecutedOn;  }
            set
            {
                _state.ExecutedOn = value;
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
        public ICommandRepository CommandRepository { get { return _repository; } set { _repository = value; InitState(); } }

        private ICommandProcessor _commandProcessor;
        public virtual ICommandProcessor CommandProcessor { get { return _commandProcessor; } set { _commandProcessor = value; } }

        public ICommand CreateCommand<T>(ICommandRepository commandRepository, ICommandableEntity entity) where T : ICommand, new()
        {
            var createdCommand = new T()
            {
                CommandRepository = commandRepository
            };
            return createdCommand;
        }

        /// <summary>
        /// Serialize the command and add it's state to the repository.
        /// </summary>
        /// todo: this must not happen when the state is already in the repository - we would never update commands?
        /// also do we then still need to use DataContract to identify the proper members
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
            //command.Post();
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
                //list.Add(new CommandBase(_commandRepository, state));
            }
            return list;
        }

    }
}
