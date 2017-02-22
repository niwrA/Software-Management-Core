using CommandsShared;
using CompaniesShared;
using ContactsShared;
using EmploymentsShared;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using ProductsShared;
using ProjectsShared;
using System;
using System.Collections.Generic;
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
        private IMongoClient _client;
        private IMongoDatabase _database;
        private Dictionary<Guid, ICommandState> _commandStates { get; set; }
        public MainRepository(IMongoClient client)
        {
            _client = client;
            _database = _client.GetDatabase("SoftwareManagement");
            _commandStates = new Dictionary<Guid, ICommandState>();
        }
        public void AddCommandState(ICommandState state)
        {
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
            throw new NotImplementedException();
        }

        public IContactState CreateContactState(Guid guid, string name)
        {
            throw new NotImplementedException();
        }

        public IEmploymentState CreateEmploymentState(Guid guid, Guid contactGuid, Guid companyRoleGuid)
        {
            throw new NotImplementedException();
        }

        public IProductState CreateProductState(Guid guid, string name)
        {
            throw new NotImplementedException();
        }

        public IProjectState CreateProjectState(Guid guid, string name)
        {
            throw new NotImplementedException();
        }

        public void DeleteCompanyState(Guid guid)
        {
            throw new NotImplementedException();
        }

        public void DeleteContactState(Guid guid)
        {
            throw new NotImplementedException();
        }

        public void DeleteEmploymentState(Guid entityGuid)
        {
            throw new NotImplementedException();
        }

        public void DeleteProductState(Guid guid)
        {
            throw new NotImplementedException();
        }

        public void DeleteProjectState(Guid guid)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Guid guid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICommandState> GetAllNew()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICommandState> GetAllProcessed()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICommandState> GetCommandStates(Guid entityGuid)
        {
            var states = new List<ICommandState>();
            var collection = _database.GetCollection<CommandState>("CommandStates");
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
            throw new NotImplementedException();
        }

        public IEnumerable<ICompanyState> GetCompanyStates()
        {
            throw new NotImplementedException();
        }

        public IContactState GetContactState(Guid guid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IContactState> GetContactStates()
        {
            throw new NotImplementedException();
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

        public IProductState GetProductState(Guid guid)
        {
            return new ProductState { Guid = Guid.NewGuid(), CreatedOn = DateTime.UtcNow, Name = "Dummy Product" };

            throw new NotImplementedException();
        }

        public IEnumerable<IProductState> GetProductStates()
        {
            var dummy = new ProductState { Guid = Guid.NewGuid(), CreatedOn = DateTime.UtcNow, Name = "Dummy Product" };
            return new List<IProductState> { dummy };
            //throw new NotImplementedException();
        }

        public IProjectState GetProjectState(Guid guid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IProjectState> GetProjectStates()
        {
            throw new NotImplementedException();
        }

        public IList<ICommandState> GetUpdatesSinceLast(long lastReceivedStamp)
        {
            throw new NotImplementedException();
        }

        public void PersistChanges()
        {
            var collection = _database.GetCollection<CommandState>("CommandStates");
            foreach (CommandState command in _commandStates.Values)
            {
                collection.InsertOne(command);
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

        public void SetProcessed(ICommandState state)
        {
            //throw new NotImplementedException();
        }
    }

}
