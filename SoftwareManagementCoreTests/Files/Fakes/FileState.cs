using FilesShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareManagementCoreTests.Files.Fakes
{
    public class FileState : IFileState
    {
        public FileState()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
            Guid = Guid.NewGuid();
            Name = "Fake File";
            EntityGuid = Guid.NewGuid();
            ForGuid = Guid.NewGuid();
            ForType = "fakes";
            SiteName = "Fake SiteName";
        }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public Guid EntityGuid { get; set; }
        public string FolderName { get { return ForType + @"/" + ForGuid; } }
        public Guid ForGuid { get; set; }
        public string ForType { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string SiteName { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
    }
}


