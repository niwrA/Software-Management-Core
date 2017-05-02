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
    public class ProductVersionState : NamedEntityState, IProductVersionState
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Revision { get; set; }
        public int Build { get; set; }
        public Guid ProductGuid { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class ProductState : NamedEntityState, IProductState
    {
        public ProductState()
        {
            ProductVersionStates = new List<IProductVersionState>() as ICollection<IProductVersionState>;
        }

        public string Description { get; set; }
        public string BusinessCase { get; set; }
        public ICollection<IProductVersionState> ProductVersionStates { get; set; }
    }
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
    public class ContactState : NamedEntityState, IContactState
    {
        public DateTime? BirthDate { get; set; }
        public string Email { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class CompanyState : NamedEntityState, ICompanyState
    {
        public CompanyState()
        {
            CompanyRoleStates = new List<ICompanyRoleState>() as ICollection<ICompanyRoleState>;
            CompanyEnvironmentStates = new List<ICompanyEnvironmentState>() as ICollection<ICompanyEnvironmentState>;
        }
        public ICollection<ICompanyRoleState> CompanyRoleStates { get; set; }
        public ICollection<ICompanyEnvironmentState> CompanyEnvironmentStates { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class CompanyRoleState :NamedEntityState, ICompanyRoleState
    {
    }

    [BsonIgnoreExtraElements]
    public class CompanyEnvironmentState : NamedEntityState, ICompanyEnvironmentState
    {
        public string Url { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class EmploymentState :EntityState, IEmploymentState
    {
        public Guid ContactGuid { get; set; }
        public Guid CompanyRoleGuid { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ContactName { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ProjectRoleAssignmentState : EntityState, IProjectRoleAssignmentState
    {
        public Guid ContactGuid { get; set; }
        public Guid ProjectRoleGuid { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ContactName { get; set; }
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
    public interface IMainRepository : IProductStateRepository, IContactStateRepository,
        IProjectStateRepository, ICompanyStateRepository, ICommandStateRepository, IEmploymentStateRepository,
        IProjectRoleAssignmentStateRepository, ILinkStateRepository
    { };
    public class MainRepository : IMainRepository
    {
        private const string CommandStatesCollection = "CommandStates";
        private const string ProductStatesCollection = "ProductStates";
        private const string DesignStatesCollection = "DesignStates";
        private const string ProjectStatesCollection = "ProjectStates";
        private const string ContactStatesCollection = "ContactStates";
        private const string CompanyStatesCollection = "CompanyStates";
        private const string EmploymentStatesCollection = "EmploymentStates";
        private const string ProjectRoleAssignmentStatesCollection = "ProjectRoleAssignmentStates";
        private const string LinkStatesCollection = "LinkStates";

        private IMongoClient _client;
        private IMongoDatabase _database;

        private Dictionary<Guid, IProductState> _productStates;
        private List<Guid> _deletedProductStates;
        private Dictionary<Guid, IProductState> _updatedProductStates;

        private Dictionary<Guid, IDesignState> _designStates;
        private List<Guid> _deletedDesignStates;
        private Dictionary<Guid, IDesignState> _updatedDesignStates;

        private Dictionary<Guid, IProjectState> _projectStates;
        private List<Guid> _deletedProjectStates;
        private Dictionary<Guid, IProjectState> _updatedProjectStates;

        private Dictionary<Guid, IContactState> _contactStates;
        private List<Guid> _deletedContactStates;
        private Dictionary<Guid, IContactState> _updatedContactStates;

        private Dictionary<Guid, ICompanyState> _companyStates;
        private List<Guid> _deletedCompanyStates;
        private Dictionary<Guid, ICompanyState> _updatedCompanyStates;

        private Dictionary<Guid, ILinkState> _linkStates;
        private List<Guid> _deletedLinkStates;
        private Dictionary<Guid, ILinkState> _updatedLinkStates;

        private Dictionary<Guid, IEmploymentState> _employmentStates;
        private List<Guid> _deletedEmploymentStates;

        private Dictionary<Guid, IProjectRoleAssignmentState> _projectRoleAssignmentStates;
        private List<Guid> _deletedProjectRoleAssignmentStates;

        private Dictionary<Guid, ICommandState> _commandStates { get; set; }

        public MainRepository(IMongoClient client)
        {
            _client = client;
            _database = _client.GetDatabase("SoftwareManagement");

            _commandStates = new Dictionary<Guid, ICommandState>();

            _productStates = new Dictionary<Guid, IProductState>();
            _deletedProductStates = new List<Guid>();
            _updatedProductStates = new Dictionary<Guid, IProductState>();

            _designStates = new Dictionary<Guid, IDesignState>();
            _deletedDesignStates = new List<Guid>();
            _updatedDesignStates = new Dictionary<Guid, IDesignState>();

            _projectStates = new Dictionary<Guid, IProjectState>();
            _deletedProjectStates = new List<Guid>();
            _updatedProjectStates = new Dictionary<Guid, IProjectState>();

            _contactStates = new Dictionary<Guid, IContactState>();
            _deletedContactStates = new List<Guid>();
            _updatedContactStates = new Dictionary<Guid, IContactState>();

            _companyStates = new Dictionary<Guid, ICompanyState>();
            _deletedCompanyStates = new List<Guid>();
            _updatedCompanyStates = new Dictionary<Guid, ICompanyState>();

            _linkStates = new Dictionary<Guid, ILinkState>();
            _deletedLinkStates = new List<Guid>();
            _updatedLinkStates = new Dictionary<Guid, ILinkState>();

            _employmentStates = new Dictionary<Guid, IEmploymentState>();
            _deletedEmploymentStates = new List<Guid>();

            _projectRoleAssignmentStates = new Dictionary<Guid, IProjectRoleAssignmentState>();
            _deletedProjectRoleAssignmentStates = new List<Guid>();
        }

        public ICompanyRoleState AddRoleToCompanyState(Guid guid, Guid roleGuid, string name)
        {
            var state = GetCompanyState(guid);
            var roleState = state.CompanyRoleStates.FirstOrDefault(s => s.Guid == roleGuid); // todo: work with Single and catch errors?
            if (roleState == null)
            {
                roleState = new CompanyRoleState { Guid = roleGuid, Name = name };
                state.CompanyRoleStates.Add(roleState);
            }
            return roleState;
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

        // for consideration - include some part of this in both the link and company entity read projections?
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

        // todo: do we maybe want to store all link data so that we can get all that by companyGuid at once?
        // if so we would need to update both here and in links for linkupates
        public IEnumerable<IContactState> GetContactsByCompanyRoleGuid(Guid companyRoleGuid)
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
                return linkStates?.ToList();
            }
            return null;
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
        private void TrackDesignState(IDesignState state)
        {
            if (state != null && !_updatedDesignStates.ContainsKey(state.Guid))
            {
                _updatedDesignStates.Add(state.Guid, state);
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

        private void TrackLinkState(ILinkState state)
        {
            if (state != null && !_updatedLinkStates.ContainsKey(state.Guid))
            {
                _updatedLinkStates.Add(state.Guid, state);
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

            PersistLinks();
            PersistContacts();
            PersistProducts();
            PersistProjects();
            PersistCompanies();
            PersistEmployments();
            PersistProjectRoleAssignments();
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
        // todo: repository tests
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
            var versionState = new ProductVersionState()
            {
                Guid = versionGuid,
                Name = name
            };
            state.ProductVersionStates.Add(versionState);
            return versionState;
        }

        public ICompanyEnvironmentState AddEnvironmentToCompanyState(Guid guid, Guid environmentGuid, string name)
        {
            var state = GetCompanyState(guid);
            var environmentState = state.CompanyEnvironmentStates.FirstOrDefault(s => s.Guid == environmentGuid); // todo: work with Single and catch errors?
            if (environmentState == null)
            {
                environmentState = new CompanyEnvironmentState { Guid = environmentGuid, Name = name };
                state.CompanyEnvironmentStates.Add(environmentState);
            } // todo: else throw error? replace?
            return environmentState;
        }
        // todo: add test
        public void RemoveEnvironmentFromCompanyState(Guid guid, Guid environmentGuid)
        {
            var state = GetCompanyState(guid);
            var environmentState = state.CompanyEnvironmentStates.FirstOrDefault(s => s.Guid == environmentGuid); // todo: work with Single and catch errors?
            if (environmentState != null)
            {
                state.CompanyEnvironmentStates.Remove(environmentState);
            }
        }

        public ICompanyEnvironmentState GetEnvironmentState(Guid companyGuid, Guid environmentGuid)
        {
            var companyState = GetCompanyState(companyGuid);
            var state = companyState.CompanyEnvironmentStates.SingleOrDefault(s => s.Guid == environmentGuid);
            return state;
        }

        public IProjectRoleAssignmentState CreateProjectRoleAssignmentState(Guid guid, Guid linkGuid, Guid companyRoleGuid)
        {
            var state = new ProjectRoleAssignmentState()
            {
                Guid = guid,
                ContactGuid = linkGuid,
                ProjectRoleGuid = companyRoleGuid
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

        public IEnumerable<IProjectRoleAssignmentState> GetProjectRoleAssignmentsByContactGuid(Guid contactGuid)
        {
            throw new NotImplementedException();
        }
    }
}
