﻿using niwrA.CommandManager;
using ContactsShared;
using System;
using System.Collections.Generic;
using System.Text;
using niwrA.CommandManager.Contracts;

namespace EmploymentsShared
{
    public interface IEmploymentState: ITimeStampedEntityState
    {
        Guid ContactGuid { get; set; }
        Guid CompanyRoleGuid { get; set; }
        string ContactName { get; set; }
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
    }
    public interface IEmployment
    {
        Guid Guid { get; }
        Guid ContactGuid { get; }
        Guid CompanyRoleGuid { get; }
        DateTime? StartDate { get; }
        DateTime? EndDate { get; }

        void ChangeStartDate(DateTime? startDate, DateTime? originalStartDate);
        void ChangeEndDate(DateTime? endDate, DateTime? originalEndDate);
        void Terminate(DateTime endDate);
    }
    public interface IEmploymentStateRepository
    {
        IEmploymentState CreateEmploymentState(Guid guid, Guid contactGuid, Guid companyRoleGuid);
        IEmploymentState GetEmploymentState(Guid guid);
        IEnumerable<IEmploymentState> GetEmploymentsByCompanyRoleGuid(Guid companyRoleGuid);
        IEnumerable<IEmploymentState> GetEmploymentsByContactGuid(Guid contactGuid);
        IEnumerable<IContactState> GetContactsByCompanyRoleGuid(Guid companyRoleGuid);
        void DeleteEmploymentState(Guid entityGuid);
        IEnumerable<IEmploymentState> GetEmploymentStates();
        void PersistChanges();
    }
    public class Employment : IEmployment
    {
        IEmploymentState _state;
        IEmploymentStateRepository _repo;

        public Guid Guid { get { return _state.Guid; } }
        public Guid ContactGuid { get { return _state.ContactGuid; } }
        public Guid CompanyRoleGuid { get { return _state.CompanyRoleGuid; } }
        public DateTime? StartDate { get { return _state.StartDate; } }
        public DateTime? EndDate { get { return _state.EndDate; } }
        public string ContactName { get { return _state.ContactName; } }

        public Employment(IEmploymentState state)
        {
            _state = state;
        }
        public Employment(IEmploymentState state, IEmploymentStateRepository repo) : this(state)
        {
            _repo = repo;
        }

        public void ChangeStartDate(DateTime? startDate, DateTime? originalStartDate)
        {
            if (_state.EndDate == originalStartDate)
            {
                _state.EndDate = startDate;
            }
        }

        public void ChangeEndDate(DateTime? endDate, DateTime? originalEndDate)
        {
            if (_state.EndDate == originalEndDate)
            {
                _state.EndDate = endDate;
            }
        }

        public void RenameContact(string name, string originalName)
        {
            if(_state.ContactName == originalName)
            {
                _state.ContactName = name;
            }
        }

        public void Terminate(DateTime endDate)
        {
            _state.EndDate = endDate;
        }
    }

    public interface IEmploymentService : ICommandProcessor
    {
        IEmployment CreateEmployment(Guid guid, Guid contactGuid, Guid companyGuid, DateTime? startDate, DateTime? endDate, string contactName);
        void DeleteEmployment(Guid entityGuid);
        IEmployment GetEmployment(Guid guid);
        void PersistChanges();
    }

    public class EmploymentService : IEmploymentService
    {
        private IEmploymentStateRepository _repo;
        public EmploymentService(IEmploymentStateRepository repo)
        {
            _repo = repo;
        }
        public IEmployment CreateEmployment(Guid guid, Guid contactGuid, Guid companyRoleGuid, DateTime? startDate, DateTime? endDate, string contactName)
        {
            var state = _repo.CreateEmploymentState(guid, contactGuid, companyRoleGuid);
            state.StartDate = startDate;
            state.EndDate = endDate;
            state.ContactName = contactName;
            return new Employment(state, _repo);
        }

        public void DeleteEmployment(Guid entityGuid)
        {
            _repo.DeleteEmploymentState(entityGuid);
        }

        public IEmployment GetEmployment(Guid guid)
        {
            var state = _repo.GetEmploymentState(guid);
            return new Employment(state, _repo);
        }
        public void PersistChanges()
        {
            _repo.PersistChanges();
        }
    }
}
