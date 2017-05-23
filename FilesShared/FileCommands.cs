using System;
using System.Collections.Generic;
using System.Text;
using CommandsShared;

namespace FilesShared
{
    public abstract class FileCommand : CommandBase
    {
        public FileCommand() : base() { }
        public FileCommand(ICommandStateRepository repo) : base(repo) { }
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
        public Guid ForGuid { get; set; }
        public override void Execute()
        {
            ((IFileService)base.CommandProcessor).CreateFile(EntityGuid, ForGuid, FolderName, Name);
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

    public class ChangeUrlForFileCommand : FileCommand
    {
        public string OriginalUrl { get; set; }
        public string Url { get; set; }
        public override void Execute()
        {
            var product = ((IFileService)base.CommandProcessor).GetFile(this.EntityGuid);
            product.MoveToFolder(this.Url, this.OriginalUrl);
            base.Execute();
        }
    }


}
