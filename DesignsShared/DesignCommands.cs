using System;
using System.Collections.Generic;
using System.Text;
using niwrA.CommandManager;
using niwrA.CommandManager.Contracts;

namespace DesignsShared
{
    public abstract class DesignCommand : CommandBase, ICommand
    {
        public DesignCommand() : base() { }
        public DesignCommand(ICommandStateRepository repo) : base(repo) { }
        public new Guid EntityGuid { get { return System.Guid.Parse(base.EntityGuid); } }
        public new Guid EntityRootGuid { get { return System.Guid.Parse(base.EntityRootGuid); } }
        public virtual void Execute() { }
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
