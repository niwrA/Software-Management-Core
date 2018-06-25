using System;
using System.Collections.Generic;
using System.Text;
using niwrA.CommandManager;
using niwrA.CommandManager.Contracts;

namespace CompaniesShared
{
    public abstract class CompanyCommand : CommandBase, ICommand
    {
        public CompanyCommand() : base() { }
        public CompanyCommand(ICommandStateRepository repo) : base(repo) { }
        public new Guid EntityGuid { get { return System.Guid.Parse(base.EntityGuid); } set { EntityGuid = value; } }
        public new Guid EntityRootGuid { get { return System.Guid.Parse(base.EntityRootGuid); } set { EntityRootGuid = value; } }
        public virtual void Execute() { }
    }

    public class DeleteCompanyCommand : CompanyCommand
    {
        public override void Execute()
        {
            ((ICompanyService)base.CommandProcessor).DeleteCompany(this.EntityGuid);
            base.Execute();
        }
    }

    public class CreateCompanyCommand : CompanyCommand
    {
        public string Name { get; set; }
        public override void Execute()
        {
            ((ICompanyService)base.CommandProcessor).CreateCompany(this.EntityGuid, this.Name);
            base.Execute();
        }
    }

    public class RenameCompanyCommand : CompanyCommand
    {
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public override void Execute()
        {
            var product = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityRootGuid);
            product.Rename(this.Name, this.OriginalName);
            base.Execute();
        }
    }
    public class ChangeCodeForCompanyCommand : CompanyCommand
    {
        public string OriginalCode { get; set; }
        public string Code { get; set; }
        public override void Execute()
        {
            var product = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityRootGuid);
            product.ChangeCode(this.Code, this.OriginalCode);
            base.Execute();
        }
    }
    public class ChangeExternalIdForCompanyCommand : CompanyCommand
    {
        public string OriginalExternalId { get; set; }
        public string ExternalId { get; set; }
        public override void Execute()
        {
            var product = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityRootGuid);
            product.ChangeExternalId(this.ExternalId, this.OriginalExternalId);
            base.Execute();
        }
    }
    public class AddCompanyRoleCommand : CompanyCommand
    {
        public string RoleName { get; set; }
        public override void Execute()
        {
            var company = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityRootGuid);
            company.AddRoleToCompany(this.EntityGuid, this.RoleName);
            base.Execute();
        }
    }
    public class RemoveCompanyRoleCommand : CompanyCommand
    {
        public override void Execute()
        {
            var company = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityRootGuid);
            company.RemoveRoleFromCompany(this.EntityGuid);
            base.Execute();
        }
    }

    public class AddCompanyEnvironmentCommand : CompanyCommand
    {
        public string EnvironmentName { get; set; }
        public override void Execute()
        {
            var company = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityRootGuid);
            company.AddEnvironmentToCompany(this.EntityGuid, this.EnvironmentName);
            base.Execute();
        }
    }
    public class RemoveCompanyEnvironmentCommand : CompanyCommand
    {
        public override void Execute()
        {
            var company = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityRootGuid);
            company.RemoveEnvironmentFromCompany(this.EntityGuid);
            base.Execute();
        }
    }

}
