using ContactsShared;
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
    public class ContactState : NamedEntityState, IContactState
    {
        public DateTime? BirthDate { get; set; }
        public string Email { get; set; }
        public Guid? AvatarFileGuid { get; set; }
        public string AvatarUrl { get; set; }
    }
    public class ContactStateRepository: IContactStateRepository
    {
        private const string ContactStatesCollection = "ContactStates";

        private Dictionary<Guid, IContactState> _contactStates;
        private List<Guid> _deletedContactStates;
        private Dictionary<Guid, IContactState> _updatedContactStates;
        private IMongoClient _client;
        private IMongoDatabase _database;

        public ContactStateRepository(IMongoClient client)
        {
            _client = client;
            _database = _client.GetDatabase("SoftwareManagement");

            _contactStates = new Dictionary<Guid, IContactState>();
            _deletedContactStates = new List<Guid>();
            _updatedContactStates = new Dictionary<Guid, IContactState>();
        }

        public IContactState CreateContactState(Guid guid, string name)
        {
            var state = new ContactState()
            {
                Guid = guid
            };
            _contactStates.Add(state.Guid, state);
            return state;
        }

        public void DeleteContactState(Guid guid)
        {
            _deletedContactStates.Add(guid);
        }


        public IContactState GetContactState(Guid guid)
        {
            if (!_contactStates.TryGetValue(guid, out IContactState state))
            {
                if (!_updatedContactStates.TryGetValue(guid, out state))
                {
                    var collection = _database.GetCollection<ContactState>(ContactStatesCollection);
                    var filter = Builders<ContactState>.Filter.Eq("Guid", guid);
                    state = collection.Find(filter).FirstOrDefault();

                    TrackContactState(state);
                }
            }
            return state;
        }

        public IEnumerable<IContactState> GetContactStates()
        {
            var collection = _database.GetCollection<ContactState>(ContactStatesCollection);
            var filter = new BsonDocument();
            var states = collection.Find(filter);

            return states?.ToList();
        }

        private void TrackContactState(IContactState state)
        {
            if (state != null && !_updatedContactStates.ContainsKey(state.Guid))
            {
                _updatedContactStates.Add(state.Guid, state);
            }
        }


        public void PersistChanges()
        {
            PersistContacts();
        }

        private void PersistContacts()
        {
            var contactCollection = _database.GetCollection<ContactState>(ContactStatesCollection);
            // inserts
            if (_contactStates.Values.Any())
            {
                var contacts = _contactStates.Values.Select(s => s as ContactState).ToList();
                contactCollection.InsertMany(contacts);
                _contactStates.Clear();
            }

            // todo: can these be batched?
            // updates
            if (_updatedContactStates.Values.Any())
            {
                var contacts = _updatedContactStates.Values.Select(s => s as ContactState).ToList();
                foreach (var state in contacts)
                {
                    var filter = Builders<ContactState>.Filter.Eq("Guid", state.Guid);
                    contactCollection.ReplaceOne(filter, state);
                }
                _updatedContactStates.Clear();
            }

            // deletes
            if (_deletedContactStates.Any())
            {
                var collection = _database.GetCollection<ContactState>(ContactStatesCollection);
                foreach (var guid in _deletedContactStates)
                {
                    var filter = Builders<ContactState>.Filter.Eq("Guid", guid);
                    collection.DeleteOne(filter);
                }
                _deletedContactStates.Clear();
            }
        }

        public Task PersistChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
