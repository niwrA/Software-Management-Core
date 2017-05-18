using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using ProjectsShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareManagementMongoDbCoreRepository
{
    [BsonIgnoreExtraElements]
    public class ProjectState : NamedEntityState, IProjectState
    {
        public ProjectState()
        {
            ProjectRoleStates = new List<IProjectRoleState>() as ICollection<IProjectRoleState>;
        }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<IProjectRoleState> ProjectRoleStates { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ProjectRoleState : NamedEntityState, IProjectRoleState
    {
    }

    public class ProjectStateRepository: IProjectStateRepository
    {
        private const string ProjectStatesCollection = "ProjectStates";

        private IMongoClient _client;
        private IMongoDatabase _database;

        private Dictionary<Guid, IProjectState> _projectStates;
        private List<Guid> _deletedProjectStates;
        private Dictionary<Guid, IProjectState> _updatedProjectStates;
        public ProjectStateRepository(IMongoClient client)
        {
            _client = client;
            _database = _client.GetDatabase("SoftwareManagement");

            _projectStates = new Dictionary<Guid, IProjectState>();
            _deletedProjectStates = new List<Guid>();
            _updatedProjectStates = new Dictionary<Guid, IProjectState>();
        }

        public IProjectState CreateProjectState(Guid guid, string name)
        {
            var state = new ProjectState()
            {
                Guid = guid
            };
            _projectStates.Add(state.Guid, state);
            return state;
        }
        public void DeleteProjectState(Guid guid)
        {
            _deletedProjectStates.Add(guid);
        }

        public void AddRoleToProjectState(Guid guid, Guid roleGuid, string name)
        {
            var state = GetProjectState(guid);
            var roleState = state.ProjectRoleStates.FirstOrDefault(s => s.Guid == roleGuid); // todo: work with Single and catch errors?
            if (roleState == null)
            {
                state.ProjectRoleStates.Add(new ProjectRoleState { Guid = roleGuid, Name = name });
            }
        }

        public IProjectState GetProjectState(Guid guid)
        {
            if (!_projectStates.TryGetValue(guid, out IProjectState state))
            {
                if (!_updatedProjectStates.TryGetValue(guid, out state))
                {
                    var collection = _database.GetCollection<ProjectState>(ProjectStatesCollection);
                    var filter = Builders<ProjectState>.Filter.Eq("Guid", guid);
                    state = collection.Find(filter).FirstOrDefault();

                    TrackProjectState(state);
                }
            }
            return state;
        }

        private void TrackProjectState(IProjectState state)
        {
            if (state != null && !_updatedProjectStates.ContainsKey(state.Guid))
            {
                _updatedProjectStates.Add(state.Guid, state);
            }
        }

        public IEnumerable<IProjectState> GetProjectStates()
        {
            var collection = _database.GetCollection<ProjectState>(ProjectStatesCollection);
            var filter = new BsonDocument();
            var states = collection.Find(filter);

            return states?.ToList();
        }

        public void RemoveRoleFromProjectState(Guid guid, Guid roleGuid)
        {
            var state = GetProjectState(guid);
            var roleState = state.ProjectRoleStates.FirstOrDefault(s => s.Guid == roleGuid); // todo: work with Single and catch errors?
            if (roleState != null)
            {
                state.ProjectRoleStates.Remove(roleState);
            }
        }

        private void PersistProjects()
        {
            var projectCollection = _database.GetCollection<ProjectState>(ProjectStatesCollection);
            // inserts
            if (_projectStates.Values.Any())
            {
                var projects = _projectStates.Values.Select(s => s as ProjectState).ToList();
                projectCollection.InsertMany(projects);
                _projectStates.Clear();
            }

            // todo: can these be batched?
            // updates
            if (_updatedProjectStates.Values.Any())
            {
                var projects = _updatedProjectStates.Values.Select(s => s as ProjectState).ToList();
                foreach (var state in projects)
                {
                    var filter = Builders<ProjectState>.Filter.Eq("Guid", state.Guid);
                    projectCollection.ReplaceOne(filter, state);
                }
                _updatedProjectStates.Clear();
            }

            // deletes
            if (_deletedProjectStates.Any())
            {
                var collection = _database.GetCollection<ProjectState>(ProjectStatesCollection);
                foreach (var guid in _deletedProjectStates)
                {
                    var filter = Builders<ProjectState>.Filter.Eq("Guid", guid);
                    collection.DeleteOne(filter);
                }
                _deletedProjectStates.Clear();
            }
        }

        public void PersistChanges()
        {
            PersistProjects();
        }

        public Task PersistChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
