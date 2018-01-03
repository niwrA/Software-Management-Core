using CommandsShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompaniesShared
{
  public abstract class AccountCommand : EnvironmentCommand
  {
    public AccountCommand() : base() { }
    public AccountCommand(ICommandStateRepository repo) : base(repo) { }
    public Guid AccountGuid { get; set; }
  }
  public class AddAccountToCompanyEnvironmentCommand : AccountCommand
  {
    public string AccountName { get; set; }
    public override void Execute()
    {
      var company = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityGuid);
      var environment = company.GetEnvironment(this.EnvironmentGuid);
      environment.AddAccount(AccountGuid, AccountName);

      base.Execute();
    }
  }
  public class RenameCompanyEnvironmentAccountCommand : AccountCommand
  {
    public string OriginalName { get; set; }
    public string Name { get; set; }
    public override void Execute()
    {
      var root = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityGuid);
      var environment = root.GetEnvironment(EnvironmentGuid);
      var account = environment.GetAccount(this.AccountGuid);
      account.Rename(Name, this.OriginalName);
      base.Execute();
    }
  }
  public class RemoveAccountFromCompanyEnvironmentCommand : AccountCommand
  {
    public override void Execute()
    {
      var company = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityGuid);
      var environment = company.GetEnvironment(this.EnvironmentGuid);
      environment.RemoveAccount(this.AccountGuid);
      base.Execute();
    }
  }
}
