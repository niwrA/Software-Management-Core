using LinksShared;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareManagementMongoDbCoreRepository
{
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

    public class LinkStateRepository: ILinkStateRepository
    {
        private const string LinkStatesCollection = "LinkStates";

        private IMongoClient _client;
        private IMongoDatabase _database;

        private Dictionary<Guid, ILinkState> _linkStates;
        private List<Guid> _deletedLinkStates;
        private Dictionary<Guid, ILinkState> _updatedLinkStates;

        public LinkStateRepository(IMongoClient client)
        {
            _client = client;
            _database = _client.GetDatabase("SoftwareManagement");


            _linkStates = new Dictionary<Guid, ILinkState>();
            _deletedLinkStates = new List<Guid>();
            _updatedLinkStates = new Dictionary<Guid, ILinkState>();
        }

        public ILinkState CreateLinkState(Guid guid, string name)
        {
            var state = new LinkState()
            {
                Guid = guid,
                Name = name
            };
            _linkStates.Add(state.Guid, state);
            return state;
        }

        public ILinkState GetLinkState(Guid guid)
        {
            if (!_linkStates.TryGetValue(guid, out ILinkState state))
            {
                if (!_updatedLinkStates.TryGetValue(guid, out state))
                {
                    var collection = _database.GetCollection<LinkState>(LinkStatesCollection);
                    var filter = Builders<LinkState>.Filter.Eq("Guid", guid);
                    state = collection.Find(filter).FirstOrDefault();

                    TrackLinkState(state);
                }
            }
            return state;
        }

        public IEnumerable<ILinkState> GetLinkStates()
        {
            var collection = _database.GetCollection<LinkState>(LinkStatesCollection);
            var filter = new BsonDocument();
            var states = collection.Find(filter);

            return states?.ToList();
        }

        public IEnumerable<ILinkState> GetLinkStatesForGuid(Guid forGuid)
        {
            var collection = _database.GetCollection<LinkState>(LinkStatesCollection);
            var filter = Builders<LinkState>.Filter.Eq("ForGuid", forGuid);
            var states = collection.Find(filter);

            return states?.ToList();
        }

        public void DeleteLinkState(Guid guid)
        {
            _deletedLinkStates.Add(guid);
        }


        private void TrackLinkState(ILinkState state)
        {
            if (state != null && !_updatedLinkStates.ContainsKey(state.Guid))
            {
                _updatedLinkStates.Add(state.Guid, state);
            }
        }
        private void PersistLinks()
        {
            var linkCollection = _database.GetCollection<LinkState>(LinkStatesCollection);
            // inserts
            if (_linkStates.Values.Any())
            {
                var links = _linkStates.Values.Select(s => s as LinkState).ToList();
                linkCollection.InsertMany(links);
                _linkStates.Clear();
            }

            // todo: can these be batched?
            // updates
            if (_updatedLinkStates.Values.Any())
            {
                var links = _updatedLinkStates.Values.Select(s => s as LinkState).ToList();
                foreach (var state in links)
                {
                    var filter = Builders<LinkState>.Filter.Eq("Guid", state.Guid);
                    linkCollection.ReplaceOne(filter, state);
                }
                _updatedLinkStates.Clear();
            }

            // deletes
            if (_deletedLinkStates.Any())
            {
                var collection = _database.GetCollection<LinkState>(LinkStatesCollection);
                foreach (var guid in _deletedLinkStates)
                {
                    var filter = Builders<LinkState>.Filter.Eq("Guid", guid);
                    collection.DeleteOne(filter);
                }
                _deletedLinkStates.Clear();
            }
        }

        public void PersistChanges()
        {

            PersistLinks();
        }

        public Task PersistChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
