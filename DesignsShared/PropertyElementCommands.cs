using System;
using System.Collections.Generic;
using System.Text;
using CommandsShared;

namespace DesignsShared
{
    public abstract class PropertyElementCommand : CommandBase
    {
        public PropertyElementCommand() : base() { }
        public PropertyElementCommand(ICommandStateRepository repo) : base(repo) { }
        public Guid DesignGuid { get; set; }
        public Guid EpicElementGuid { get; set; }
        public Guid EntityElementGuid { get; set; }
    }

    public class CreatePropertyElementCommand : PropertyElementCommand
    {
        public CreatePropertyElementCommand() : base() { }
        public CreatePropertyElementCommand(ICommandStateRepository repo) : base(repo) { }
        public string Name { get; set; }
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.DesignGuid);
            var epic = design.GetEpicElement(this.EpicElementGuid);
            var entity = epic.GetEntityElement(this.EntityElementGuid);
            entity.AddPropertyElement(this.EntityGuid, this.Name);
            base.Execute();
        }
    }

    public class RenamePropertyElementCommand : PropertyElementCommand
    {
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.DesignGuid);
            var epic = design.GetEpicElement(this.EpicElementGuid);
            var entity = epic.GetEntityElement(this.EntityElementGuid);
            var property = entity.GetPropertyElement(this.EntityGuid);
            property.Rename(this.Name, this.OriginalName);
            base.Execute();
        }
    }

    public class ChangeDescriptionOfPropertyElementCommand : PropertyElementCommand
    {
        public string Description { get; set; }
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.DesignGuid);
            var epic = design.GetEpicElement(this.EpicElementGuid);
            var entity = epic.GetEntityElement(this.EntityElementGuid);
            var property = entity.GetPropertyElement(this.EntityGuid);
            property.ChangeDescription(this.Description);
            base.Execute();
        }
    }
    public class DeletePropertyElementCommand : PropertyElementCommand
    {
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.DesignGuid);
            var epic = design.GetEpicElement(this.EpicElementGuid);
            var entity = epic.GetEntityElement(this.EntityElementGuid);
            entity.DeletePropertyElement(this.EntityGuid);
            base.Execute();
        }
    }
}
