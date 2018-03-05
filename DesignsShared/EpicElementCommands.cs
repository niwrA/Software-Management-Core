using System;
using System.Collections.Generic;
using System.Text;
using niwrA.CommandManager;

namespace DesignsShared
{
    public abstract class EpicElementCommand : CommandBase, ICommand
    {
        public EpicElementCommand() : base() { }
        public EpicElementCommand(ICommandStateRepository repo) : base(repo) { }
        public virtual void Execute() { }
  }

  public class CreateEpicElementCommand : EpicElementCommand
    {
        public CreateEpicElementCommand() : base() { }
        public CreateEpicElementCommand(ICommandStateRepository repo) : base(repo) { }
        public string Name { get; set; }
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
            design.AddEpicElement(this.EntityGuid, this.Name);
            base.Execute();
        }
    }

    public class RenameEpicElementCommand : EpicElementCommand
    {
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
            var epic = design.GetEpicElement(this.EntityGuid);
            epic.Rename(this.Name, this.OriginalName);
            base.Execute();
        }
    }

    public class ChangeDescriptionOfEpicElementCommand : EpicElementCommand
    {
        public string Description { get; set; }
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
            var epic = design.GetEpicElement(this.EntityGuid);
            epic.ChangeDescription(this.Description);
            base.Execute();
        }
    }
    public class DeleteEpicElementCommand : EpicElementCommand
    {
        public string Name { get; set; }
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
            design.DeleteEpicElement(this.EntityGuid);
            base.Execute();
        }
    }
}
