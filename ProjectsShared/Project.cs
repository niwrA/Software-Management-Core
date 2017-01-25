using System;

namespace ProjectsShared
{
    public class Project
    {
        private IProjectState _state;
        public Project(IProjectState state)
        {
            _state = state;
        }

        public Guid Guid { get { return _state.Guid; } }
        public string Name { get { return _state.Name; } }
        public DateTime CreatedOn { get { return _state.CreatedOn; } }

        public void Rename(string name)
        {
            _state.Name = name;
        }

        public void ChangeStartDate(DateTime? startDate)
        {
            _state.StartDate = startDate;
        }

        public void ChangeEndDate(DateTime? endDate)
        {
            _state.EndDate = endDate;
        }
    }
}
