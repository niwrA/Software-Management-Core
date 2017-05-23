using CommandsShared;
using DateTimeShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace FilesShared
{
    public interface IFileService : ICommandProcessor
    {
        IFile CreateFile(Guid guid, Guid linkForGuid, string url, string name);
        IFile GetFile(Guid guid);
        void DeleteFile(Guid guid);
        void PersistChanges();
    }

    public interface IFileState : INamedEntityState
    {
        Guid EntityGuid { get; set; }
        string FolderName { get; set; }
        Guid ForGuid { get; set; }
        string Description { get; set; }
        string ImageUrl { get; set; }
        string Path { get; set; }
        string Type
        {
            get;
        }
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
        string FolderName { get; }
        void MoveToFolder(string targetFolder, string sourceFolder);
        string Description { get; }
        string ImageUrl { get; }
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
        public string FolderName { get { return _state.FolderName; } }
        public DateTime CreatedOn { get { return _state.CreatedOn; } }

        public string Description { get { return _state.Description; } }
        public string ImageUrl { get { return _state.ImageUrl; } }

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

        public void MoveToFolder(string targetFolder, string sourceFolder)
        {
            if (_state.FolderName == sourceFolder)
            {
                _state.FolderName = targetFolder;
            }
            else
            {
                // todo: implement concurrency policy
            }
        }
        public string Path
        {
            get
            {
                return _state.Path;
            }
        }
        public string Type
        {
            get
            {
                return _state.Type;
            }
        }
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
        public IFile CreateFile(Guid guid, Guid linkForGuid, string url, string name)
        {
            var state = _repo.CreateFileState(guid, name);
            state.ForGuid = linkForGuid;
            state.CreatedOn = _dateTimeProvider.GetUtcDateTime();
            state.UpdatedOn = _dateTimeProvider.GetUtcDateTime();
            var link = new File(state, _repo);
            link.MoveToFolder(url, null);
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
        private FileService _links;
        private Guid _guid;
        private string _name;
        private string _url;

        public FileBuilder(FileService links)
        {
            _links = links;
        }

        public IFile Build(Guid linkForGuid, string name, string url)
        {
            EnsureGuid();
            var link = _links.CreateFile(_guid, linkForGuid, url, name);
            link.MoveToFolder(_url, null);
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
