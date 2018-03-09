using ProductsShared;
using System;
using Xunit;
using Moq;
using DateTimeShared;

namespace SoftwareManagementCoreTests
{
  [Trait("Entity", "ProductIssue")]
  public class ProductIssueTests
  {
    [Fact(DisplayName = "Resolve_ImplementsState")]
    public void ResolveProductIssueImplementsState()
    {
      var productIssueState = new Mock<IProductIssueState>();
      var sut = new ProductIssue(productIssueState.Object);
      var guid = Guid.NewGuid();

      sut.Resolve(guid);

      productIssueState.VerifySet(v => v.ResolvedVersionGuid = guid);
    }

  }
}
