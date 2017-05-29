using FilesShared;
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
    public class FileState : NamedEntityState, IFileState
    {
        public string FileName { get; set; }
        public string FolderName { get { return ForType + @"/" + ForGuid; } }
        public Guid EntityGuid { get; set; }
        public Guid ForGuid { get; set; }
        public string ForType { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
    }

    public class FileStateRepository : IFileStateRepository
    {
        private const string FileStatesCollection = "FileStates";

        private IMongoClient _client;
        private IMongoDatabase _database;

        private Dictionary<Guid, IFileState> _linkStates;
        private List<Guid> _deletedFileStates;
        private Dictionary<Guid, IFileState> _updatedFileStates;

        public FileStateRepository(IMongoClient client)
        {
            _client = client;
            _database = _client.GetDatabase("SoftwareManagement");
            _linkStates = new Dictionary<Guid, IFileState>();
            _deletedFileStates = new List<Guid>();
            _updatedFileStates = new Dictionary<Guid, IFileState>();
        }
        public IFileState CreateFileState(Guid guid, string name)
        {
            var state = new FileState()
            {
                Guid = guid,
                Name = name
            };
            _linkStates.Add(state.Guid, state);
            return state;
        }

        public IFileState GetFileState(Guid guid)
        {
            if (!_linkStates.TryGetValue(guid, out IFileState state))
            {
                if (!_updatedFileStates.TryGetValue(guid, out state))
                {
                    var collection = _database.GetCollection<FileState>(FileStatesCollection);
                    var filter = Builders<FileState>.Filter.Eq("Guid", guid);
                    state = collection.Find(filter).FirstOrDefault();

                    TrackFileState(state);
                }
            }
            return state;
        }

        public IEnumerable<IFileState> GetFileStates()
        {
            var collection = _database.GetCollection<FileState>(FileStatesCollection);
            var filter = new BsonDocument();
            var states = collection.Find(filter);

            return states?.ToList();
        }

        public IEnumerable<IFileState> GetFileStatesForGuid(Guid forGuid)
        {
            var collection = _database.GetCollection<FileState>(FileStatesCollection);
            var filter = Builders<FileState>.Filter.Eq("ForGuid", forGuid);
            var states = collection.Find(filter);

            return states?.ToList();
        }

        public void DeleteFileState(Guid guid)
        {
            _deletedFileStates.Add(guid);
        }


        private void TrackFileState(IFileState state)
        {
            if (state != null && !_updatedFileStates.ContainsKey(state.Guid))
            {
                _updatedFileStates.Add(state.Guid, state);
            }
        }
        private void PersistFiles()
        {
            var linkCollection = _database.GetCollection<FileState>(FileStatesCollection);
            // inserts
            if (_linkStates.Values.Any())
            {
                var links = _linkStates.Values.Select(s => s as FileState).ToList();
                linkCollection.InsertMany(links);
                _linkStates.Clear();
            }

            // todo: can these be batched?
            // updates
            if (_updatedFileStates.Values.Any())
            {
                var links = _updatedFileStates.Values.Select(s => s as FileState).ToList();
                foreach (var state in links)
                {
                    var filter = Builders<FileState>.Filter.Eq("Guid", state.Guid);
                    linkCollection.ReplaceOne(filter, state);
                }
                _updatedFileStates.Clear();
            }

            // deletes
            if (_deletedFileStates.Any())
            {
                var collection = _database.GetCollection<FileState>(FileStatesCollection);
                foreach (var guid in _deletedFileStates)
                {
                    var filter = Builders<FileState>.Filter.Eq("Guid", guid);
                    collection.DeleteOne(filter);
                }
                _deletedFileStates.Clear();
            }
        }

        public void PersistChanges()
        {

            PersistFiles();
        }

        public Task PersistChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
