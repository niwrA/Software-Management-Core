using CommandsShared;
using CompaniesShared;
using ContactsShared;
using EmploymentsShared;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using ProductsShared;
using ProjectsShared;
using CompaniesShared;
using ContactsShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoftwareManagementMongoDbCoreRepository
{
    [BsonIgnoreExtraElements]
    public class ProductState : IProductState
    {
        public string Description { get; set; }
        public string BusinessCase { get; set; }
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class ProjectState : IProjectState
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<IProjectRoleState> ProjectRoleStates { get; set; }
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ContactState : IContactState
    {
        public DateTime? BirthDate { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class CompanyState : ICompanyState
    {
        public ICollection<ICompanyRoleState> CompanyRoleStates { get; set; }
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class CommandState : ICommandState
    {
        public Guid Guid { get; set; }
        public Guid EntityGuid { get; set; }
        public string CommandTypeId { get; set; }
        public string ParametersJson { get; set; }
        public DateTime? ExecutedOn { get; set; }
        public DateTime? ReceivedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UserName { get; set; }
    }
    public interface IMainRepository : IProductStateRepository, IContactStateRepository,
        IProjectStateRepository, ICompanyStateRepository, ICommandStateRepository, IEmploymentStateRepository
    { };
    public class MainRepository : IMainRepository
    {
        private const string CommandStatesCollection = "CommandStates";
        private const string ProductStatesCollection = "ProductStates";
        private const string ProjectStatesCollection = "ProjectStates";
        private const string ContactStatesCollection = "ContactStates";
        private const string CompanyStatesCollection = "CompanyStates";

        private IMongoClient _client;
        private IMongoDatabase _database;
        private Dictionary<Guid, IProductState> _productStates;
        private List<Guid> _deletedProductStates;
        private Dictionary<Guid, IProductState> _updatedProductStates;

        private Dictionary<Guid, IProjectState> _projectStates;
        private List<Guid> _deletedProjectStates;
        private Dictionary<Guid, IProjectState> _updatedProjectStates;

        private Dictionary<Guid, IContactState> _contactStates;
        private List<Guid> _deletedContactStates;
        private Dictionary<Guid, IContactState> _updatedContactStates;

        private Dictionary<Guid, ICompanyState> _companyStates;
        private List<Guid> _deletedCompanyStates;
        private Dictionary<Guid, ICompanyState> _updatedCompanyStates;

        private Dictionary<Guid, ICommandState> _commandStates { get; set; }

        public MainRepository(IMongoClient client)
        {
            _client = client;
            _database = _client.GetDatabase("SoftwareManagement");
            _commandStates = new Dictionary<Guid, ICommandState>();
            _productStates = new Dictionary<Guid, IProductState>();
            _deletedProductStates = new List<Guid>();
            _updatedProductStates = new Dictionary<Guid, IProductState>();

            _projectStates = new Dictionary<Guid, IProjectState>();
            _deletedProjectStates = new List<Guid>();
            _updatedProjectStates = new Dictionary<Guid, IProjectState>();

            _contactStates = new Dictionary<Guid, IContactState>();
            _deletedContactStates = new List<Guid>();
            _updatedContactStates = new Dictionary<Guid, IContactState>();

            _companyStates = new Dictionary<Guid, ICompanyState>();
            _deletedCompanyStates = new List<Guid>();
            _updatedCompanyStates = new Dictionary<Guid, ICompanyState>();
        }

        public void AddRoleToCompanyState(Guid projectGuid, Guid roleGuid, string name)
        {
            throw new NotImplementedException();
        }

        public void AddRoleToProjectState(Guid projectGuid, Guid roleGuid, string name)
        {
            throw new NotImplementedException();
        }

        public ICommandState CreateCommandState()
        {
            var state = new CommandState();
            state.Guid = Guid.NewGuid();
            _commandStates.Add(state.Guid, state);
            return state;
        }

        public ICompanyState CreateCompanyState(Guid guid, string name)
        {
            var state = new CompanyState()
            {
                Guid = Guid.NewGuid()
            };
            _companyStates.Add(state.Guid, state);
            return state;
        }

        public IContactState CreateContactState(Guid guid, string name)
        {
            var state = new ContactState()
            {
                Guid = Guid.NewGuid()
            };
            _contactStates.Add(state.Guid, state);
            return state;
        }

        public IEmploymentState CreateEmploymentState(Guid guid, Guid contactGuid, Guid companyRoleGuid)
        {
            throw new NotImplementedException();
        }

        public IProductState CreateProductState(Guid guid, string name)
        {
            var state = new ProductState()
            {
                Guid = Guid.NewGuid()
            };
            _productStates.Add(state.Guid, state);
            return state;
        }

        public IProjectState CreateProjectState(Guid guid, string name)
        {
            var state = new ProjectState()
            {
                Guid = Guid.NewGuid()
            };
            _projectStates.Add(state.Guid, state);
            return state;
        }

        public void DeleteCompanyState(Guid guid)
        {
            _deletedCompanyStates.Add(guid);
        }

        public void DeleteContactState(Guid guid)
        {
            _deletedContactStates.Add(guid);
        }

        public void DeleteEmploymentState(Guid entityGuid)
        {
            throw new NotImplementedException();
        }

        public void DeleteProductState(Guid guid)
        {
            _deletedProductStates.Add(guid);
        }

        public void DeleteProjectState(Guid guid)
        {
            _deletedProjectStates.Add(guid);
        }

        public IEnumerable<ICommandState> GetCommandStates(Guid entityGuid)
        {
            var states = new List<ICommandState>();
            var collection = _database.GetCollection<CommandState>(CommandStatesCollection);
            var filter = Builders<CommandState>.Filter.Eq("EntityGuid", entityGuid);
            var results = collection.Find(filter).ToList();
            if (results.Count > 0)
            {
                foreach (var result in results)
                {
                    states.Add(result);
                }
            }

            return states;
        }

        public ICompanyState GetCompanyState(Guid guid)
        {
            var collection = _database.GetCollection<CompanyState>(CompanyStatesCollection);
            var filter = Builders<CompanyState>.Filter.Eq("Guid", guid);
            ICompanyState state = collection.Find(filter).First();

            TrackCompanyState(state);

            return state;
        }

        public IEnumerable<ICompanyState> GetCompanyStates()
        {
            var collection = _database.GetCollection<CompanyState>(CompanyStatesCollection);
            var filter = new BsonDocument();
            var states = collection.Find(filter).ToList();

            return states;
        }

        public IContactState GetContactState(Guid guid)
        {
            var collection = _database.GetCollection<ContactState>(ContactStatesCollection);
            var filter = Builders<ContactState>.Filter.Eq("Guid", guid);
            IContactState state = collection.Find(filter).First();

            TrackContactState(state);

            return state;
        }

        public IEnumerable<IContactState> GetContactStates()
        {
            var collection = _database.GetCollection<ContactState>(ContactStatesCollection);
            var filter = new BsonDocument();
            var states = collection.Find(filter).ToList();

            return states;
        }

        public ICollection<IEmploymentState> GetEmploymentsByCompanyRoleGuid(Guid companyRoleGuid)
        {
            throw new NotImplementedException();
        }

        public ICollection<IEmploymentState> GetEmploymentsByContactGuid(Guid contactGuid)
        {
            throw new NotImplementedException();
        }

        public IEmploymentState GetEmploymentState(Guid guid)
        {
            throw new NotImplementedException();
        }

        public ICollection<IEmploymentState> GetEmploymentStates()
        {
            throw new NotImplementedException();
        }

        // todo: specify if read-only? it will likely barely matter in most cases,
        // as long as for write mode persistchanges does the work
        public IProductState GetProductState(Guid guid)
        {
            var collection = _database.GetCollection<ProductState>(ProductStatesCollection);
            var filter = Builders<ProductState>.Filter.Eq("Guid", guid);
            IProductState state = collection.Find(filter).First();

            TrackProductState(state);

            return state;
        }

        private void TrackProductState(IProductState state)
        {
            if (!_updatedProductStates.ContainsKey(state.Guid))
            {
                _updatedProductStates.Add(state.Guid, state);
            }
        }

        public IProjectState GetProjectState(Guid guid)
        {
            var collection = _database.GetCollection<ProjectState>(ProjectStatesCollection);
            var filter = Builders<ProjectState>.Filter.Eq("Guid", guid);
            IProjectState state = collection.Find(filter).First();

            TrackProjectState(state);

            return state;
        }

        private void TrackProjectState(IProjectState state)
        {
            if (!_updatedProjectStates.ContainsKey(state.Guid))
            {
                _updatedProjectStates.Add(state.Guid, state);
            }
        }

        private void TrackContactState(IContactState state)
        {
            if (!_updatedContactStates.ContainsKey(state.Guid))
            {
                _updatedContactStates.Add(state.Guid, state);
            }
        }

        private void TrackCompanyState(ICompanyState state)
        {
            if (!_updatedCompanyStates.ContainsKey(state.Guid))
            {
                _updatedCompanyStates.Add(state.Guid, state);
            }
        }

        // readonly by default. Should we enhance the interface? Or create a separate read-only repo?
        public IEnumerable<IProductState> GetProductStates()
        {
            var collection = _database.GetCollection<ProductState>(ProductStatesCollection);
            var filter = new BsonDocument();
            var states = collection.Find(filter).ToList();

            return states;
        }

        public IEnumerable<IProjectState> GetProjectStates()
        {
            var collection = _database.GetCollection<ProjectState>(ProjectStatesCollection);
            var filter = new BsonDocument();
            var states = collection.Find(filter).ToList();

            return states;
        }

        public void PersistChanges()
        {
            PersistCommands();

            PersistProducts();

            PersistContacts();
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

        private void PersistProducts()
        {
            var productCollection = _database.GetCollection<ProductState>(ProductStatesCollection);
            // inserts
            if (_productStates.Values.Any())
            {
                var products = _productStates.Values.Select(s => s as ProductState).ToList();
                productCollection.InsertMany(products);
                _productStates.Clear();
            }

            // todo: can these be batched?
            // updates
            if (_updatedProductStates.Values.Any())
            {
                var products = _updatedProductStates.Values.Select(s => s as ProductState).ToList();
                foreach (var state in products)
                {
                    var filter = Builders<ProductState>.Filter.Eq("Guid", state.Guid);
                    productCollection.ReplaceOne(filter, state);
                }
                _updatedProductStates.Clear();
            }

            // deletes
            if (_deletedProductStates.Any())
            {
                var collection = _database.GetCollection<ProductState>(ProductStatesCollection);
                foreach (var guid in _deletedProductStates)
                {
                    var filter = Builders<ProductState>.Filter.Eq("Guid", guid);
                    collection.DeleteOne(filter);
                }
                _deletedProductStates.Clear();
            }
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

        public void RemoveRoleFromCompanyState(Guid guid, Guid roleGuid)
        {
            throw new NotImplementedException();
        }

        public void RemoveRoleFromProjectState(Guid guid, Guid roleGuid)
        {
            throw new NotImplementedException();
        }
    }
}
