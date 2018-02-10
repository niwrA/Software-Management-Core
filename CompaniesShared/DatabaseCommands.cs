using niwrA.CommandManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompaniesShared
{
  public abstract class DatabaseCommand : EnvironmentCommand
  {
    public DatabaseCommand() : base() { }
    public DatabaseCommand(ICommandStateRepository repo) : base(repo) { }
    public Guid DatabaseGuid { get; set; }
  }
  public class AddDatabaseToCompanyEnvironmentCommand : DatabaseCommand
  {
    public string DatabaseName { get; set; }
    public override void Execute()
    {
      var company = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityGuid);
      var environment = company.GetEnvironment(this.EnvironmentGuid);
      environment.AddDatabase(DatabaseGuid, DatabaseName);

      base.Execute();
    }
  }
  public class RenameCompanyEnvironmentDatabaseCommand : DatabaseCommand
  {
    public string OriginalName { get; set; }
    public string Name { get; set; }
    public override void Execute()
    {
      var root = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityGuid);
      var environment = root.GetEnvironment(EnvironmentGuid);
      var database = environment.GetDatabase(this.DatabaseGuid);
      database.Rename(Name, this.OriginalName);
      base.Execute();
    }
  }
  public class RemoveDatabaseFromCompanyEnvironmentCommand : DatabaseCommand
  {
    public override void Execute()
    {
      var company = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityGuid);
      var environment = company.GetEnvironment(this.EnvironmentGuid);
      environment.RemoveDatabase(this.DatabaseGuid);
      base.Execute();
    }
  }
}
