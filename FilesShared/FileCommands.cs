using System;
using System.Collections.Generic;
using System.Text;
using niwrA.CommandManager;
using niwrA.CommandManager.Contracts;

namespace FilesShared
{
    public abstract class FileCommand : CommandBase, ICommand
    {
        public FileCommand() : base() { }
        public FileCommand(ICommandStateRepository repo) : base(repo) { }
        public new Guid EntityGuid { get { return System.Guid.Parse(base.EntityGuid); } }
        public new Guid EntityRootGuid { get { return System.Guid.Parse(base.EntityRootGuid); } }
        public virtual void Execute() { }
    }

    public class DeleteFileCommand : FileCommand
    {
        public override void Execute()
        {
            ((IFileService)base.CommandProcessor).DeleteFile(this.EntityGuid);
            base.Execute();
        }

    }

    public class CreateFileCommand : FileCommand
    {
        public string FolderName { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public Guid ForGuid { get; set; }
        public string ForType { get; set; }
        public string Type { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public override void Execute()
        {
            ((IFileService)base.CommandProcessor).CreateFile(EntityGuid, ForGuid, ForType, Name, FileName, Type, ContentType, Size);
            base.Execute();
        }
    }

    public class RenameFileCommand : FileCommand
    {
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public override void Execute()
        {
            var product = ((IFileService)base.CommandProcessor).GetFile(this.EntityGuid);
            product.Rename(this.Name, this.OriginalName);
            base.Execute();
        }
    }

}
