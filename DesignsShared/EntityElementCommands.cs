﻿using System;
using System.Collections.Generic;
using System.Text;
using niwrA.CommandManager;
using niwrA.CommandManager.Contracts;

namespace DesignsShared
{
    public abstract class EntityElementCommand : CommandBase, ICommand
    {
        public EntityElementCommand() : base() { }
        public EntityElementCommand(ICommandStateRepository repo) : base(repo) { }
        public Guid EpicElementGuid { get; set; }
        public new Guid EntityGuid { get { return System.Guid.Parse(base.EntityGuid); } }
        public new Guid EntityRootGuid { get { return System.Guid.Parse(base.EntityRootGuid); } }
        public virtual void Execute() { }
    }

    public class CreateEntityElementCommand : EntityElementCommand
    {
        public CreateEntityElementCommand() : base() { }
        public CreateEntityElementCommand(ICommandStateRepository repo) : base(repo) { }
        public string Name { get; set; }
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
            var epic = design.GetEpicElement(this.EpicElementGuid);
            epic.AddEntityElement(this.EntityGuid, this.Name, null);
            base.Execute();
        }
    }

    public class RenameEntityElementCommand : EntityElementCommand
    {
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
            var epic = design.GetEpicElement(this.EpicElementGuid);
            var entity = epic.GetEntityElement(this.EntityGuid);
            entity.Rename(this.Name, this.OriginalName);
            base.Execute();
        }
    }

    public class ChangeDescriptionOfEntityElementCommand : EntityElementCommand
    {
        public string Description { get; set; }
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
            var epic = design.GetEpicElement(this.EpicElementGuid);
            var entity = epic.GetEntityElement(this.EntityGuid);
            entity.ChangeDescription(this.Description);
            base.Execute();
        }
    }
    public class ChangePluralNameOfEntityElementCommand : EntityElementCommand
    {
        public string OriginalPluralName { get; set; }
        public string PluralName { get; set; }
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
            var epic = design.GetEpicElement(this.EpicElementGuid);
            var entity = epic.GetEntityElement(this.EntityGuid);
            entity.ChangePluralName(this.PluralName, this.OriginalPluralName);
            base.Execute();
        }
    }

    public class ChangeIsCollectionForEntityElementCommand : EntityElementCommand
    {
        public bool IsCollection { get; set; }
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
            var epic = design.GetEpicElement(this.EpicElementGuid);
            var entity = epic.GetEntityElement(this.EntityGuid);
            entity.ChangeIsCollection(this.IsCollection);
            base.Execute();
        }
    }

    public class DeleteEntityElementCommand : EntityElementCommand
    {
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
            var epic = design.GetEpicElement(this.EpicElementGuid);
            epic.DeleteEntityElement(this.EntityGuid);
            base.Execute();
        }
    }
    public class RemoveChildFromEntityElementCommand : EntityElementCommand
    {
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
            var epic = design.GetEpicElement(this.EpicElementGuid);
            epic.DeleteEntityElement(this.EntityGuid);
            base.Execute();
        }
    }

    public class AddChildToEntityElementCommand : EntityElementCommand
    {
        public AddChildToEntityElementCommand() : base() { }
        public string Name { get; set; }
        public Guid ParentGuid { get; set; }
        public override void Execute()
        {
            var design = ((IDesignService)base.CommandProcessor).GetDesign(this.EntityRootGuid);
            var epic = design.GetEpicElement(this.EpicElementGuid);
            epic.AddEntityElement(this.EntityGuid, this.Name, this.ParentGuid);
            base.Execute();
        }
    }

}
