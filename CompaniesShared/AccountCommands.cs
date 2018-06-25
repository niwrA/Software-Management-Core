using niwrA.CommandManager;
using niwrA.CommandManager.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompaniesShared
{
    public abstract class AccountCommand : EnvironmentCommand
    {
        public AccountCommand() : base() { }
        public AccountCommand(ICommandStateRepository repo) : base(repo) { }
        public Guid EnvironmentGuid { get; set; }
    }
    public class AddCompanyEnvironmentAccountCommand : AccountCommand
    {
        public string AccountName { get; set; }
        public override void Execute()
        {
            var company = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityRootGuid);
            var environment = company.GetEnvironment(this.EnvironmentGuid);
            environment.AddAccount(this.EntityGuid, AccountName);

            base.Execute();
        }
    }
    public class RenameCompanyEnvironmentAccountCommand : AccountCommand
    {
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public override void Execute()
        {
            var root = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityRootGuid);
            var environment = root.GetEnvironment(EnvironmentGuid);
            var account = environment.GetAccount(this.EntityGuid);
            account.Rename(Name, this.OriginalName);
            base.Execute();
        }
    }
    public class RemoveCompanyEnvironmentAccountCommand : AccountCommand
    {
        public override void Execute()
        {
            var company = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityRootGuid);
            var environment = company.GetEnvironment(this.EnvironmentGuid);
            environment.RemoveAccount(this.EntityGuid);
            base.Execute();
        }
    }
}
