using System;
using System.Collections.Generic;
using System.Text;
using CommandsShared;

namespace CompaniesShared
{
    public abstract class CompanyCommand : CommandBase
    {
        public CompanyCommand() : base() { }
        public CompanyCommand(ICommandStateRepository repo) : base(repo) { }
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
            var product = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityGuid);
            product.Rename(this.Name, this.OriginalName);
            base.Execute();
        }
    }
    public class AddRoleToCompanyCommand : CompanyCommand
    {
        public string RoleName { get; set; }
        public Guid RoleGuid { get; set; }
        public override void Execute()
        {
            var company = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityGuid);
            company.AddRoleToCompany(this.RoleGuid, this.RoleName);
            base.Execute();
        }
    }
    public class RemoveRoleFromCompanyCommand : CompanyCommand
    {
        public Guid RoleGuid { get; set; }
        public override void Execute()
        {
            var company = ((ICompanyService)base.CommandProcessor).GetCompany(this.EntityGuid);
            company.RemoveRoleFromCompany(this.RoleGuid);
            base.Execute();
        }
    }

}
