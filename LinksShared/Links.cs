using CommandsShared;
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
        Guid LinkForGuid { get; set; }
    }

    public interface ILinkStateRepository : IEntityRepository
    {
        ILinkState CreateLinkState(Guid guid, string name);
        ILinkState GetLinkState(Guid guid);
        IEnumerable<ILinkState> GetLinkStates();
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
    }
    public class Link : ILink
    {
        private ILinkState _state;
        private ILinkStateRepository _repo;

        public Link(ILinkState state)
        {
            _state = state;
        }
        public Link(ILinkState state, ILinkStateRepository repo) : this(state)
        {
            _repo = repo;
        }

        public Guid Guid { get { return _state.Guid; } }
        public string Name { get { return _state.Name; } }
        public string Url { get { return _state.Url; } }
        public DateTime CreatedOn { get { return _state.CreatedOn; } }

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
            }
            else
            {
                // todo: implement concurrency policy
            }
        }
    }
    public class LinkService : ILinkService
    {
        private IDateTimeProvider _dateTimeProvider;
        private ILinkStateRepository _repo;
        public LinkService(ILinkStateRepository repo, IDateTimeProvider dateTimeProvider)
        {
            _repo = repo;
            _dateTimeProvider = dateTimeProvider;
        }
        public ILink CreateLink(Guid guid, Guid linkForGuid, string url, string name)
        {
            var state = _repo.CreateLinkState(guid, name);
            state.LinkForGuid = linkForGuid;
            state.CreatedOn = _dateTimeProvider.GetUtcDateTime();
            state.UpdatedOn = _dateTimeProvider.GetUtcDateTime();
            state.Url = url;
            return new Link(state);
        }
        public ILink GetLink(Guid guid)
        {
            var state = _repo.GetLinkState(guid);
            return new Link(state, _repo);
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
