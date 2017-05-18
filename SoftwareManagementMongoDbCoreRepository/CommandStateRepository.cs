using CommandsShared;
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
    public class EntityState : IEntityState
    {
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Guid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class NamedEntityState : EntityState, IEntityState
    {
        public string Name { get; set; }
    }


    [BsonIgnoreExtraElements]
    public class LinkState : NamedEntityState, ILinkState
    {
        public DateTime? BirthDate { get; set; }
        public string Url { get; set; }
        public Guid EntityGuid { get; set; }
        public Guid ForGuid { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string SiteName { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class CommandState : EntityState, ICommandState
    {
        public Guid EntityGuid { get; set; }
        public string CommandTypeId { get; set; }
        public string ParametersJson { get; set; }
        public DateTime? ExecutedOn { get; set; }
        public DateTime? ReceivedOn { get; set; }
        public string UserName { get; set; }
    }
    public class CommandStateRepository : ICommandStateRepository
    {
        private const string CommandStatesCollection = "CommandStates";
     
        private IMongoClient _client;
        private IMongoDatabase _database;

        private Dictionary<Guid, ICommandState> _commandStates { get; set; }

        public CommandStateRepository(IMongoClient client)
        {
            _client = client;
            _database = _client.GetDatabase("SoftwareManagement");

            _commandStates = new Dictionary<Guid, ICommandState>();
        }


        public ICommandState CreateCommandState()
        {
            var state = new CommandState()
            {
                Guid = Guid.NewGuid()
            };
            _commandStates.Add(state.Guid, state);
            return state;
        }

        // for consideration - include some part of this in both the link and company entity read projections?
        public IEnumerable<ICommandState> GetCommandStates(Guid entityGuid)
        {
            var states = new List<ICommandState>();
            var collection = _database.GetCollection<CommandState>(CommandStatesCollection);
            var filter = Builders<CommandState>.Filter.Eq("EntityGuid", entityGuid);
            var results = collection.Find(filter);
            if (results?.Count() > 0)
            {
                foreach (var result in results.ToList())
                {
                    states.Add(result);
                }
            }

            return states;
        }


        // todo: do we maybe want to store all link data so that we can get all that by companyGuid at once?
        // if so we would need to update both here and in links for linkupates
        
        public void PersistChanges()
        {
            PersistCommands();
        }

        private void PersistCommands()
        {
            if (_commandStates.Any())
            {
                var commandCollection = _database.GetCollection<CommandState>(CommandStatesCollection);
                var commands = _commandStates.Values.Select(s => s as CommandState).ToList();
                commandCollection.InsertMany(commands);
            }
        }

    }
}
