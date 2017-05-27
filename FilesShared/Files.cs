using CommandsShared;
using DateTimeShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace FilesShared
{
    public interface IFileService : ICommandProcessor
    {
        IFile CreateFile(Guid guid, Guid forGuid, string forType, string name, string fileName, string type);
        IFile GetFile(Guid guid);
        void DeleteFile(Guid guid);
        void PersistChanges();
    }

    public interface IFileState : INamedEntityState
    {
        Guid EntityGuid { get; set; }
        string FolderName { get; }
        Guid ForGuid { get; set; }
        string ForType { get; set; }
        string Description { get; set; }
        string Type { get; set; }
        string FileName { get; set; }
    }

    public interface IFileStateRepository : IEntityRepository
    {
        IFileState CreateFileState(Guid guid, string name);
        IFileState GetFileState(Guid guid);
        IEnumerable<IFileState> GetFileStates();
        IEnumerable<IFileState> GetFileStatesForGuid(Guid forGuid);
        void DeleteFileState(Guid guid);
    }
    public interface IEntity
    {
        Guid Guid { get; }
        string Name { get; }
        DateTime CreatedOn { get; }
        void Rename(string name, string originalName);
    }
    public interface IFile : IEntity
    {
        string FileName { get; }
        string FolderName { get; }
        string Description { get; }
        string Type { get; }
    }
    public class File : IFile
    {
        private IFileState _state;
        private IFileStateRepository _repo;

        public File(IFileState state)
        {
            _state = state;
        }
        //public File(IFileState state, IFileStateRepository repo) : this(state)
        //{
        //    _repo = repo;
        //}
        public File(IFileState state, IFileStateRepository repo) : this(state)
        {
            _repo = repo;
        }
        public Guid Guid { get { return _state.Guid; } }
        public string Name { get { return _state.Name; } }
        public string FileName { get { return _state.FileName; } }
        public string FolderName { get { return _state.FolderName; } }
        public DateTime CreatedOn { get { return _state.CreatedOn; } }

        public string Description { get { return _state.Description; } }

        //        public IImageFile ImageFile { get { return _state.ImageFile; } }

        public void Rename(string name, string originalName)
        {
            if (_state.Name == originalName)
            {
                _state.Name = name;
            }
            else
            {
                // todo: implement concurrency policy
            }
        }

        public string Type { get { return _state.Type; } }
    }
    public class FileService : IFileService
    {
        private IDateTimeProvider _dateTimeProvider;
        private IFileStateRepository _repo;

        public FileService(IFileStateRepository repo, IDateTimeProvider dateTimeProvider)
        {
            _repo = repo;
            _dateTimeProvider = dateTimeProvider;
        }
        public IFile CreateFile(Guid guid, Guid forGuid, string forType, string name, string fileName, string type)
        {
            var state = _repo.CreateFileState(guid, name);
            state.ForGuid = forGuid;
            state.CreatedOn = _dateTimeProvider.GetUtcDateTime();
            state.UpdatedOn = _dateTimeProvider.GetUtcDateTime();
            state.FileName = fileName;
            state.ForType = forType;
            state.Type = type;
            var link = new File(state, _repo);
            return link;
        }
        public IFile GetFile(Guid guid)
        {
            var state = _repo.GetFileState(guid);
            return new File(state, _repo);
        }
        public void DeleteFile(Guid guid)
        {
            _repo.DeleteFileState(guid);
        }

        public void PersistChanges()
        {
            _repo.PersistChanges();
        }
    }
    public class FileBuilder
    {
        private FileService _files;
        private Guid _guid;
        private string _name;
        private string _url;

        public FileBuilder(FileService files)
        {
            _files = files;
        }

        public IFile Build(Guid linkForGuid, string name, string fileName, string folderName, string type)
        {
            EnsureGuid();
            var link = _files.CreateFile(_guid, linkForGuid, folderName, name, fileName, type);
            return link;
        }

        private void EnsureGuid()
        {
            if (_guid == null || _guid == Guid.Empty)
            {
                _guid = Guid.NewGuid();
            }
        }

        public FileBuilder WithGuid(Guid guid)
        {
            _guid = guid;
            return this;
        }

        public FileBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public FileBuilder WithUrl(string url)
        {
            _url = url;
            return this;
        }
    }
}
