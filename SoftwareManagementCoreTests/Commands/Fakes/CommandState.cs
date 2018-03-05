using System;
using niwrA.CommandManager;

namespace SoftwareManagementCoreTests.Fakes
{
  public class CommandState : ICommandState
  {
    public string Command { get; set; }
    public string CommandVersion { get; set; }
    public Guid EntityGuid { get; set; }
    public Guid EntityRootGuid { get; set; }
    public string Entity { get; set; }
    public string EntityRoot { get; set; }
    public DateTime? ExecutedOn { get; set; }
    public Guid Guid { get; set; }
    public string ParametersJson { get; set; }
    public DateTime? ReceivedOn { get; set; }
    public DateTime CreatedOn { get; set; }
    public string UserName { get; set; }
  }
}
