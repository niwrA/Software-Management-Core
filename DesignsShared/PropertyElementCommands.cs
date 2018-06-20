using System;
using System.Collections.Generic;
using System.Text;
using niwrA.CommandManager;

namespace DesignsShared
{
    public abstract class PropertyElementCommand : CommandBase, ICommand
    {
        public PropertyElementCommand() : base() { }
        public PropertyElementCommand(ICommandStateRepository repo) : base(repo) { }
        public Guid EpicElementGuid { get; set; }
        public Guid EntityElementGuid { get; set; }
        public virtual void Execute() { }
    }

    public class CreatePropertyElementCommand : PropertyElementCommand
    {
        public CreatePropertyElementCommand() : base() { }
        public CreatePropertyElementCommand(ICommandStateRepository repo) : base(repo) { }
        public string Name { get; set; }
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
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
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
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
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
            var epic = design.GetEpicElement(this.EpicElementGuid);
            var entity = epic.GetEntityElement(this.EntityElementGuid);
            var property = entity.GetPropertyElement(this.EntityGuid);
            property.ChangeDescription(this.Description);
            base.Execute();
        }
    }

    public class ChangeDataTypeOfPropertyElementCommand : PropertyElementCommand
    {
        public string OriginalDataType { get; set; }
        public string DataType { get; set; }
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
            var epic = design.GetEpicElement(this.EpicElementGuid);
            var entity = epic.GetEntityElement(this.EntityElementGuid);
            var property = entity.GetPropertyElement(this.EntityGuid);
            property.ChangeDataType(this.DataType, this.OriginalDataType);
            base.Execute();
        }
    }
    public class ChangeIsStatePropertyElementCommand : PropertyElementCommand
    {
        public bool IsState { get; set; }
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
            var epic = design.GetEpicElement(this.EpicElementGuid);
            var entity = epic.GetEntityElement(this.EntityElementGuid);
            var property = entity.GetPropertyElement(this.EntityGuid);
            property.ChangeIsState(this.IsState);
            base.Execute();
        }
    }

    public class DeletePropertyElementCommand : PropertyElementCommand
    {
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
            var epic = design.GetEpicElement(this.EpicElementGuid);
            var entity = epic.GetEntityElement(this.EntityElementGuid);
            entity.DeletePropertyElement(this.EntityGuid);
            base.Execute();
        }
    }
}
