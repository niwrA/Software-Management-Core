﻿using CommandsShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductInstallationsShared
{
  public interface IProductInstallationState
  {
    Guid Guid { get; set; }
    Guid ProductGuid { get; set; }
    Guid? ProductVersionGuid { get; set; }
    Guid CompanyGuid { get; set; }
    Guid? CompanyEnvironmentGuid { get; set; }
    DateTime? StartDate { get; set; }
    DateTime? EndDate { get; set; }
  }
  public interface IProductInstallationStateRepository
  {
    IProductInstallationState CreateProductInstallationState(Guid guid, Guid companyGuid, Guid productGuid);
    IProductInstallationState GetProductInstallationState(Guid guid);
    IEnumerable<IProductInstallationState> GetProductInstallationsByCompanyGuid(Guid companyGuid);
    IEnumerable<IProductInstallationState> GetProductInstallationsByProductGuid(Guid productGuid);
    IEnumerable<IProductInstallationState> GetProductInstallationStates();
    void DeleteProductInstallationState(Guid entityGuid);
    void PersistChanges();
    // IEnumerable<ICompanyState> GetCompaniesByProductGuid(Guid guid);

  }
  public interface IProductInstallation
  {
    Guid Guid { get; }
    Guid ProductGuid { get; }
    Guid? ProductVersionGuid { get; }
    Guid CompanyGuid { get; }
    Guid? CompanyEnvironmentGuid { get; }
    DateTime? StartDate { get; }
    DateTime? EndDate { get; }

    void ChangeStartDate(DateTime? startDate, DateTime? originalStartDate);
    void ChangeEndDate(DateTime? endDate, DateTime? originalEndDate);

  }
  public class ProductInstallation : IProductInstallation
  {
    private IProductInstallationState _state;
    private IProductInstallationStateRepository _repo;
    public ProductInstallation(IProductInstallationState state)
    {
      _state = state;
    }
    public ProductInstallation(IProductInstallationState state, IProductInstallationStateRepository repo) : this(state)
    {
      _repo = repo;
    }
    public Guid Guid { get { return _state.Guid; } }
    public Guid ProductGuid { get { return _state.ProductGuid; } }
    public Guid? ProductVersionGuid { get { return _state.ProductVersionGuid; } }
    public Guid CompanyGuid { get { return _state.CompanyGuid; } }
    public Guid? CompanyEnvironmentGuid { get { return _state.CompanyEnvironmentGuid; } }
    public DateTime? StartDate { get { return _state.StartDate; } }
    public DateTime? EndDate { get { return _state.EndDate; } }

    public void ChangeStartDate(DateTime? startDate, DateTime? originalStartDate)
    {
      if (_state.StartDate == originalStartDate)
      {
        this._state.StartDate = startDate;
      }
    }
    public void ChangeEndDate(DateTime? endDate, DateTime? originalEndDate)
    {
      if (_state.EndDate == originalEndDate)
      {
        this._state.EndDate = endDate;
      }
    }
  }
  public interface IProductInstallationService : ICommandProcessor
  {
    IProductInstallation CreateProductInstallation(Guid guid, Guid companyGuid, Guid productGuid, Guid? companyEnvironmentGuid, Guid? productVersionGuid, DateTime? startDate, DateTime? endDate);
    void DeleteProductInstallation(Guid entityGuid);
    IProductInstallation GetProductInstallation(Guid guid);
    void PersistChanges();

  }
  public class ProductInstallationService : IProductInstallationService
  {
    private IProductInstallationStateRepository _repo;
    public ProductInstallationService(IProductInstallationStateRepository repo)
    {
      _repo = repo;
    }
    public IProductInstallation CreateProductInstallation(Guid guid, Guid companyGuid, Guid productGuid, Guid? companyEnvironmentGuid, Guid? productVersionGuid, DateTime? startDate, DateTime? endDate)
    {
      var state = _repo.CreateProductInstallationState(guid, companyGuid, productGuid);
      state.StartDate = startDate;
      state.EndDate = endDate;
      state.CompanyEnvironmentGuid = companyEnvironmentGuid;
      state.ProductVersionGuid = productVersionGuid;
      return new ProductInstallation(state, _repo);
    }

    public void DeleteProductInstallation(Guid entityGuid)
    {
      _repo.DeleteProductInstallationState(entityGuid);
    }

    public IProductInstallation GetProductInstallation(Guid guid)
    {
      var state = _repo.GetProductInstallationState(guid);
      return new ProductInstallation(state, _repo);
    }
    public void PersistChanges()
    {
      _repo.PersistChanges();
    }

  }
}
