﻿using CommandsShared;
using CompaniesShared;
using ContactsShared;
using EmploymentsShared;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using ProductsShared;
using ProjectsShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoftwareManagementMongoDbCoreRepository
{
    [BsonIgnoreExtraElements]
    public class ProductVersionState : IProductVersionState
    {
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Guid { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Revision { get; set; }
        public int Build { get; set; }
        public Guid ProductGuid { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class ProductState : IProductState
    {
        public ProductState()
        {
            ProductVersionStates = new List<IProductVersionState>();
        }

        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Guid { get; set; }
        public string Description { get; set; }
        public string BusinessCase { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public ICollection<IProductVersionState> ProductVersionStates { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class ProjectState : IProjectState
    {
        public ProjectState()
        {
            ProjectRoleStates = new List<IProjectRoleState>() as ICollection<IProjectRoleState>;
        }
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Guid { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<IProjectRoleState> ProjectRoleStates { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ProjectRoleState : IProjectRoleState
    {
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ContactState : IContactState
    {
        public DateTime? BirthDate { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Guid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class CompanyState : ICompanyState
    {
        public CompanyState()
        {
            CompanyRoleStates = new List<ICompanyRoleState>() as ICollection<ICompanyRoleState>;
        }
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Guid { get; set; }
        public ICollection<ICompanyRoleState> CompanyRoleStates { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class CompanyRoleState : ICompanyRoleState
    {
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class EmploymentState : IEmploymentState
    {
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Guid { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Guid ContactGuid { get; set; }
        public Guid CompanyRoleGuid { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ContactName { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class CommandState : ICommandState
    {
        [BsonId(IdGenerator = typeof(GuidGenerator))]
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
        private const string EmploymentStatesCollection = "EmploymentStates";

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

        private Dictionary<Guid, IEmploymentState> _employmentStates;
        private List<Guid> _deletedEmploymentStates;

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

            _employmentStates = new Dictionary<Guid, IEmploymentState>();
            _deletedEmploymentStates = new List<Guid>();
        }

        public void AddRoleToCompanyState(Guid guid, Guid roleGuid, string name)
        {
            var state = GetCompanyState(guid);
            var roleState = state.CompanyRoleStates.FirstOrDefault(s => s.Guid == roleGuid); // todo: work with Single and catch errors?
            if (roleState == null)
            {
                state.CompanyRoleStates.Add(new CompanyRoleState { Guid = roleGuid, Name = name });
            }
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

        public ICommandState CreateCommandState()
        {
            var state = new CommandState()
            {
                Guid = Guid.NewGuid()
            };
            _commandStates.Add(state.Guid, state);
            return state;
        }

        public ICompanyState CreateCompanyState(Guid guid, string name)
        {
            var state = new CompanyState()
            {
                Guid = guid
            };
            _companyStates.Add(state.Guid, state);
            return state;
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

        // for consideration - include some part of this in both the contact and company entity read projections?
        public IEmploymentState CreateEmploymentState(Guid guid, Guid contactGuid, Guid companyRoleGuid)
        {
            var state = new EmploymentState()
            {
                Guid = guid,
                ContactGuid = contactGuid,
                CompanyRoleGuid = companyRoleGuid
            };
            _employmentStates.Add(state.Guid, state);
            return state;
        }

        public IProductState CreateProductState(Guid guid, string name)
        {
            var state = new ProductState()
            {
                Guid = guid
            };
            _productStates.Add(state.Guid, state);
            return state;
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

        public void DeleteCompanyState(Guid guid)
        {
            _deletedCompanyStates.Add(guid);
        }

        public void DeleteContactState(Guid guid)
        {
            _deletedContactStates.Add(guid);
        }

        public void DeleteEmploymentState(Guid guid)
        {
            _deletedEmploymentStates.Add(guid);
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

        public ICompanyState GetCompanyState(Guid guid)
        {
            if (!_companyStates.TryGetValue(guid, out ICompanyState state))
            {
                if (!_updatedCompanyStates.TryGetValue(guid, out state))
                {
                    var collection = _database.GetCollection<CompanyState>(CompanyStatesCollection);
                    var filter = Builders<CompanyState>.Filter.Eq("Guid", guid);
                    state = collection.Find(filter).FirstOrDefault();

                    TrackCompanyState(state);

                }
            }
            return state;
        }

        public IEnumerable<ICompanyState> GetCompanyStates()
        {
            var collection = _database.GetCollection<CompanyState>(CompanyStatesCollection);
            var filter = new BsonDocument();
            var states = collection.Find(filter);

            return states?.ToList();
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

        // todo: this can probably be done more efficiently
        public IEnumerable<IEmploymentState> GetEmploymentsByCompanyRoleGuid(Guid companyRoleGuid)
        {
            var collection = _database.GetCollection<EmploymentState>(EmploymentStatesCollection);
            var filter = Builders<EmploymentState>.Filter.Eq("CompanyRoleGuid", companyRoleGuid);
            var states = collection.Find(filter);

            if (states != null)
            {
                var contactsCollection = _database.GetCollection<ContactState>(ContactStatesCollection);
                var contactGuids = states.ToList().Select(s => s.ContactGuid).ToList();
                var filterDef = new FilterDefinitionBuilder<ContactState>();
                var contactsFilter = filterDef.In(x => x.Guid, contactGuids);
                var contactStates = contactsCollection.Find(contactsFilter).ToList();
                var employmentStates = states.ToList();
                foreach(var state in contactStates)
                {
                    var employmentState = employmentStates.FirstOrDefault(s => s.ContactGuid == state.Guid);
                    if(employmentState!=null)
                    {
                        employmentState.ContactName = state.Name;
                    }
                }
            }
            return states?.ToList();
        }

        // todo: do we maybe want to store all contact data so that we can get all that by companyGuid at once?
        // if so we would need to update both here and in contacts for contactupates
        public IEnumerable<IContactState> GetContactsByCompanyRoleGuid(Guid companyRoleGuid)
        {
            var collection = _database.GetCollection<EmploymentState>(EmploymentStatesCollection);
            var filter = Builders<EmploymentState>.Filter.Eq("CompanyRoleGuid", companyRoleGuid);
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

        public IEnumerable<IEmploymentState> GetEmploymentsByContactGuid(Guid contactGuid)
        {
            var collection = _database.GetCollection<EmploymentState>(ContactStatesCollection);
            var filter = Builders<EmploymentState>.Filter.Eq("ContactRoleGuid", contactGuid);
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

        // todo: specify if read-only? it will likely barely matter in most cases,
        // as long as for write mode persistchanges does the work
        public IProductState GetProductState(Guid guid)
        {
            if (!_productStates.TryGetValue(guid, out IProductState state))
            {
                if (!_updatedProductStates.TryGetValue(guid, out state))
                {

                    var collection = _database.GetCollection<ProductState>(ProductStatesCollection);
                    var filter = Builders<ProductState>.Filter.Eq("Guid", guid);

                    state = collection.Find(filter).FirstOrDefault();

                    TrackProductState(state);
                }
            }
            return state;
        }

        private void TrackProductState(IProductState state)
        {
            if (state != null && !_updatedProductStates.ContainsKey(state.Guid))
            {
                _updatedProductStates.Add(state.Guid, state);
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

        private void TrackContactState(IContactState state)
        {
            if (state != null && !_updatedContactStates.ContainsKey(state.Guid))
            {
                _updatedContactStates.Add(state.Guid, state);
            }
        }

        private void TrackCompanyState(ICompanyState state)
        {
            if (state != null && !_updatedCompanyStates.ContainsKey(state.Guid))
            {
                _updatedCompanyStates.Add(state.Guid, state);
            }
        }

        // readonly by default. Should we enhance the interface? Or create a separate read-only repo?
        public IEnumerable<IProductState> GetProductStates()
        {
            var collection = _database.GetCollection<ProductState>(ProductStatesCollection);
            var filter = new BsonDocument();
            var states = collection.Find(filter);

            return states?.ToList();
        }

        public IEnumerable<IProjectState> GetProjectStates()
        {
            var collection = _database.GetCollection<ProjectState>(ProjectStatesCollection);
            var filter = new BsonDocument();
            var states = collection.Find(filter);

            return states?.ToList();
        }

        public void PersistChanges()
        {
            PersistCommands();

            PersistContacts();
            PersistProducts();
            PersistProjects();
            PersistCompanies();
            PersistEmployments();
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

        private void PersistCompanies()
        {
            var companyCollection = _database.GetCollection<CompanyState>(CompanyStatesCollection);
            // inserts
            if (_companyStates.Values.Any())
            {
                var companies = _companyStates.Values.Select(s => s as CompanyState).ToList();
                companyCollection.InsertMany(companies);
                _companyStates.Clear();
            }

            // todo: can these be batched?
            // updates
            if (_updatedCompanyStates.Values.Any())
            {
                var companies = _updatedCompanyStates.Values.Select(s => s as CompanyState).ToList();
                foreach (var state in companies)
                {
                    var filter = Builders<CompanyState>.Filter.Eq("Guid", state.Guid);
                    companyCollection.ReplaceOne(filter, state);
                }
                _updatedCompanyStates.Clear();
            }

            // deletes
            if (_deletedCompanyStates.Any())
            {
                var collection = _database.GetCollection<CompanyState>(CompanyStatesCollection);
                foreach (var guid in _deletedCompanyStates)
                {
                    var filter = Builders<CompanyState>.Filter.Eq("Guid", guid);
                    collection.DeleteOne(filter);
                }
                _deletedCompanyStates.Clear();
            }
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

        public Task PersistChangesAsync()
        {
            throw new NotImplementedException();
        }

        public void RemoveRoleFromCompanyState(Guid guid, Guid roleGuid)
        {
            var state = GetCompanyState(guid);
            var roleState = state.CompanyRoleStates.FirstOrDefault(s => s.Guid == roleGuid); // todo: work with Single and catch errors?
            if (roleState != null)
            {
                state.CompanyRoleStates.Remove(roleState);
            }
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

        public IProductVersionState CreateProductVersionState(Guid guid, Guid versionGuid, string name)
        {
            var state = GetProductState(guid);
            var versionState = new ProductVersionState();
            versionState.Guid = versionGuid;
            versionState.Name = name;
            state.ProductVersionStates.Add(versionState);
            return versionState;
        }
    }
}