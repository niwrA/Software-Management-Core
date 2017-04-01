using CompaniesShared;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Companies
{
    [Trait("Entity", "Environment")]
    public class EnvironmentsTests
    {
        [Fact(DisplayName = "Rename")]
        public void CanRenameCompany()
        {
            var stateMock = new Mock<ICompanyEnvironmentState>();
            var sut = new CompanyEnvironment(stateMock.Object);

            stateMock.Setup(s => s.Name).Returns("old");

            sut.Rename("new", "old");

            stateMock.VerifySet(t => t.Name = "new");
        }
    }
}
