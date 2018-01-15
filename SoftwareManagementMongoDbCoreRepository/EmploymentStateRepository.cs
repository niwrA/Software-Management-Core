using EmploymentsShared;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ContactsShared;

namespace SoftwareManagementMongoDbCoreRepository
{


    [BsonIgnoreExtraElements]
    public class EmploymentState : TimeStampedEntityState, IEmploymentState
    {
        public Guid ContactGuid { get; set; }
        public Guid CompanyRoleGuid { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ContactName { get; set; }
    }

    public class EmploymentStateRepository: IEmploymentStateRepository
    {
        private const string CommandStatesCollection = "CommandStates";
        private const string ProjectStatesCollection = "ProjectStates";
        private const string ContactStatesCollection = "ContactStates";

        private const string CompanyStatesCollection = "CompanyStates";
        private const string EmploymentStatesCollection = "EmploymentStates";
        private const string ProjectRoleAssignmentStatesCollection = "ProjectRoleAssignmentStates";

        private IMongoClient _client;
        private IMongoDatabase _database;

        private Dictionary<Guid, IEmploymentState> _employmentStates;
        private List<Guid> _deletedEmploymentStates;


        public EmploymentStateRepository(IMongoClient client)
        {
            _client = client;
            _database = _client.GetDatabase("SoftwareManagement");
            
            _employmentStates = new Dictionary<Guid, IEmploymentState>();
            _deletedEmploymentStates = new List<Guid>();

        }
        public IEmploymentState CreateEmploymentState(Guid guid, Guid linkGuid, Guid companyRoleGuid)
        {
            var state = new EmploymentState()
            {
                Guid = guid,
                ContactGuid = linkGuid,
                CompanyRoleGuid = companyRoleGuid
            };
            _employmentStates.Add(state.Guid, state);
            return state;
        }

        public void DeleteEmploymentState(Guid guid)
        {
            _deletedEmploymentStates.Add(guid);
        }

        public IEnumerable<IContactState> GetContactsByCompanyRoleGuid(Guid companyRoleGuid)
        {
            throw new NotImplementedException();
        }

        // todo: this can probably be done more efficiently
        public IEnumerable<IEmploymentState> GetEmploymentsByCompanyRoleGuid(Guid companyRoleGuid)
        {
            var collection = _database.GetCollection<EmploymentState>(EmploymentStatesCollection);
            var filter = Builders<EmploymentState>.Filter.Eq("CompanyRoleGuid", companyRoleGuid);
            var states = collection.Find(filter);

            if (states != null)
            {
                var linksCollection = _database.GetCollection<ContactState>(ContactStatesCollection);
                var linkGuids = states.ToList().Select(s => s.ContactGuid).ToList();
                var filterDef = new FilterDefinitionBuilder<ContactState>();
                var linksFilter = filterDef.In(x => x.Guid, linkGuids);
                var linkStates = linksCollection.Find(linksFilter).ToList();
                var employmentStates = states.ToList();
                foreach (var state in linkStates)
                {
                    var employmentState = employmentStates.FirstOrDefault(s => s.ContactGuid == state.Guid);
                    if (employmentState != null)
                    {
                        employmentState.ContactName = state.Name;
                    }
                }
            }
            return states?.ToList();
        }

        public IEnumerable<IEmploymentState> GetEmploymentsByContactGuid(Guid linkGuid)
        {
            var collection = _database.GetCollection<EmploymentState>(ContactStatesCollection);
            var filter = Builders<EmploymentState>.Filter.Eq("ContactRoleGuid", linkGuid);
            var states = collection.Find(filter);

            return states?.ToList();
        }

        public IEmploymentState GetEmploymentState(Guid guid)
        {
            if (!_employmentStates.TryGetValue(guid, out IEmploymentState state))
            {
                var collection = _database.GetCollection<EmploymentState>(EmploymentStatesCollection);
                var filter = Builders<EmploymentState>.Filter.Eq("Guid", guid);
                state = collection.Find(filter).FirstOrDefault();
            }
            return state;
        }

        public IEnumerable<IEmploymentState> GetEmploymentStates()
        {
            var collection = _database.GetCollection<EmploymentState>(EmploymentStatesCollection);
            var filter = new BsonDocument();
            var states = collection.Find(filter);

            return states?.ToList();
        }


        public void PersistChanges()
        {
            PersistEmployments();
        }

        private void PersistEmployments()
        {
            // inserts
            if (_employmentStates.Values.Any())
            {
                var collection = _database.GetCollection<EmploymentState>(EmploymentStatesCollection);
                var entities = _employmentStates.Values.Select(s => s as EmploymentState).ToList();
                collection.InsertMany(entities);
                _employmentStates.Clear();
            }

            // deletes
            if (_deletedEmploymentStates.Any())
            {
                var collection = _database.GetCollection<EmploymentState>(EmploymentStatesCollection);
                foreach (var guid in _deletedEmploymentStates)
                {
                    var filter = Builders<EmploymentState>.Filter.Eq("Guid", guid);
                    collection.DeleteOne(filter, null, CancellationToken.None);
                }
                _deletedEmploymentStates.Clear();
            }
        }



    }
}
