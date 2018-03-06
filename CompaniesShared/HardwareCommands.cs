using niwrA.CommandManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompaniesShared
{
    public abstract class HardwareCommand : EnvironmentCommand
    {
        public HardwareCommand() : base() { }
        public HardwareCommand(ICommandStateRepository repo) : base(repo) { }
        public Guid EnvironmentGuid { get; set; }
    }
    public class AddCompanyEnvironmentHardwareCommand : HardwareCommand
    {
        public string HardwareName { get; set; }
        public override void Execute()
        {
            var company = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityRootGuid);
            var environment = company.GetEnvironment(this.EnvironmentGuid);
            environment.AddHardware(EntityGuid, HardwareName);

            base.Execute();
        }
    }
    public class RemoveCompanyEnvironmentHardwareCommand : HardwareCommand
    {
        public override void Execute()
        {
            var company = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityRootGuid);
            var environment = company.GetEnvironment(this.EnvironmentGuid);
            environment.RemoveHardware(this.EntityGuid);
            base.Execute();
        }
    }
    public class RenameCompanyEnvironmentHardwareCommand : HardwareCommand
    {
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public override void Execute()
        {
            var root = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityRootGuid);
            var environment = root.GetEnvironment(EnvironmentGuid);
            var hardware = environment.GetHardware(this.EntityGuid);
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
        var hardware = environment.GetHardware(this.EntityGuid);
        hardware.ChangeIpAddress(IpAddress, this.OriginalIpAddress);
        base.Execute();
      }
    }
}
