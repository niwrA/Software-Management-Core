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
