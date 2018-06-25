using System;
using System.Collections.Generic;
using System.Text;
using niwrA.CommandManager;
using niwrA.CommandManager.Contracts;

namespace DesignsShared
{
    public abstract class CommandElementCommand : CommandBase, ICommand
    {
        public CommandElementCommand() : base() { }
        public CommandElementCommand(ICommandStateRepository repo) : base(repo) { }
        public Guid EpicElementGuid { get; set; }
        public Guid EntityElementGuid { get; set; }
        public new Guid EntityGuid { get { return System.Guid.Parse(base.EntityGuid); } }
        public new Guid EntityRootGuid { get { return System.Guid.Parse(base.EntityRootGuid); } }
        public virtual void Execute() { }
    }

    public class CreateCommandElementCommand : CommandElementCommand
    {
        public CreateCommandElementCommand() : base() { }
        public CreateCommandElementCommand(ICommandStateRepository repo) : base(repo) { }
        public string Name { get; set; }
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
            var epic = design.GetEpicElement(this.EpicElementGuid);
            var entity = epic.GetEntityElement(this.EntityElementGuid);
            entity.AddCommandElement(this.EntityGuid, this.Name);
            base.Execute();
        }
    }

    public class RenameCommandElementCommand : CommandElementCommand
    {
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
            var epic = design.GetEpicElement(this.EpicElementGuid);
            var entity = epic.GetEntityElement(this.EntityElementGuid);
            var property = entity.GetCommandElement(this.EntityGuid);
            property.Rename(this.Name, this.OriginalName);
            base.Execute();
        }
    }

    public class ChangeDescriptionOfCommandElementCommand : CommandElementCommand
    {
        public string Description { get; set; }
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
            var epic = design.GetEpicElement(this.EpicElementGuid);
            var entity = epic.GetEntityElement(this.EntityElementGuid);
            var property = entity.GetCommandElement(this.EntityGuid);
            property.ChangeDescription(this.Description);
            base.Execute();
        }
    }
    public class DeleteCommandElementCommand : CommandElementCommand
    {
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
            var epic = design.GetEpicElement(this.EpicElementGuid);
            var entity = epic.GetEntityElement(this.EntityElementGuid);
            entity.DeleteCommandElement(this.EntityGuid);
            base.Execute();
        }
    }
}
