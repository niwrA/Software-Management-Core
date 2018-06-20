using niwrA.CommandManager;
using CompaniesShared;
using ContactsShared;
using DesignsShared;
using EmploymentsShared;
using LinksShared;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using ProductsShared;
using ProjectRoleAssignmentsShared;
using ProjectsShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoftwareManagementMongoDbCoreRepository
{
    [BsonIgnoreExtraElements]
    public class DesignState : IDesignState
    {

        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Guid { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public ICollection<IEpicElementState> EpicElementStates { get; set; } = new List<IEpicElementState>() as ICollection<IEpicElementState>;
    }
    [BsonIgnoreExtraElements]
    public class EpicElementState : IEpicElementState
    {
        public EpicElementState()
        {
            EntityElementStates = new List<IEntityElementState>() as ICollection<IEntityElementState>;
        }
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Guid { get; set; }
        public Guid DesignGuid { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public ICollection<IEntityElementState> EntityElementStates { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class EntityElementState : IEntityElementState
    {
        public EntityElementState()
        {
            PropertyElementStates = new List<IPropertyElementState>();
            CommandElementStates = new List<ICommandElementState>();
        }
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Guid { get; set; }
        public Guid DesignGuid { get; set; }
        public Guid EpicElementGuid { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string PluralName { get; set; }
        public Guid? ParentGuid { get; set; }
        public bool IsCollection { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public ICollection<ICommandElementState> CommandElementStates { get; set; }
        public ICollection<IPropertyElementState> PropertyElementStates { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class CommandElementState : ICommandElementState
    {
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Guid { get; set; }
        public Guid DesignGuid { get; set; }
        public Guid EpicElementGuid { get; set; }
        public Guid EntityElementGuid { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class PropertyElementState : IPropertyElementState
    {
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Guid { get; set; }
        public Guid DesignGuid { get; set; }
        public Guid EpicElementGuid { get; set; }
        public Guid EntityElementGuid { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string DataType { get; set; }
        public bool IsState { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    public class DesignStateRepository : IDesignStateRepository
    {
        private const string DesignStatesCollection = "DesignStates";

        private IMongoClient _client;
        private IMongoDatabase _database;

        private Dictionary<Guid, IDesignState> _designStates;
        private List<Guid> _deletedDesignStates;
        private Dictionary<Guid, IDesignState> _updatedDesignStates;
        public DesignStateRepository(IMongoClient client)
        {
            _client = client;
            _database = _client.GetDatabase("SoftwareManagement");

            _designStates = new Dictionary<Guid, IDesignState>();
            _deletedDesignStates = new List<Guid>();
            _updatedDesignStates = new Dictionary<Guid, IDesignState>();
        }

        private void TrackDesignState(IDesignState state)
        {
            if (state != null && !_updatedDesignStates.ContainsKey(state.Guid))
            {
                _updatedDesignStates.Add(state.Guid, state);
            }
        }

        public void PersistChanges()
        {
            PersistDesigns();
        }

        private void PersistDesigns()
        {
            var designCollection = _database.GetCollection<DesignState>(DesignStatesCollection);
            // inserts
            if (_designStates.Values.Any())
            {
                var designs = _designStates.Values.Select(s => s as DesignState).ToList();
                designCollection.InsertMany(designs);
                _designStates.Clear();
            }

            // todo: can these be batched?
            // updates
            if (_updatedDesignStates.Values.Any())
            {
                var designs = _updatedDesignStates.Values.Select(s => s as DesignState).ToList();
                foreach (var state in designs)
                {
                    var filter = Builders<DesignState>.Filter.Eq("Guid", state.Guid);
                    designCollection.ReplaceOne(filter, state);
                }
                _updatedDesignStates.Clear();
            }

            // deletes
            if (_deletedDesignStates.Any())
            {
                var collection = _database.GetCollection<DesignState>(DesignStatesCollection);
                foreach (var guid in _deletedDesignStates)
                {
                    var filter = Builders<DesignState>.Filter.Eq("Guid", guid);
                    collection.DeleteOne(filter);
                }
                _deletedDesignStates.Clear();
            }
        }
        public Task PersistChangesAsync()
        {
            throw new NotImplementedException();
        }
        // todo: repository tests
        public IDesignState CreateDesignState(Guid guid, string name)
        {
            var state = new DesignState()
            {
                Guid = guid,
                Name = name
            };
            _designStates.Add(state.Guid, state);
            return state;
        }

        public IDesignState GetDesignState(Guid guid)
        {
            if (!_designStates.TryGetValue(guid, out IDesignState state))
            {
                if (!_updatedDesignStates.TryGetValue(guid, out state))
                {

                    var collection = _database.GetCollection<DesignState>(DesignStatesCollection);
                    var filter = Builders<DesignState>.Filter.Eq("Guid", guid);

                    state = collection.Find(filter).FirstOrDefault();

                    TrackDesignState(state);
                }
            }
            return state;
        }

        public IEnumerable<IDesignState> GetDesignStates()
        {
            var collection = _database.GetCollection<DesignState>(DesignStatesCollection);
            var filter = new BsonDocument();
            var states = collection.Find(filter);

            return states?.ToList();
        }

        public void DeleteDesignState(Guid guid)
        {
            _deletedDesignStates.Add(guid);
        }

        public IEpicElementState CreateEpicElementState(Guid designGuid, Guid guid, string name)
        {
            var state = GetDesignState(designGuid);

            var epicElementState = new EpicElementState()
            {
                DesignGuid = designGuid,
                Guid = guid,
                Name = name
            };
            state.EpicElementStates.Add(epicElementState);
            return epicElementState;
        }

        public IEntityElementState CreateEntityElementState(Guid designGuid, Guid epicGuid, Guid guid, string name, Guid? parentGuid)
        {
            var state = GetDesignState(designGuid);
            var epic = state.EpicElementStates.Single(s => s.Guid == epicGuid);

            var entityElementState = new EntityElementState()
            {
                DesignGuid = designGuid,
                EpicElementGuid = epicGuid,
                Guid = guid,
                Name = name,
                ParentGuid = parentGuid
            };
            epic.EntityElementStates.Add(entityElementState);
            return entityElementState;
        }

        public IPropertyElementState CreatePropertyElementState(Guid designGuid, Guid epicGuid, Guid entityGuid, Guid guid, string name)
        {
            var state = GetDesignState(designGuid);
            var epic = state.EpicElementStates.Single(s => s.Guid == epicGuid);
            var entity = epic.EntityElementStates.Single(s => s.Guid == entityGuid);

            var propertyElementState = new PropertyElementState()
            {
                DesignGuid = designGuid,
                EpicElementGuid = epicGuid,
                EntityElementGuid = entityGuid,
                Guid = guid,
                Name = name
            };
            entity.PropertyElementStates.Add(propertyElementState);
            return propertyElementState;
        }

        public ICommandElementState CreateCommandElementState(Guid designGuid, Guid epicGuid, Guid entityGuid, Guid guid, string name)
        {
            var state = GetDesignState(designGuid);
            var epic = state.EpicElementStates.Single(s => s.Guid == epicGuid);
            var entity = epic.EntityElementStates.Single(s => s.Guid == entityGuid);

            var commandElementState = new CommandElementState()
            {
                DesignGuid = designGuid,
                EpicElementGuid = epicGuid,
                EntityElementGuid = entityGuid,
                Guid = guid,
                Name = name
            };
            entity.CommandElementStates.Add(commandElementState);
            return commandElementState;
        }

    }
}
