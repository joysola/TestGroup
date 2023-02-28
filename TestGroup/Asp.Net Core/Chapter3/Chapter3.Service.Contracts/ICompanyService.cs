using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter3.Service.Contracts
{
    public interface ICompanyService
    {
        IEnumerable<Company> GetAllCompanies(bool trackChanges);
    }
}
