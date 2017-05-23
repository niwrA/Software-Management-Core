﻿using CommandsShared;
using Moq;
using FilesShared;
using SoftwareManagementCoreTests.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.Files
{
    [Trait("Entity", "File")]
    public class CompanyCommandsTests
    {
        [Fact(DisplayName = "CreateFileCommand")]
        public void CreateCommand()
        {
            var linksMock = new Mock<IFileService>();
            var sut = new CommandBuilder<CreateFileCommand>().Build(linksMock.Object) as CreateFileCommand;

            sut.Name = "New File";
            sut.FolderName = "http://somewhere.nice";
            sut.ForGuid = Guid.NewGuid();
            sut.Execute();

            linksMock.Verify(s => s.CreateFile(sut.EntityGuid, sut.ForGuid, sut.FolderName, sut.Name), Times.Once);
        }

        [Fact(DisplayName = "DeleteFileCommand")]
        public void DeleteCommand()
        {
            var linksMock = new Mock<IFileService>();
            var sut = new CommandBuilder<DeleteFileCommand>().Build(linksMock.Object) as DeleteFileCommand;

            sut.Execute();

            linksMock.Verify(s => s.DeleteFile(sut.EntityGuid), Times.Once);
        }

        [Fact(DisplayName = "RenameFileCommand")]
        public void RenameCommand()
        {
            var sutBuilder = new FileCommandBuilder<RenameFileCommand>();
            var sut = sutBuilder.Build() as RenameFileCommand;

            sut.Name = "New Name";
            sut.OriginalName = "Original Name";
            sut.Execute();

            sutBuilder.FileMock.Verify(s => s.Rename(sut.Name, sut.OriginalName), Times.Once);
        }

        [Fact(DisplayName = "ChangeUrlForFileCommand")]
        public void ChangeUrlForFileCommand()
        {
            var sutBuilder = new FileCommandBuilder<ChangeUrlForFileCommand>();
            var sut = sutBuilder.Build() as ChangeUrlForFileCommand;

            sut.Url = "New";
            sut.OriginalUrl = "Original";
            sut.Execute();

            sutBuilder.FileMock.Verify(s => s.MoveToFolder(sut.Url, sut.OriginalUrl), Times.Once);
        }
    }

    class FileCommandBuilder<T> where T : ICommand, new()
    {
        public Mock<IFile> FileMock { get; set; }
        public ICommand Build()
        {
            var linksMock = new Mock<IFileService>();
            var linkMock = new Mock<IFile>();
            this.FileMock = linkMock;

            var sut = new CommandBuilder<T>().Build(linksMock.Object);

            linksMock.Setup(s => s.GetFile(sut.EntityGuid)).Returns(linkMock.Object);

            return sut;
        }
    }
}
