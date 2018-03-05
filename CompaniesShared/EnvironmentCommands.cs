using niwrA.CommandManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompaniesShared
{
    public abstract class EnvironmentCommand : CompanyCommand
    {
        public EnvironmentCommand() : base() { }
        public EnvironmentCommand(ICommandStateRepository repo) : base(repo) { }
    }

    public class RenameCompanyEnvironmentCommand : EnvironmentCommand
    {
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public override void Execute()
        {
            var root = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityRootGuid);
            var environment = root.GetEnvironment(this.EntityGuid);
            environment.Rename(this.Name, this.OriginalName);
            base.Execute();
        }
    }

    public class ChangeUrlForCompanyEnvironmentCommand : EnvironmentCommand
    {
        public string OriginalUrl { get; set; }
        public string Url { get; set; }
        public override void Execute()
        {
            var root = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityRootGuid);
            var environment = root.GetEnvironment(this.EntityGuid);
            environment.ChangeUrl(this.Url, this.OriginalUrl);
            base.Execute();
        }
    }
}
