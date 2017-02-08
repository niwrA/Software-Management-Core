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
    }
    // defines the contract for a Command Repository implementation
    public interface ICommandStateRepository
    {
        void PersistChanges();
        ICommandState CreateCommandState();
        void AddCommandState(ICommandState state);
        void SetProcessed(ICommandState state);
        IEnumerable<ICommandState> GetAllProcessed();
        IEnumerable<ICommandState> GetAllNew();
        //IList<ICommandState> GetUpdates();
        //IList<ICommandState> GetCreates();
        IList<ICommandState> GetUpdatesSinceLast(long lastReceivedStamp);
        bool Exists(Guid guid);
        IEnumerable<ICommandState> GetCommandStates(Guid entityGuid);
    }
    public interface IEntityState
    {
        Guid Guid { get; set; }
        DateTime CreatedOn { get; set; }
        DateTime UpdatedOn { get; set; }
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
        public Guid Guid { get { return _state.Guid; } set { _state.Guid = value; } }
        public Guid EntityGuid { get { return _state.EntityGuid; } set { _state.EntityGuid = value; } }
        public virtual void Execute() { _repository.SetProcessed(_state); }
        public DateTime? ExecutedOn { get { return _state.ExecutedOn; } set { _state.ExecutedOn = value; } }

        public string CommandTypeId { get { return _state.CommandTypeId; } set { _state.CommandTypeId = value; } }
        public ICommandState State { get { return _state; } set { _state = value; } }

        public ICommandStateRepository CommandRepository { get { return _repository; } set { _repository = value; InitState(); } }

        private ICommandProcessor _commandProcessor;
        public virtual ICommandProcessor CommandProcessor { get { return _commandProcessor; } set { _commandProcessor = value; } }

        public ICommand CreateCommand<T>(ICommandStateRepository commandRepository, ICommandableEntity entity) where T : ICommand, new()
        {
            var createdCommand = new T()
            {
                CommandRepository = commandRepository
            };
            return createdCommand;
        }

        /// <summary>
        /// Serialize the command and add its state to the repository.
        /// </summary>
        /// todo: this must not happen when the state is already in the repository - we would never update commands on post
        /// also do we then still need to use DataContract to identify the proper members?
        public void Post()
        {
            this.ParametersJson = this.Serialize();
            _repository.AddCommandState(this._state);
        }

        public void SetState(ICommandState state)
        {
            _state = state;
        }
    }
    // the domain class for Commands 
    // todo: integrate with the Command Manager and cleanup old code
    public class Commands : ICommands
    {
        public class Injections
        {
            public ICommandStateRepository CommandRepository { get; set; }
        }

        private ICommandStateRepository _commandRepository;
        public ObservableCollection<ICommand> PostedCommands { get; set; }
        public Commands(ICommandStateRepository commandRepository)
        {
            _commandRepository = commandRepository;
            PostedCommands = new ObservableCollection<ICommand>();
        }
        public ICommandStateRepository Repository { get { return _commandRepository; } }
        public void PostCommand(ICommand command)
        {
            //command.Post();
            PostedCommands.Add(command);
        }
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
