using CommandsShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmploymentsShared
{
    public interface IEmploymentState
    {
        Guid Guid { get; set; }
        Guid ContactGuid { get; set; }
        Guid CompanyRoleGuid { get; set; }
        DateTime StartDate { get; set; }
        DateTime? EndDate { get; set; }
    }
    public interface IEmployment
    {
        void ChangeStartDate(DateTime? startDate, DateTime? originalStartDate);
        void ChangeEndDate(DateTime? endDate, DateTime? originalEndDate);
        void Terminate(DateTime endDate);
    }
    public interface IEmploymentRepository
    {
        IEmploymentState CreateEmploymentState(Guid guid, Guid contactGuid, Guid companyRoleGuid);
        IEmploymentState GetEmploymentState(Guid guid);
        ICollection<Guid> GetContactGuidsByCompanyRole(Guid companyRoleGuid);
        ICollection<Guid> GetCompanyRoleGuidsByContact(Guid contactGuid);
    }
    public class Employment: IEmployment
    {
        IEmploymentState _state;
        IEmploymentRepository _repo;
        public Employment(IEmploymentState state, IEmploymentRepository repo)
        {
            _state = state;
            _repo = repo;
        }

        public void ChangeStartDate(DateTime? startDate, DateTime? originalStartDate)
        {
            if(_state.EndDate == originalStartDate)
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

        public void Terminate(DateTime endDate)
        {
            _state.EndDate = endDate;
        }

    }
    public interface IEmployments : ICommandProcessor
    {
        IEmployment CreateEmployment(Guid guid, Guid contactGuid, Guid companyGuid, DateTime startDate, DateTime? endDate);
        IEmployment GetEmployment(Guid guid);
    }
    public class Employments: IEmployments
    {
        private IEmploymentRepository _repo;
        public Employments(IEmploymentRepository repo)
        {
            _repo = repo;
        }
        public IEmployment CreateEmployment(Guid guid, Guid contactGuid, Guid companyRoleGuid, DateTime startDate, DateTime? endDate)
        {
            var state = _repo.CreateEmploymentState(guid, contactGuid, companyRoleGuid);
            state.StartDate = startDate;
            state.EndDate = endDate;
            return new Employment(state,_repo);
        }
        public IEmployment GetEmployment(Guid guid)
        {
            var state = _repo.GetEmploymentState(guid);
            return new Employment(state, _repo);
        }
    }
}
