using niwrA.CommandManager;
using CompaniesShared;
using Moq;
using SoftwareManagementCoreTests.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Companies
{
  [Trait("Entity", "Hardware")]
  public class HardwareCommandsTests
  {
    [Fact(DisplayName = "RenameHardwareCommand")]
    public void RenameCommand()
    {
      var sutBuilder = new HardwareCommandBuilder<RenameCompanyEnvironmentHardwareCommand>();
      var sut = sutBuilder.Build() as RenameCompanyEnvironmentHardwareCommand;

      sut.Name = "New Name";
      sut.OriginalName = "Original Name";
      sut.Execute();

      sutBuilder.HardwareMock.Verify(s => s.Rename(sut.Name, sut.OriginalName), Times.Once);
    }
    [Fact(DisplayName = "ChangeIpAddressForHardwareCommand")]
    public void ChangeIpAddressForCommand()
    {
      var sutBuilder = new HardwareCommandBuilder<ChangeIpAddressForCompanyEnvironmentHardwareCommand>();
      var sut = sutBuilder.Build() as ChangeIpAddressForCompanyEnvironmentHardwareCommand;

      sut.IpAddress = "New IP";
      sut.OriginalIpAddress = "Original IP";
      sut.Execute();

      sutBuilder.HardwareMock.Verify(s => s.ChangeIpAddress(sut.IpAddress, sut.OriginalIpAddress), Times.Once);
    }
    [Fact(DisplayName = "AddHardwareToCompanyEnvironmentCommand")]
    public void AddHardwareToCompanyEnvironmentCommand()
    {
      var sutBuilder = new HardwareCommandBuilder<AddHardwareToCompanyEnvironmentCommand>();
      var sut = sutBuilder.Build() as AddHardwareToCompanyEnvironmentCommand;

      sut.HardwareName = "New Name";
      sut.EnvironmentGuid = Guid.NewGuid();
      sut.EntityGuid = Guid.NewGuid();
      sut.Execute();

      sutBuilder.CompanyEnvironmentMock.Verify(s => s.AddHardware(sut.EntityGuid, sut.HardwareName), Times.Once);
    }
    [Fact(DisplayName = "AddHardwareToCompanyEnvironmentCommand")]
    public void RemoveHardwareFromCompanyEnvironmentCommand()
    {
      var sutBuilder = new HardwareCommandBuilder<RemoveHardwareFromCompanyEnvironmentCommand>();
      var sut = sutBuilder.Build() as RemoveHardwareFromCompanyEnvironmentCommand;

      sut.EnvironmentGuid = Guid.NewGuid();
      sut.EntityGuid = Guid.NewGuid();
      sut.Execute();

      sutBuilder.CompanyEnvironmentMock.Verify(s => s.RemoveHardware(sut.EntityGuid), Times.Once);
    }

  }


  class HardwareCommandBuilder<T> where T : HardwareCommand, new()
  {
    public Mock<ICompany> CompanyMock { get; set; }
    public Mock<ICompanyEnvironment> CompanyEnvironmentMock { get; set; }
    public Mock<ICompanyEnvironmentHardware> HardwareMock { get; set; }
    public HardwareCommand Build()
    {
      var companiesMock = new Mock<ICompanyService>();
      var companyMock = new Mock<ICompany>();
      var companyEnvironmentMock = new Mock<ICompanyEnvironment>();
      var hardwareMock = new Mock<ICompanyEnvironmentHardware>();

      this.CompanyMock = companyMock;
      this.CompanyEnvironmentMock = companyEnvironmentMock;
      this.HardwareMock = hardwareMock;

      var sut = new CommandBuilder<T>().Build(companiesMock.Object) as HardwareCommand;

      companiesMock.Setup(s => s.GetCompany(It.IsAny<Guid>())).Returns(companyMock.Object);
      companyMock.Setup(s => s.GetEnvironment(It.IsAny<Guid>())).Returns(companyEnvironmentMock.Object);
      companyEnvironmentMock.Setup(s => s.GetHardware(It.IsAny<Guid>())).Returns(hardwareMock.Object);

      return sut;
    }
  }
}
