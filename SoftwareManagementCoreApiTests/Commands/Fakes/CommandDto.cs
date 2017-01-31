using CommandsShared;
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
            this.EntityGuid = Guid.NewGuid();
            this.Guid = Guid.NewGuid();
            this.Name = "Rename";
            this.ParametersJson = @"{OriginalName:'Old',Name:'New'}";
        }
    }
    public class RenameProductCommandDto : CommandDto
    {
        public RenameProductCommandDto()
        {
            this.CreatedOn = DateTime.Now;
            this.Entity = "Product";
            this.EntityGuid = Guid.NewGuid();
            this.Guid = Guid.NewGuid();
            this.Name = "Rename";
            this.ParametersJson = @"{OriginalName:'Old',Name:'New'}";
        }
    }
}
