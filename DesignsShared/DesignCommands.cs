using System;
using System.Collections.Generic;
using System.Text;
using CommandsShared;

namespace DesignsShared
{
    public abstract class DesignCommand : CommandBase
    {
        public DesignCommand() : base() { }
        public DesignCommand(ICommandStateRepository repo) : base(repo) { }
    }

    public class CreateDesignCommand : DesignCommand
    {
        public CreateDesignCommand() : base() { }
        public CreateDesignCommand(ICommandStateRepository repo) : base(repo) { }
        public string Name { get; set; }
        public override void Execute()
        {
            ((IDesignService)base.CommandProcessor).CreateDesign(this.EntityGuid, this.Name);
            base.Execute();
        }
    }

    public class RenameDesignCommand : DesignCommand
    {
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityGuid);
            design.Rename(this.Name, this.OriginalName);
            base.Execute();
        }
    }

    public class ChangeDescriptionOfDesignCommand : DesignCommand
    {
        public string Description { get; set; }
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityGuid);
            design.ChangeDescription(this.Description);
            base.Execute();
        }
    }
    public class DeleteDesignCommand : DesignCommand
    {
        public string Name { get; set; }
        public override void Execute()
        {
            ((IDesignService)base.CommandProcessor).DeleteDesign(this.EntityGuid);
            base.Execute();
        }
    }
    
}
