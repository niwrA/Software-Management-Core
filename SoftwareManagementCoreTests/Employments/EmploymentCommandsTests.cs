using niwrA.CommandManager;
using Moq;
using EmploymentsShared;
using SoftwareManagementCoreTests.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Employments
{
    [Trait("Entity", "Employment")]
    public class EmploymentCommandsTests
    {
        [Fact(DisplayName = "CreateEmploymentCommand")]
        public void CreateCommand()
        {
            var employmentsMock = new Mock<IEmploymentService>();
            var sut = new CommandBuilder<CreateEmploymentCommand>().Build(employmentsMock.Object) as CreateEmploymentCommand;

            sut.ContactGuid = Guid.NewGuid();
            sut.CompanyRoleGuid = Guid.NewGuid();
            sut.StartDate = DateTime.Now.Date;
            sut.EndDate = DateTime.Now.Date.AddYears(1);
            sut.Execute();

            employmentsMock.Verify(s => s.CreateEmployment(sut.EntityGuid, sut.ContactGuid, sut.CompanyRoleGuid, sut.StartDate, sut.EndDate, sut.ContactName), Times.Once);
        }

        // todo: duplicate check on Employments

        [Fact(DisplayName = "DeleteEmploymentCommand")]
        public void DeleteCommand()
        {
            var employmentsMock = new Mock<IEmploymentService>();
            var sut = new CommandBuilder<DeleteEmploymentCommand>().Build(employmentsMock.Object) as DeleteEmploymentCommand;

            sut.Execute();

            employmentsMock.Verify(s => s.DeleteEmployment(sut.EntityGuid), Times.Once);
        }

        [Fact(DisplayName = "ChangeStartDateOfEmploymentCommand")]
        public void ChangeEmploymentOfStartDateCommand()
        {
            var sutBuilder = new EmploymentCommandBuilder<ChangeStartDateOfEmploymentCommand>();
            var sut = sutBuilder.Build() as ChangeStartDateOfEmploymentCommand;

            sut.OriginalStartDate = DateTime.Now;
            sut.StartDate = DateTime.Now;
            sut.Execute();

            sutBuilder.EmploymentMock.Verify(s => s.ChangeStartDate(sut.StartDate, sut.OriginalStartDate), Times.Once);
        }

        [Fact(DisplayName = "ChangeEndDateOfEmploymentCommand")]
        public void ChangeEndDateOfEmploymentCommand()
        {
            var sutBuilder = new EmploymentCommandBuilder<ChangeEndDateOfEmploymentCommand>();
            var sut = sutBuilder.Build() as ChangeEndDateOfEmploymentCommand;

            sut.OriginalEndDate = DateTime.Now;
            sut.EndDate = DateTime.Now;
            sut.Execute();

            sutBuilder.EmploymentMock.Verify(s => s.ChangeEndDate(sut.EndDate, sut.OriginalEndDate), Times.Once);
        }
    }

    class EmploymentCommandBuilder<T> where T : ICommand, new()
    {
        public Mock<IEmployment> EmploymentMock { get; set; }
        public ICommand Build()
        {
            var employmentsMock = new Mock<IEmploymentService>();
            var employmentMock = new Mock<IEmployment>();
            this.EmploymentMock = employmentMock;

            var sut = new CommandBuilder<T>().Build(employmentsMock.Object);

            employmentsMock.Setup(s => s.GetEmployment(sut.EntityGuid)).Returns(employmentMock.Object);

            return sut;
        }
    }
}
