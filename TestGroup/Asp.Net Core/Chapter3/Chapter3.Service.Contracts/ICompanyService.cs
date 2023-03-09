using Entities;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter3.Service.Contracts
{
    public interface ICompanyService
    {
        IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges);

        CompanyDto GetCompany(Guid companyId, bool trackChanges);
    }
}
