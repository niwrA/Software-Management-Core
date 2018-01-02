using CommandsShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompaniesShared
{
  public abstract class HardwareCommand : EnvironmentCommand
  {
    public HardwareCommand() : base() { }
    public HardwareCommand(ICommandStateRepository repo) : base(repo) { }
    public Guid HardwareGuid { get; set; }
  }
  public class AddHardwareToCompanyEnvironmentCommand : HardwareCommand
  {
    public string HardwareName { get; set; }
    public override void Execute()
    {
      var company = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityGuid);
      var environment = company.GetEnvironment(this.EnvironmentGuid);
      environment.AddHardware(HardwareGuid, HardwareName);

      base.Execute();
    }
  }
  public class RenameCompanyEnvironmentHardwareCommand : HardwareCommand
  {
    public string OriginalName { get; set; }
    public string Name { get; set; }
    public override void Execute()
    {
      var root = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityGuid);
      var environment = root.GetEnvironment(EnvironmentGuid);
      var hardware = environment.GetHardware(this.HardwareGuid);
      hardware.Rename(Name, this.OriginalName);
      base.Execute();
    }
  }
  public class ChangeIpAddressForCompanyEnvironmentHardwareCommand : HardwareCommand
  {
    public string OriginalIpAddress { get; set; }
    public string IpAddress { get; set; }
    public override void Execute()
    {
      var root = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityGuid);
      var environment = root.GetEnvironment(EnvironmentGuid);
      var hardware = environment.GetHardware(this.HardwareGuid);
      hardware.ChangeIpAddress(IpAddress, this.OriginalIpAddress);
      base.Execute();
    }
  }
  public class RemoveHardwareFromCompanyEnvironmentCommand : HardwareCommand
  {
    public override void Execute()
    {
      var company = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityGuid);
      var environment = company.GetEnvironment(this.EnvironmentGuid);
      environment.RemoveHardware(this.HardwareGuid);
      base.Execute();
    }
  }
}
