using System;
using CommandsShared;

namespace SoftwareManagementCoreTests.Fakes
{
    public class CommandState : ICommandState
    {
        public string CommandTypeId { get; set; }
        public Guid EntityGuid { get; set; }
        public DateTime? ExecutedOn { get; set; }
        public Guid Guid { get; set; }
        public string ParametersJson { get; set; }
        public DateTime? ReceivedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UserName { get; set; }
    }
}
