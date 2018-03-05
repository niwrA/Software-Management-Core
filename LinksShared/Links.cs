using niwrA.CommandManager;
using DateTimeShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinksShared
{
    public interface ILinkService : ICommandProcessor
    {
        ILink CreateLink(Guid guid, Guid linkForGuid, string url, string name);
        ILink GetLink(Guid guid);
        void DeleteLink(Guid guid);
        void PersistChanges();
    }

    public interface ILinkState : INamedEntityState
    {
        Guid EntityGuid { get; set; }
        string Url { get; set; }
        Guid ForGuid { get; set; }
        string Description { get; set; }
        string ImageUrl { get; set; }
        string SiteName { get; set; }
    }

    public interface ILinkStateRepository : IEntityRepository
    {
        ILinkState CreateLinkState(Guid guid, string name);
        ILinkState GetLinkState(Guid guid);
        IEnumerable<ILinkState> GetLinkStates();
        IEnumerable<ILinkState> GetLinkStatesForGuid(Guid forGuid);
        void DeleteLinkState(Guid guid);
    }
    public interface IEntity
    {
        Guid Guid { get; }
        string Name { get; }
        DateTime CreatedOn { get; }
        void Rename(string name, string originalName);
    }
    public interface ILink : IEntity
    {
        string Url { get; }
        void ChangeUrl(string url, string originalUrl);
        string Description { get; }
        string SiteName { get; }
        string ImageUrl { get; }
        // IImageLink ImageLink { get; }
    }
    public class Link : ILink
    {
        private ILinkState _state;
        private ILinkStateRepository _repo;
        private ILinkDetailsProcessor _detailsProcessor;

        public Link(ILinkState state)
        {
            _state = state;
        }
        //public Link(ILinkState state, ILinkStateRepository repo) : this(state)
        //{
        //    _repo = repo;
        //}
        public Link(ILinkState state, ILinkStateRepository repo, ILinkDetailsProcessor detailsProcessor) : this(state)
        {
            _repo = repo;
            _detailsProcessor = detailsProcessor;
        }
        public Guid Guid { get { return _state.Guid; } }
        public string Name { get { return _state.Name; } }
        public string Url { get { return _state.Url; } }
        public DateTime CreatedOn { get { return _state.CreatedOn; } }

        public string Description { get { return _state.Description; } }

        public string SiteName { get { return _state.SiteName; } }
        public string ImageUrl { get { return _state.ImageUrl; } }

        //        public IImageLink ImageLink { get { return _state.ImageLink; } }

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

        public void ChangeUrl(string url, string originalUrl)
        {
            if (_state.Url == originalUrl)
            {
                _state.Url = url;
                var updatedDetails = _detailsProcessor.ProcessLinkDetails(url);
                _state.Description = updatedDetails.Description;
                _state.ImageUrl = updatedDetails.Image == null ? "" : updatedDetails.Image.Url;
                _state.Name = updatedDetails.Title;
                _state.SiteName = updatedDetails.SiteName;
            }
            else
            {
                // todo: implement concurrency policy
            }
        }
    }
    public class LinkService : ILinkService
    {
        private DateTimeShared.IDateTimeProvider _dateTimeProvider;
        private ILinkStateRepository _repo;
        private ILinkDetailsProcessor _linkDetailsProcessor;

        public LinkService(ILinkStateRepository repo, DateTimeShared.IDateTimeProvider dateTimeProvider, ILinkDetailsProcessor linkDetailsProcessor)
        {
            _repo = repo;
            _dateTimeProvider = dateTimeProvider;
            _linkDetailsProcessor = linkDetailsProcessor;
        }
        public ILink CreateLink(Guid guid, Guid linkForGuid, string url, string name)
        {
            var state = _repo.CreateLinkState(guid, name);
            state.ForGuid = linkForGuid;
            state.CreatedOn = _dateTimeProvider.GetUtcDateTime();
            state.UpdatedOn = _dateTimeProvider.GetUtcDateTime();
            var link = new Link(state, _repo, _linkDetailsProcessor);
            link.ChangeUrl(url, null);
            return link;
        }
        public ILink GetLink(Guid guid)
        {
            var state = _repo.GetLinkState(guid);
            return new Link(state, _repo, _linkDetailsProcessor);
        }
        public void DeleteLink(Guid guid)
        {
            _repo.DeleteLinkState(guid);
        }

        public void PersistChanges()
        {
            _repo.PersistChanges();
        }
    }
    public class LinkBuilder
    {
        private LinkService _links;
        private Guid _guid;
        private string _name;
        private string _url;

        public LinkBuilder(LinkService links)
        {
            _links = links;
        }

        public ILink Build(Guid linkForGuid, string name, string url)
        {
            EnsureGuid();
            var link = _links.CreateLink(_guid, linkForGuid, url, name);
            link.ChangeUrl(_url, null);
            return link;
        }

        private void EnsureGuid()
        {
            if (_guid == null || _guid == Guid.Empty)
            {
                _guid = Guid.NewGuid();
            }
        }

        public LinkBuilder WithGuid(Guid guid)
        {
            _guid = guid;
            return this;
        }

        public LinkBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public LinkBuilder WithUrl(string url)
        {
            _url = url;
            return this;
        }
    }
}
