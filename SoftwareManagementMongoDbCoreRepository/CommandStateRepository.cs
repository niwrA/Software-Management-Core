﻿using niwrA.CommandManager;
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
using niwrA.CommandManager.Contracts;

namespace SoftwareManagementMongoDbCoreRepository
{


    [BsonIgnoreExtraElements]
    public class TimeStampedEntityState : ITimeStampedEntityState
    {
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Guid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class NamedEntityState : TimeStampedEntityState, ITimeStampedEntityState
    {
        public string Name { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class CommandState : TimeStampedEntityState, ICommandState
    {
        public string EntityGuid { get; set; }
        public string Entity { get; set; }
        public string EntityRootGuid { get; set; }
        public string EntityRoot { get; set; }
        public string Command { get; set; }
        public string CommandVersion { get; set; }
        public string ParametersJson { get; set; }
        public DateTime? ExecutedOn { get; set; }
        public DateTime? ReceivedOn { get; set; }
        public string UserName { get; set; }
        public string TenantId { get; set; }
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


        public ICommandState CreateCommandState(Guid guid)
        {
            var state = new CommandState()
            {
                Guid = guid
            };
            _commandStates.Add(state.Guid, state);
            return state;
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

        public IEnumerable<ICommandState> GetUnprocessedCommandStates()
        {
            var states = new List<ICommandState>();
            var collection = _database.GetCollection<CommandState>(CommandStatesCollection);
            var filter = Builders<CommandState>.Filter.Where(w => w.ExecutedOn == null);
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

        public IEnumerable<ICommandState> GetCommandStates()
        {
            var collection = _database.GetCollection<CommandState>(CommandStatesCollection);
            var filter = new BsonDocument();
            var states = collection.Find(filter);

            return states?.ToList();
        }

        public IEnumerable<ICommandState> GetCommandStates(string entityGuid)
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
    }
}
