using System;
using System.Collections.Generic;
using System.Text;

namespace ProductInstallationShared
{
  public interface IProductInstallationState
  {
    Guid ProductGuid { get; set; }
    Guid ProductVersionGuid { get; set; }
    Guid CompanyGuid { get; set; }
    Guid CompanyEnvironmentGuid { get; set; }
  }
  public interface IProductInstallationRepository
  {

  }
  public class ProductInstallation
  {
    private IProductInstallationState _state;
    private IProductInstallationRepository _repo;
    public ProductInstallation(IProductInstallationState state, IProductInstallationRepository repo)
    {
      _state = state;
      _repo = repo;
    }
    Guid ProductGuid { get { return _state.ProductGuid; } }
    Guid ProductVersionGuid { get { return _state.ProductVersionGuid; } }
    Guid CompanyGuid { get { return _state.CompanyGuid; } }
    Guid CompanyEnvironmentGuid { get { return _state.CompanyEnvironmentGuid; } }
  }
    public class ProductInstallations
    {

    }
}
