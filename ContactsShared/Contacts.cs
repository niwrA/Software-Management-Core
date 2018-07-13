using niwrA.CommandManager;
using DateTimeShared;
using System;
using System.Collections.Generic;
using System.Text;
using niwrA.CommandManager.Contracts;

namespace ContactsShared
{
  public interface IContactService : ICommandProcessor
  {
    IContact CreateContact(Guid guid, string name);
    IContact GetContact(Guid guid);
    void DeleteContact(Guid guid);
  }

  public interface IContactState : INamedEntityState
  {
    DateTime? BirthDate { get; set; }
    string Email { get; set; }
    Guid? AvatarFileGuid { get; set; }
    string AvatarUrl { get; set; }
  }

  public interface IContactStateRepository : IEntityRepository
  {
    IContactState CreateContactState(Guid guid, string name);
    IContactState GetContactState(Guid guid);
    void DeleteContactState(Guid guid);
  }
  public interface IContactStateReadOnlyRepository : IEntityReadOnlyRepository
  {
    IContactState GetContactState(Guid guid);
    IEnumerable<IContactState> GetContactStates();
  }

  public interface IEntity
  {
    Guid Guid { get; }
    string Name { get; }
    DateTime CreatedOn { get; }
    void Rename(string name, string originalName);
  }
  public interface IContact : IEntity
  {
    string Email { get; }
    void ChangeBirthDate(DateTime? birthDate, DateTime? originalBirthDate);
    void ChangeEmail(string email, string originalEmail);
    void ChangeAvatar(Guid? avatarFileGuid, Guid? originalAvatarFileGuid, string avatarUrl);
  }
  public class Contact : IContact
  {
    private IContactState _state;
    private IContactStateRepository _repo;

    public Contact(IContactState state)
    {
      _state = state;
    }
    public Contact(IContactState state, IContactStateRepository repo) : this(state)
    {
      _repo = repo;
    }

    public Guid Guid { get { return _state.Guid; } }
    public string Name { get { return _state.Name; } }
    public string Email { get { return _state.Email; } }
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

    public void ChangeEmail(string email, string originalEmail)
    {
      if (_state.Email == originalEmail)
      {
        _state.Email = email;
      }
      else
      {
        // todo: implement concurrency policy
      }
    }

    // todo: implement concurrency policy
    public void ChangeBirthDate(DateTime? birthDate, DateTime? originalBirthDate)
    {
      if (_state.BirthDate == originalBirthDate)
      {
        _state.BirthDate = birthDate;
      }
    }

    public void ChangeAvatar(Guid? avatarFileGuid, Guid? originalAvatarFileGuid, string avatarUrl)
    {
      if (_state.AvatarFileGuid == originalAvatarFileGuid)
      {
        _state.AvatarFileGuid = avatarFileGuid;
        _state.AvatarUrl = avatarUrl;
      }
    }
  }
  public class ContactService : IContactService
  {
    private DateTimeShared.IDateTimeProvider _dateTimeProvider;
    private IContactStateRepository _repo;
    public ContactService(IContactStateRepository repo, DateTimeShared.IDateTimeProvider dateTimeProvider)
    {
      _repo = repo;
      _dateTimeProvider = dateTimeProvider;
    }
    public IContact CreateContact(Guid guid, string name)
    {
      var state = _repo.CreateContactState(guid, name);
      state.CreatedOn = _dateTimeProvider.GetUtcDateTime();
      state.UpdatedOn = _dateTimeProvider.GetUtcDateTime();
      state.Name = name;
      return new Contact(state);
    }
    public IContact GetContact(Guid guid)
    {
      var state = _repo.GetContactState(guid);
      return new Contact(state, _repo);
    }
    public void DeleteContact(Guid guid)
    {
      _repo.DeleteContactState(guid);
    }

    public void PersistChanges()
    {
      _repo.PersistChanges();
    }
  }
  public class ContactBuilder
  {
    private ContactService _contacts;
    private Guid _guid;
    private string _name;
    private string _email;

    public ContactBuilder(ContactService contacts)
    {
      _contacts = contacts;
    }

    public IContact Build(string name)
    {
      EnsureGuid();
      var contact = _contacts.CreateContact(_guid, name);
      contact.ChangeEmail(_email, null);
      return contact;
    }

    private void EnsureGuid()
    {
      if (_guid == null || _guid == Guid.Empty)
      {
        _guid = Guid.NewGuid();
      }
    }

    public ContactBuilder WithGuid(Guid guid)
    {
      _guid = guid;
      return this;
    }

    public ContactBuilder WithName(string name)
    {
      _name = name;
      return this;
    }

    public ContactBuilder WithEmail(string email)
    {
      _email = email;
      return this;
    }
  }
}
