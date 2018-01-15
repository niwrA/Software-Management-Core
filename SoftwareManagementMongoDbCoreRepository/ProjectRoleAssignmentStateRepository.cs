using ContactsShared;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using ProjectRoleAssignmentsShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoftwareManagementMongoDbCoreRepository
{
    [BsonIgnoreExtraElements]
    public class ProjectRoleAssignmentState : TimeStampedEntityState, IProjectRoleAssignmentState
    {
        public Guid ContactGuid { get; set; }
        public Guid ProjectGuid { get; set; }
        public Guid ProjectRoleGuid { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ContactName { get; set; }
    }


    public class ProjectRoleAssignmentStateRepository: IProjectRoleAssignmentStateRepository
    {
        private const string ContactStatesCollection = "ContactStates";
        private const string ProjectStatesCollection = "ProjectStates";
        private const string ProjectRoleAssignmentStatesCollection = "ProjectRoleAssignmentStates";

        private IMongoClient _client;
        private IMongoDatabase _database;

        private Dictionary<Guid, IProjectRoleAssignmentState> _projectRoleAssignmentStates;
        private List<Guid> _deletedProjectRoleAssignmentStates;

        public ProjectRoleAssignmentStateRepository(IMongoClient client)
        {
            _client = client;
            _database = _client.GetDatabase("SoftwareManagement");

            _projectRoleAssignmentStates = new Dictionary<Guid, IProjectRoleAssignmentState>();
            _deletedProjectRoleAssignmentStates = new List<Guid>();
        }


        private void PersistProjectRoleAssignments()
        {
            // inserts
            if (_projectRoleAssignmentStates.Values.Any())
            {
                var collection = _database.GetCollection<ProjectRoleAssignmentState>(ProjectRoleAssignmentStatesCollection);
                var entities = _projectRoleAssignmentStates.Values.Select(s => s as ProjectRoleAssignmentState).ToList();
                collection.InsertMany(entities);
                _projectRoleAssignmentStates.Clear();
            }

            // deletes
            if (_deletedProjectRoleAssignmentStates.Any())
            {
                var collection = _database.GetCollection<ProjectRoleAssignmentState>(ProjectRoleAssignmentStatesCollection);
                foreach (var guid in _deletedProjectRoleAssignmentStates)
                {
                    var filter = Builders<ProjectRoleAssignmentState>.Filter.Eq("Guid", guid);
                    collection.DeleteOne(filter, null, CancellationToken.None);
                }
                _deletedProjectRoleAssignmentStates.Clear();
            }
        }
        public Task PersistChangesAsync()
        {
            throw new NotImplementedException();
        }

        public IProjectRoleAssignmentState CreateProjectRoleAssignmentState(Guid guid, Guid contactGuid, Guid projectGuid, Guid projectRoleAssignmentGuid)
        {
            var state = new ProjectRoleAssignmentState()
            {
                Guid = guid,
                ContactGuid = contactGuid,
                ProjectGuid = projectGuid,
                ProjectRoleGuid = projectRoleAssignmentGuid
            };
            _projectRoleAssignmentStates.Add(state.Guid, state);
            return state;
        }

        public IProjectRoleAssignmentState GetProjectRoleAssignmentState(Guid guid)
        {
            if (!_projectRoleAssignmentStates.TryGetValue(guid, out IProjectRoleAssignmentState state))
            {
                var collection = _database.GetCollection<ProjectRoleAssignmentState>(ProjectRoleAssignmentStatesCollection);
                var filter = Builders<ProjectRoleAssignmentState>.Filter.Eq("Guid", guid);
                state = collection.Find(filter).FirstOrDefault();
            }
            return state;
        }

        public IEnumerable<IProjectRoleAssignmentState> GetProjectRoleAssignmentsByProjectRoleGuid(Guid projectRoleGuid)
        {
            var collection = _database.GetCollection<ProjectRoleAssignmentState>(ProjectRoleAssignmentStatesCollection);
            var filter = Builders<ProjectRoleAssignmentState>.Filter.Eq("ProjectRoleGuid", projectRoleGuid);
            var states = collection.Find(filter);

            if (states != null)
            {
                var linksCollection = _database.GetCollection<ContactState>(ContactStatesCollection);
                var linkGuids = states.ToList().Select(s => s.ContactGuid).ToList();
                var filterDef = new FilterDefinitionBuilder<ContactState>();
                var linksFilter = filterDef.In(x => x.Guid, linkGuids);
                var linkStates = linksCollection.Find(linksFilter).ToList();
                var projectRoleAssignmentStates = states.ToList();
                foreach (var state in linkStates)
                {
                    var projectRoleAssignmentState = projectRoleAssignmentStates.FirstOrDefault(s => s.ContactGuid == state.Guid);
                    if (projectRoleAssignmentState != null)
                    {
                        projectRoleAssignmentState.ContactName = state.Name;
                    }
                }
            }
            return states?.ToList();
        }

        public IEnumerable<IContactState> GetContactsByProjectRoleGuid(Guid projectRoleGuid)
        {
            var collection = _database.GetCollection<ProjectRoleAssignmentState>(ProjectRoleAssignmentStatesCollection);
            var filter = Builders<ProjectRoleAssignmentState>.Filter.Eq("ProjectRoleGuid", projectRoleGuid);
            var states = collection.Find(filter);
            if (states != null)
            {
                var linksCollection = _database.GetCollection<ContactState>(ContactStatesCollection);
                var linkGuids = states.ToList().Select(s => s.ContactGuid).ToList();
                var filterDef = new FilterDefinitionBuilder<ContactState>();
                var linksFilter = filterDef.In(x => x.Guid, linkGuids);
                var linkStates = linksCollection.Find(linksFilter).ToList();
                return linkStates?.ToList();
            }
            return null;
        }

        public void DeleteProjectRoleAssignmentState(Guid guid)
        {
            _deletedProjectRoleAssignmentStates.Add(guid);
        }
        // todo: add test
        public IEnumerable<IProjectRoleAssignmentState> GetProjectRoleAssignmentStates()
        {
            var collection = _database.GetCollection<ProjectRoleAssignmentState>(ProjectRoleAssignmentStatesCollection);
            var filter = new BsonDocument();
            var states = collection.Find(filter);

            return states?.ToList();
        }

        public IEnumerable<IProjectRoleAssignmentState> GetProjectRoleAssignmentsByContactGuid(Guid contactGuid)
        {
            throw new NotImplementedException();
        }

        public void PersistChanges()
        {
            PersistProjectRoleAssignments();
        }

        public IEnumerable<IContactState> GetContactsByProjectGuid(Guid guid)
        {
            var collection = _database.GetCollection<ProjectRoleAssignmentState>(ProjectRoleAssignmentStatesCollection);
            var filter = Builders<ProjectRoleAssignmentState>.Filter.Eq("ProjectGuid", guid);
            var states = collection.Find(filter);
            if (states != null)
            {
                var contactsCollection = _database.GetCollection<ContactState>(ContactStatesCollection);
                var contactGuids = states.ToList().Select(s => s.ContactGuid).ToList();
                var filterDef = new FilterDefinitionBuilder<ContactState>();
                var contactsFilter = filterDef.In(x => x.Guid, contactGuids);
                var contactStates = contactsCollection.Find(contactsFilter).ToList();
                return contactStates?.ToList();
            }
            return null;
        }


    }
}
