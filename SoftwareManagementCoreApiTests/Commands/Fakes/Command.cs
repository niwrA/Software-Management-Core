using niwrA.CommandManager;
using SoftwareManagementCoreApiTests.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareManagementCoreApiTests.Fakes
{
  public class CommandState : ICommandState
  {
    public Guid Guid { get; set; }
    public Guid EntityGuid { get; set; }
    public string Entity { get; set; }
    public Guid EntityRootGuid { get; set; }
    public string EntityRoot { get; set; }
    public string Command { get; set; }
    public string CommandVersion { get; set; }
    public string ParametersJson { get; set; }
    public DateTime? ExecutedOn { get; set; }
    public DateTime? ReceivedOn { get; set; }
    public DateTime CreatedOn { get; set; }
    public string UserName { get; set; }
  }
  public class RenameProductCommand : ProductsShared.RenameProductCommand
  {
    public RenameProductCommand(RenameProductCommandDto dto, ICommandStateRepository repo)
    {
      this.CommandRepository = repo;
      this.CreatedOn = dto.CreatedOn;
      this.EntityGuid = dto.EntityGuid;
      this.Command = dto.Command;
      this.Entity = dto.Entity;
      this.Name = "new";
      this.OriginalName = "old";
      this.ReceivedOn = DateTime.Now;
    }
  }
  public class RenameProjectCommand : ProjectsShared.RenameProjectCommand
  {
    public RenameProjectCommand(RenameProjectCommandDto dto, ICommandStateRepository repo)
    {
      this.CommandRepository = repo;
      this.CreatedOn = dto.CreatedOn;
      this.EntityGuid = dto.EntityGuid;
      this.Command = dto.Command;
      this.Entity = dto.Entity;
      this.Name = "new";
      this.OriginalName = "old";
      this.ReceivedOn = DateTime.Now;
    }
  }
}
