using EmploymentsShared;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Employments
{
    [Trait("Category", "Employment")]
    public class EmploymentTests
    {
        [Fact(DisplayName = "CanCreate")]
        public void CanCreateEmployment()
        {
            var sutBuilder = new EmploymentSutBuilder();
            var sut = sutBuilder.Build();

            Guid guid = Guid.NewGuid();
            Guid contactGuid = Guid.NewGuid();
            Guid companyRoleGuid = Guid.NewGuid();

            var startDate = DateTime.Now.Date as DateTime?;
            var employment = sut.CreateEmployment(guid, contactGuid, companyRoleGuid, startDate, null);

            sutBuilder.RepoMock.Verify(v => v.CreateEmploymentState(guid, contactGuid, companyRoleGuid), Times.Once);
        }

        [Fact(DisplayName = "ReflectsState")]
        public void EmploymentReflectsState()
        {
            var state = new Fakes.EmploymentState();
            var sut = new Employment(state);

            Assert.Equal(state.Guid, sut.Guid);
            Assert.Equal(state.ContactGuid, sut.ContactGuid);
            Assert.Equal(state.CompanyRoleGuid, sut.CompanyRoleGuid);
            Assert.Equal(state.StartDate, sut.StartDate);
            Assert.Equal(state.EndDate, sut.EndDate);

        }

        public class EmploymentSutBuilder
        {
            Mock<IEmploymentStateRepository> _repo;
            public Mock<IEmploymentStateRepository> RepoMock { get { return _repo; } }
            public EmploymentService Build()
            {
                _repo = new Mock<IEmploymentStateRepository>();
                _repo.Setup(s => s.CreateEmploymentState(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(new Fakes.EmploymentState());
                var sut = new EmploymentService(_repo.Object);
                return sut;
            }
        }
    }
}
