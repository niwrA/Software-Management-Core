using niwrA.CommandManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareManagementCoreApiTests.Fakes
{
    // todo: these could be builders, but may not be necessary
    public class RenameProjectCommandDto : CommandDto
    {
        public RenameProjectCommandDto()
        {
            this.CreatedOn = DateTime.Now;
            this.Entity = "Project";
            this.EntityGuid = System.Guid.NewGuid().ToString();
            this.Guid = System.Guid.NewGuid();
            this.Command = "Rename";
            this.ParametersJson = @"{OriginalName:'Old',Name:'New'}";
        }
    }
    public class RenameProductCommandDto : CommandDto
    {
        public RenameProductCommandDto()
        {
            this.CreatedOn = DateTime.Now;
            this.Entity = "Product";
            this.EntityGuid = System.Guid.NewGuid().ToString();
            this.Guid = System.Guid.NewGuid();
            this.Command = "Rename";
            this.ParametersJson = @"{OriginalName:'Old',Name:'New'}";
        }
    }
}
