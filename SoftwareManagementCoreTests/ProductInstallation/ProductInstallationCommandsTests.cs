using CommandsShared;
using Moq;
using ProductInstallationsShared;
using SoftwareManagementCoreTests.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SoftwareManagementCoreTests.ProductInstallations
{
    [Trait("Entity", "ProductInstallation")]
    public class ProductInstallationCommandsTests
    {
        [Fact(DisplayName = "CreateProductInstallationCommand")]
        public void CreateCommand()
        {
            var ProductInstallationsMock = new Mock<IProductInstallationService>();
            var sut = new CommandBuilder<CreateProductInstallationCommand>().Build(ProductInstallationsMock.Object) as CreateProductInstallationCommand;

            sut.CompanyGuid = Guid.NewGuid();
            sut.ProductGuid = Guid.NewGuid();
            sut.CompanyEnvironmentGuid = Guid.NewGuid();
            sut.ProductVersionGuid = Guid.NewGuid();
            sut.StartDate = DateTime.Now.Date;
            sut.EndDate = DateTime.Now.Date.AddYears(1);
            sut.Execute();

            ProductInstallationsMock.Verify(s => s.CreateProductInstallation(sut.EntityGuid, sut.CompanyGuid, sut.ProductGuid, sut.CompanyEnvironmentGuid, sut.ProductVersionGuid, sut.StartDate, sut.EndDate), Times.Once);
        }

        // todo: duplicate check on ProductInstallations

        [Fact(DisplayName = "DeleteProductInstallationCommand")]
        public void DeleteCommand()
        {
            var ProductInstallationsMock = new Mock<IProductInstallationService>();
            var sut = new CommandBuilder<DeleteProductInstallationCommand>().Build(ProductInstallationsMock.Object) as DeleteProductInstallationCommand;

            sut.Execute();

            ProductInstallationsMock.Verify(s => s.DeleteProductInstallation(sut.EntityGuid), Times.Once);
        }

        [Fact(DisplayName = "ChangeStartDateOfProductInstallationCommand")]
        public void ChangeProductInstallationOfStartDateCommand()
        {
            var sutBuilder = new ProductInstallationCommandBuilder<ChangeStartDateOfProductInstallationCommand>();
            var sut = sutBuilder.Build() as ChangeStartDateOfProductInstallationCommand;

            sut.OriginalStartDate = DateTime.Now;
            sut.StartDate = DateTime.Now;
            sut.Execute();

            sutBuilder.ProductInstallationMock.Verify(s => s.ChangeStartDate(sut.StartDate, sut.OriginalStartDate), Times.Once);
        }

        [Fact(DisplayName = "ChangeEndDateOfProductInstallationCommand")]
        public void ChangeEndDateOfProductInstallationCommand()
        {
            var sutBuilder = new ProductInstallationCommandBuilder<ChangeEndDateOfProductInstallationCommand>();
            var sut = sutBuilder.Build() as ChangeEndDateOfProductInstallationCommand;

            sut.OriginalEndDate = DateTime.Now;
            sut.EndDate = DateTime.Now;
            sut.Execute();

            sutBuilder.ProductInstallationMock.Verify(s => s.ChangeEndDate(sut.EndDate, sut.OriginalEndDate), Times.Once);
        }
    }

    class ProductInstallationCommandBuilder<T> where T : ICommand, new()
    {
        public Mock<IProductInstallation> ProductInstallationMock { get; set; }
        public ICommand Build()
        {
            var ProductInstallationsMock = new Mock<IProductInstallationService>();
            var ProductInstallationMock = new Mock<IProductInstallation>();
            this.ProductInstallationMock = ProductInstallationMock;

            var sut = new CommandBuilder<T>().Build(ProductInstallationsMock.Object);

            ProductInstallationsMock.Setup(s => s.GetProductInstallation(sut.EntityGuid)).Returns(ProductInstallationMock.Object);

            return sut;
        }
    }
}
