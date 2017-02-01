using CommandsShared;
using DateTimeShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompaniesShared
{
    public interface ICompanyService : ICommandProcessor
    {
        ICompany CreateCompany(Guid guid, string name);
        ICompany GetCompany(Guid guid);
        void DeleteCompany(Guid guid);
        void PersistChanges();
    }

    public interface ICompanyState : IEntityState
    {
    }

    public interface ICompanyStateRepository : IEntityRepository
    {
        ICompanyState CreateCompanyState(Guid guid, string name);
        ICompanyState GetCompanyState(Guid guid);
        IEnumerable<ICompanyState> GetCompanyStates();
        void DeleteCompanyState(Guid guid);
    }
    public interface IEntity
    {
        Guid Guid { get; }
        string Name { get; }
        DateTime CreatedOn { get; }
        void Rename(string name, string originalName);
    }
    public interface ICompany : IEntity
    {
    }
    public class Company : ICompany
    {
        private ICompanyState _state;
        private ICompanyStateRepository _repo;

        public Company(ICompanyState state)
        {
            _state = state;
        }
        public Company(ICompanyState state, ICompanyStateRepository repo) : this(state)
        {
            _repo = repo;
        }

        public Guid Guid { get { return _state.Guid; } }
        public string Name { get { return _state.Name; } }
        public DateTime CreatedOn { get { return _state.CreatedOn; } }

        public void Rename(string name, string originalName)
        {
            if (_state.Name == originalName)
            {
                _state.Name = name;
            }
            else
            {
                // todo: implement concurrency policy
            }
        }
    }
    public class CompanyService : ICompanyService
    {
        private IDateTimeProvider _dateTimeProvider;
        private ICompanyStateRepository _repo;
        public CompanyService(ICompanyStateRepository repo, IDateTimeProvider dateTimeProvider)
        {
            _repo = repo;
            _dateTimeProvider = dateTimeProvider;
        }
        public ICompany CreateCompany(Guid guid, string name)
        {
            var state = _repo.CreateCompanyState(guid, name);
            state.CreatedOn = _dateTimeProvider.GetUtcDateTime();
            state.UpdatedOn = _dateTimeProvider.GetUtcDateTime();
            state.Name = name;
            return new Company(state);
        }
        public ICompany GetCompany(Guid guid)
        {
            var state = _repo.GetCompanyState(guid);
            return new Company(state, _repo);
        }
        public void DeleteCompany(Guid guid)
        {
            _repo.DeleteCompanyState(guid);
        }

        public void PersistChanges()
        {
            _repo.PersistChanges();
        }
    }
    public class CompanyBuilder
    {
        private CompanyService _companies;
        private Guid _guid;
        private string _name;

        public CompanyBuilder(CompanyService companies)
        {
            _companies = companies;
        }

        public ICompany Build(string name)
        {
            EnsureGuid();
            var company = _companies.CreateCompany(_guid, name);
            return company;
        }

        private void EnsureGuid()
        {
            if (_guid == null || _guid == Guid.Empty)
            {
                _guid = Guid.NewGuid();
            }
        }

        public CompanyBuilder WithGuid(Guid guid)
        {
            _guid = guid;
            return this;
        }

        public CompanyBuilder WithName(string name)
        {
            _name = name;
            return this;
        }
    }
}
