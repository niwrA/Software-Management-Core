using CommandsShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompaniesShared
{
    public abstract class EnvironmentCommand : CompanyCommand
    {
        public EnvironmentCommand() : base() { }
        public EnvironmentCommand(ICommandStateRepository repo) : base(repo) { }
        public Guid EnvironmentGuid { get; set; }
    }

    public class RenameEnvironmentCommand : EnvironmentCommand
    {
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public override void Execute()
        {
            var root = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityGuid);
            var environment = root.GetEnvironment(EnvironmentGuid);
            environment.Rename(this.Name, this.OriginalName);
            base.Execute();
        }
    }

    public class ChangeUrlForEnvironmentCommand : EnvironmentCommand
    {
        public string OriginalUrl { get; set; }
        public string Url { get; set; }
        public override void Execute()
        {
            var root = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityGuid);
            var environment = root.GetEnvironment(EnvironmentGuid);
            environment.ChangeUrl(this.Url, this.OriginalUrl);
            base.Execute();
        }
    }
}
