using ContactsShared;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using CompaniesShared;

namespace SoftwareManagementEFCoreRepository
{
  public interface IMainReadOnlyRepository : IContactStateReadOnlyRepository, ICompanyStateReadOnlyRepository { }
  public class MainReadOnlyRepository : IMainReadOnlyRepository
  {
    private MainContext _context;
    public MainReadOnlyRepository(MainContext context)
    {
        _context = context;
    }

    public ICompanyState GetCompanyState(Guid guid)
    {
      return _context.CompanyStates.AsNoTracking().Include(i => i.CompanyRoleStates)
        .Include(i => i.CompanyEnvironmentStates).ThenInclude(ti => ti.HardwareStates)
        .Include(ti => ti.CompanyEnvironmentStates).ThenInclude(ti => ti.DatabaseStates)
        .Include(ti => ti.CompanyEnvironmentStates).ThenInclude(ti => ti.AccountStates)
        .SingleOrDefault(s => s.Guid == guid);
    }

    public IEnumerable<ICompanyState> GetCompanyStates()
    {
      return _context.CompanyStates.AsNoTracking().Include(i => i.CompanyRoleStates)
        .Include(i => i.CompanyEnvironmentStates).ThenInclude(ti => ti.HardwareStates)
        .Include(ti => ti.CompanyEnvironmentStates).ThenInclude(ti => ti.DatabaseStates)
        .Include(ti => ti.CompanyEnvironmentStates).ThenInclude(ti => ti.AccountStates)
        .ToList();
    }

    public IContactState GetContactState(Guid guid)
    {
      return _context.ContactStates.AsNoTracking().SingleOrDefault(s => s.Guid == guid);
    }

    public IEnumerable<IContactState> GetContactStates()
    {
      return _context.ContactStates.AsNoTracking().ToList();
    }
  }
}
