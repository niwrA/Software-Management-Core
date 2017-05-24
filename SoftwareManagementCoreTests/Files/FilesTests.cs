using FilesShared;
using System;
using Xunit;
using Moq;
using DateTimeShared;

namespace SoftwareManagementCoreTests
{
    [Trait("Entity", "File")]
    public class FilesTests
    {
        [Fact(DisplayName = "Create")]
        public void CreateFile_ImplementsIRepository()
        {
            var repoMock = new Moq.Mock<IFileStateRepository>();
            var sut = new FileService(repoMock.Object, new DateTimeProvider());
            var stateMock = new Mock<IFileState>();
 
            repoMock.Setup(t => t.CreateFileState(It.IsAny<Guid>(), It.IsAny<string>())).Returns(stateMock.Object);

            var guid = Guid.NewGuid();
            var name = "New File";
            var folder = "New Folder";
            var forGuid = Guid.NewGuid();
            var type = ".jpg";
            sut.CreateFile(guid, forGuid, folder, name, name, type);

            repoMock.Verify(s => s.CreateFileState(guid, name), Times.Once);
        }

        [Fact(DisplayName = "Delete")]
        public void CanDeleteFile()
        {
            var repoMock = new Mock<IFileStateRepository>();
            var sut = new FileService(repoMock.Object, new DateTimeProvider());
            var guid = Guid.NewGuid();

            sut.DeleteFile(guid);

            repoMock.Verify(s => s.DeleteFileState(guid));
        }

        [Fact(DisplayName = "Get")]
        public void CanGetFile()
        {
            var repoMock = new Mock<IFileStateRepository>();
            var sut = new FileService(repoMock.Object, new DateTimeProvider());
            var stateMock = new Mock<IFileState>();
            var stateMockAlt = new Mock<IFileState>();

            var stateGuid = Guid.NewGuid();
            var altStateGuid = Guid.NewGuid();

            repoMock.Setup(t => t.GetFileState(stateGuid)).Returns(stateMock.Object);
            repoMock.Setup(t => t.GetFileState(altStateGuid)).Returns(stateMockAlt.Object);

            sut.GetFile(stateGuid);
            repoMock.Verify(t => t.GetFileState(stateGuid));

            sut.GetFile(altStateGuid);
            repoMock.Verify(t => t.GetFileState(altStateGuid));
        }

        [Fact(DisplayName = "Rename")]
        public void CanRenameFile()
        {
            var stateMock = new Mock<IFileState>();
            var sut = new File(stateMock.Object);

            stateMock.Setup(s => s.Name).Returns("old");

            sut.Rename("new", "old");

            stateMock.VerifySet(t => t.Name = "new");
        }
    }
    public class FileServiceSutBuilder
    {
        public Mock<IFileStateRepository> RepoMock { get; set; }
        public FileService Build()
        {
            var RepoMock = new Mock<IFileStateRepository>();
            var sut = new FileService(RepoMock.Object, new DateTimeProvider());

            return sut;
        }
    }
}
